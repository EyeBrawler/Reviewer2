using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
using Reviewer2.Data.Models;
using Reviewer2.Services.CRUD.FileStorage;
using Reviewer2.Services.DTOs.FileStorage;
using Reviewer2.Services.DTOs.PaperSubmission;
using Serilog;

namespace Reviewer2.Services.Submissions.PaperSubmission;

/// <inheritdoc/>
public class PaperQueryService : IPaperQueryService
{
    private readonly IDbContextFactory<ApplicationContext> _contextFactory;
    private readonly IFileStorageService _fileStorageService;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaperQueryService"/> class.
    /// </summary>
    /// <param name="contextFactory">Factory for creating <see cref="ApplicationContext"/> instances for database access.</param>
    /// <param name="fileStorageService">Service used to access and manage stored paper files.</param>
    /// <param name="userManager">ASP.NET Core Identity <see cref="UserManager{TUser}"/> used for retrieving user roles and information.</param>
    /// <exception cref="ArgumentNullException">Thrown if any of the arguments are null.</exception>
    public PaperQueryService(
        IDbContextFactory<ApplicationContext> contextFactory,
        IFileStorageService fileStorageService,
        UserManager<ApplicationUser> userManager)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserPaperDTO>> GetUserPapersAsync(Guid userId, PaperStatus? status = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        Log.Information("Fetching papers for user {UserId} with status {Status}", userId, status);

        await using var dbContext = await _contextFactory.CreateDbContextAsync();

        var query = dbContext.Papers
            .Include(p => p.Authors)
                .ThenInclude(a => a.User)
            .Include(p => p.Files)
            .Where(p => p.SubmitterUserId == userId);

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        var papers = await query
            .OrderByDescending(p => p.SubmittedAtUtc)
            .ToListAsync();

        Log.Information("Retrieved {Count} papers for user {UserId}", papers.Count, userId);

        var result = papers.Select(p => new UserPaperDTO
        {
            PaperId = p.Id,
            Title = p.Title,
            Status = p.Status.ToString(),
            SubmittedAtUtc = p.SubmittedAtUtc,
            DecisionMadeAtUtc = p.DecisionMadeAtUtc,
            Authors = string.Join(", ", p.Authors
                .OrderBy(a => a.AuthorOrder)
                .Select(a =>
                {
                    // Use linked user if available, otherwise fallback to author snapshot
                    var name = a.User != null 
                        ? $"{a.User.FirstName} {a.User.LastName}" 
                        : $"{a.FirstName} {a.LastName}";

                    // Determine role string for display
                    string role = "";
                    if (a.IsCorrespondingAuthor) role = "Corresponding";
                    else if (a.IsPresenter) role = "Presenter";

                    return string.IsNullOrEmpty(role) ? name : $"{name} ({role})";
                })),
            Files = p.Files.Select(f => new PaperFileSummaryDTO
            {
                FileId = f.Id,
                FileName = f.OriginalFileName,
                FileType = f.Type.ToString(),
                FileUrl = null // Can be populated later with a service-generated URL
            }).ToList()
        }).ToList();

        Log.Information("Mapped papers for user {UserId} to DTOs", userId);

        return result;
    }

    /// <inheritdoc/>
    public async Task<PaperDetailsDTO> GetPaperDetailsAsync(Guid paperId, Guid userId)
    {
        if (paperId == Guid.Empty)
            throw new ArgumentException("Paper ID cannot be empty.", nameof(paperId));
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        await using var dbContext = await _contextFactory.CreateDbContextAsync();

        // Load paper with authors, submitter, and files
        var paper = await dbContext.Papers
            .Include(p => p.Authors)
                .ThenInclude(a => a.User)
            .Include(p => p.Files)
            .Include(p => p.Submitter)
            .FirstOrDefaultAsync(p => p.Id == paperId);

        if (paper == null)
        {
            Log.Warning("GetPaperDetailsAsync: Paper {PaperId} not found for user {UserId}", paperId, userId);
            throw new KeyNotFoundException($"Paper with ID {paperId} was not found.");
        }

        // Ensure requesting user exists
        var requestingUser = await dbContext.Users.FindAsync(userId);
        if (requestingUser == null)
        {
            Log.Warning("GetPaperDetailsAsync: Requesting user {UserId} not found", userId);
            throw new UnauthorizedAccessException("User not found.");
        }

        // Authorization: submitter or privileged roles
        var privilegedRoles = new[] { "Admin", "ConferenceChair", "PaperChair" };
        var userRoles = await _userManager.GetRolesAsync(requestingUser);
        if (paper.SubmitterUserId != userId && !userRoles.Intersect(privilegedRoles).Any())
        {
            Log.Warning("GetPaperDetailsAsync: User {UserId} attempted to access paper {PaperId} without permission", userId, paperId);
            throw new UnauthorizedAccessException("You do not have permission to view this paper.");
        }

        // Map to DTO
        var dto = new PaperDetailsDTO
        {
            PaperId = paper.Id,
            Title = paper.Title,
            Abstract = paper.Abstract,
            Status = paper.Status.ToString(),
            SubmittedAtUtc = paper.SubmittedAtUtc,
            DecisionMadeAtUtc = paper.DecisionMadeAtUtc,
            SubmitterName = $"{paper.Submitter.FirstName} {paper.Submitter.LastName}",
            Authors = paper.Authors
                .OrderBy(a => a.AuthorOrder)
                .Select(a => a.User != null
                    ? $"{a.User.FirstName} {a.User.LastName}"
                    : $"{a.FirstName} {a.LastName}")
                .ToList(),
            Files = paper.Files
                .Select(f =>
                {
                    // Generate secure API endpoint URL for preview/download
                    // This points to the PaperFilesController we created
                    string fileUrl = $"/api/papers/{paper.Id}/files/{f.Type}";

                    return new PaperFileSummaryDTO
                    {
                        FileId = f.Id,
                        FileName = f.OriginalFileName,
                        FileType = f.Type.ToString(),
                        FileUrl = fileUrl
                    };
                })
                .ToList()
        };

        Log.Information("GetPaperDetailsAsync: User {UserId} retrieved details for paper {PaperId}", userId, paperId);
        return dto;
    }
    
    /// <inheritdoc/>
    public async Task<FileReadResult> GetPaperFileAsync(
        Guid paperId,
        FileType fileType,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (paperId == Guid.Empty)
            throw new ArgumentException("Paper ID cannot be empty.", nameof(paperId));
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        await using var dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken);

        // Load the paper including submitter
        var paper = await dbContext.Papers
            .Include(p => p.Submitter)
            .Include(p => p.Files)
            .FirstOrDefaultAsync(p => p.Id == paperId, cancellationToken);

        if (paper == null)
        {
            Log.Warning("GetPaperFileAsync: Paper {PaperId} not found for user {UserId}", paperId, userId);
            return FileReadResult.NotFound($"Paper with ID {paperId} not found.");
        }

        // Load requesting user
        var requestingUser = await dbContext.Users.FindAsync([userId], cancellationToken);
        if (requestingUser == null)
        {
            Log.Warning("GetPaperFileAsync: Requesting user {UserId} not found", userId);
            return FileReadResult.NotFound("Requesting user not found.");
        }

        // Authorization check: submitter or privileged roles
        var privilegedRoles = new[] { "Admin", "ConferenceChair", "PaperChair" };
        var userRoles = await _userManager.GetRolesAsync(requestingUser);
        if (paper.SubmitterUserId != userId && !userRoles.Intersect(privilegedRoles).Any())
        {
            Log.Warning("GetPaperFileAsync: User {UserId} attempted to access paper {PaperId} without permission", userId, paperId);
            return FileReadResult.NotFound("You do not have permission to access this file.");
        }

        // Find the requested file
        var file = paper.Files.FirstOrDefault(f => f.Type == fileType);
        if (file == null)
        {
            Log.Warning("GetPaperFileAsync: File of type {FileType} not found for paper {PaperId}", fileType, paperId);
            return FileReadResult.NotFound($"File of type {fileType} not found for this paper.");
        }

        try
        {
            // Open file stream from storage service
            var storageResult = await _fileStorageService.TryOpenReadAsync(file.StoragePath, cancellationToken);
            if (!storageResult.Success || storageResult.Stream == null)
            {
                Log.Warning("GetPaperFileAsync: Failed to read file {FileId} from storage", file.Id);
                return FileReadResult.NotFound(storageResult.ErrorMessage ?? "File could not be read from storage.");
            }

            Log.Information("GetPaperFileAsync: User {UserId} retrieved file {FileId} for paper {PaperId}", userId, file.Id, paperId);
            return storageResult;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "GetPaperFileAsync: Exception while retrieving file {FileId} for paper {PaperId}", file.Id, paperId);
            return FileReadResult.NotFound("An error occurred while retrieving the file.");
        }
    }
    
    /// <inheritdoc/>
    public async Task<IEnumerable<UserPaperDTO>> GetAllPapersAsync(PaperStatus? status = null)
    {
        Log.Information("Fetching all papers with status {Status}", status);

        await using var dbContext = await _contextFactory.CreateDbContextAsync();

        var query = dbContext.Papers
            .Include(p => p.Authors)
            .ThenInclude(a => a.User)
            .Include(p => p.Files)
            .Include(p => p.Submitter)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        var papers = await query
            .OrderByDescending(p => p.SubmittedAtUtc)
            .ToListAsync();

        Log.Information("Retrieved {Count} papers from database", papers.Count);

        var result = papers.Select(p => new UserPaperDTO
        {
            PaperId = p.Id,
            Title = p.Title,
            Status = p.Status.ToString(),
            SubmittedAtUtc = p.SubmittedAtUtc,
            DecisionMadeAtUtc = p.DecisionMadeAtUtc,
            Authors = string.Join(", ", p.Authors
                .OrderBy(a => a.AuthorOrder)
                .Select(a =>
                {
                    var name = a.User != null
                        ? $"{a.User.FirstName} {a.User.LastName}"
                        : $"{a.FirstName} {a.LastName}";

                    string role = "";
                    if (a.IsCorrespondingAuthor) role = "Corresponding";
                    else if (a.IsPresenter) role = "Presenter";

                    return string.IsNullOrEmpty(role) ? name : $"{name} ({role})";
                })),
            Files = p.Files.Select(f => new PaperFileSummaryDTO
            {
                FileId = f.Id,
                FileName = f.OriginalFileName,
                FileType = f.Type.ToString(),
                FileUrl = $"/api/papers/{p.Id}/files/{f.Type}" // simple API route for now
            }).ToList()
        }).ToList();

        Log.Information("Mapped all papers to DTOs");

        return result;
    }
}
