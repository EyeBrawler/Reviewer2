using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
using Reviewer2.Data.Models;
using Reviewer2.Services.CRUD.FileStorage;
using Reviewer2.Services.DTOs.FileStorage;
using Reviewer2.Services.DTOs.PaperSubmission;
using Serilog;

namespace Reviewer2.Services.Submissions.PaperSubmission;

/// <inheritdoc/>
public class PaperSubmissionService : IPaperSubmissionService
{
    private readonly IDbContextFactory<ApplicationContext> _contextFactory;
    private readonly IFileStorageService _fileStorage;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaperSubmissionService"/> class.
    /// </summary>
    /// <param name="contextFactory">
    /// A factory used to create <see cref="ApplicationContext"/> instances
    /// for interacting with the persistence layer.
    /// </param>
    /// <param name="fileStorage">
    /// The file storage service responsible for saving, retrieving,
    /// and deleting physical files associated with paper submissions.
    /// </param>
    public PaperSubmissionService(
        IDbContextFactory<ApplicationContext> contextFactory,
        IFileStorageService fileStorage)
    {
        _contextFactory = contextFactory;
        _fileStorage = fileStorage;
    }

    /// <inheritdoc/>
    public async Task<Guid> CreateDraftAsync(CreateDraftRequest request, Guid userId)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        await using var context = await _contextFactory.CreateDbContextAsync();

        try
        {
            // Use the aggregate factory directly
            var paper = Paper.CreateDraft(
                submitterUserId: userId,
                title: request.Title,
                abstractText: request.Abstract,
                authorsInput: request.Authors);

            await context.Papers.AddAsync(paper);
            await context.SaveChangesAsync();

            Log.Information("Draft paper {PaperId} created by user {UserId}", paper.Id, userId);

            return paper.Id;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create draft paper for user {UserId}", userId);
            throw new InvalidOperationException("An error occurred while creating the draft paper. See logs for details.", ex);
        }
    }
    
    /// <inheritdoc/>
    public async Task UpdateDraftAsync(Guid paperId, UpdateDraftRequest request, Guid userId)
    {
        ArgumentNullException.ThrowIfNull(request);

        await using var context = await _contextFactory.CreateDbContextAsync();

        try
        {
            var paper = await context.Papers
                .Include(p => p.Authors)
                .FirstOrDefaultAsync(p => p.Id == paperId);

            if (paper is null)
            {
                Log.Warning("UpdateDraftAsync failed: paper {PaperId} not found", paperId);
                throw new InvalidOperationException("Paper not found.");
            }

            if (paper.SubmitterUserId != userId)
            {
                Log.Warning("Unauthorized update attempt for paper {PaperId} by user {UserId}", paperId, userId);
                throw new UnauthorizedAccessException("You are not authorized to modify this paper.");
            }

            // Update metadata
            paper.UpdateMetadata(request.Title, request.Abstract);

            // Map and replace authors
            var updatedAuthors = MapAuthors(request.Authors);
            paper.ReplaceAuthors(updatedAuthors);

            await context.SaveChangesAsync();

            Log.Information("Draft paper {PaperId} updated by user {UserId}", paperId, userId);
        }
        catch (Exception ex) when (ex is not (InvalidOperationException or UnauthorizedAccessException))
        {
            Log.Error(ex, "Unexpected error updating draft paper {PaperId} by user {UserId}", paperId, userId);
            throw new InvalidOperationException("An unexpected error occurred while updating the draft. See logs for details.", ex);
        }
    }
    
    private static List<Author> MapAuthors(IEnumerable<AuthorInputDTO> authorsInput)
    {
        if (authorsInput == null || !authorsInput.Any())
            throw new ArgumentException("At least one author is required.", nameof(authorsInput));

        return authorsInput.Select((dto, index) => new Author
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Institution = dto.Institution,
            IsCorrespondingAuthor = dto.IsCorrespondingAuthor,
            IsPresenter = dto.IsPresenter,
            AuthorOrder = index
        }).ToList();
    }

    /// <inheritdoc/>
    public async Task<StoredFileResult> UploadFileAsync(
        Guid paperId,
        FileType fileType,
        Stream stream,
        string originalFileName,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!stream.CanRead)
        {
            Log.Warning("UploadFileAsync called with unreadable stream by user {UserId} for paper {PaperId}", userId, paperId);
            return StoredFileResult.Error("Invalid file stream.");
        }

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        try
        {
            var paper = await context.Papers
                .Include(p => p.Files)
                .FirstOrDefaultAsync(p => p.Id == paperId, cancellationToken);

            if (paper is null)
            {
                Log.Warning("UploadFileAsync failed: paper {PaperId} not found", paperId);
                return StoredFileResult.Error("Paper not found.");
            }

            if (paper.SubmitterUserId != userId)
            {
                Log.Warning("Unauthorized file upload attempt for paper {PaperId} by user {UserId}", paperId, userId);
                return StoredFileResult.Error("You are not authorized to modify this paper.");
            }

            if (!stream.CanSeek)
            {
                Log.Warning("Stream cannot seek for paper {PaperId} by user {UserId}", paperId, userId);
                return StoredFileResult.Error("Stream must support seeking to determine file size.");
            }

            var sizeInBytes = stream.Length;
            Log.Information("Starting file upload for paper {PaperId} by user {UserId}, file size: {Size} bytes", paperId, userId, sizeInBytes);

            // Store file first
            var storageResult = await _fileStorage.SaveAsync(stream, originalFileName, cancellationToken);

            if (!storageResult.Exists || string.IsNullOrWhiteSpace(storageResult.PhysicalPath))
            {
                Log.Error("File storage failed for paper {PaperId} by user {UserId}: {ErrorMessage}", paperId, userId, storageResult.ErrorMessage);
                return storageResult;
            }

            var storagePath = storageResult.PhysicalPath;

            try
            {
                // Construct domain entity
                var paperFile = new PaperFile(
                    paper.Id,
                    fileType,
                    storagePath,
                    originalFileName,
                    sizeInBytes);

                // Replace existing file of same type
                paper.ReplaceFile(paperFile);

                await context.SaveChangesAsync(cancellationToken);

                Log.Information("File successfully uploaded for paper {PaperId} by user {UserId}, path: {Path}", paperId, userId, storagePath);
                return StoredFileResult.Success(storagePath);
            }
            catch (Exception ex)
            {
                // Compensation — delete file if DB save fails
                await _fileStorage.DeleteAsync(storagePath, cancellationToken);

                Log.Error(ex, "Failed to persist file metadata for paper {PaperId} by user {UserId}. File deleted.", paperId, userId);

                return StoredFileResult.Error(
                    "The file was stored but could not be associated with the paper.");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error during UploadFileAsync for paper {PaperId} by user {UserId}", paperId, userId);
            return StoredFileResult.Error("An unexpected error occurred while uploading the file. See logs for details.");
        }
    }

    /// <inheritdoc/>
    public async Task SubmitAsync(Guid paperId, Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        try
        {
            // Load the paper along with authors and files
            var paper = await context.Papers
                .Include(p => p.Authors)
                .Include(p => p.Files)
                .FirstOrDefaultAsync(p => p.Id == paperId);

            if (paper is null)
            {
                Log.Warning("SubmitAsync failed: paper {PaperId} not found", paperId);
                throw new InvalidOperationException("Paper not found.");
            }

            if (paper.SubmitterUserId != userId)
            {
                Log.Warning("Unauthorized submit attempt for paper {PaperId} by user {UserId}", paperId, userId);
                throw new UnauthorizedAccessException("You are not authorized to submit this paper.");
            }

            // Aggregate handles all business rules and validations
            try
            {
                paper.Submit();
            }
            catch (InvalidOperationException ex)
            {
                Log.Warning(ex,
                    "Paper {PaperId} failed submission due to business rule violation", paperId);
                throw;
            }

            // Persist state change
            await context.SaveChangesAsync();

            Log.Information("Paper {PaperId} successfully submitted by user {UserId}", paperId, userId);
        }
        catch (Exception ex) when (!(ex is InvalidOperationException || ex is UnauthorizedAccessException))
        {
            // Catch unexpected exceptions for diagnostics
            Log.Error(ex, "Unexpected error during SubmitAsync for paper {PaperId} by user {UserId}", paperId, userId);
            throw new InvalidOperationException("An unexpected error occurred while submitting the paper. See logs for details.", ex);
        }
    }
}