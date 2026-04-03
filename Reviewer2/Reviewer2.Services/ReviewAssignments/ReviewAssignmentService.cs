using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
using Reviewer2.Data.Models;
using Reviewer2.Services.DTOs.ReviewAssignments;
using Serilog;

namespace Reviewer2.Services.ReviewAssignments;

///<inheritdoc/>
public class ReviewAssignmentService : IReviewAssignmentService
{
    private readonly IDbContextFactory<ApplicationContext> _contextFactory;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Constructs an instance of the ReviewAssignmentService.
    /// </summary>
    /// <param name="contextFactory">The DbContextFactory used for accessing the database.</param>
    /// <param name="userManager">The Core Identity service for getting user data.</param>
    public ReviewAssignmentService(
        IDbContextFactory<ApplicationContext> contextFactory,
        UserManager<ApplicationUser> userManager)
    {
        _contextFactory = contextFactory;
        _userManager = userManager;
    }
    
    ///<inheritdoc/>
    public async Task<ReviewAssignmentBoardDTO> GetAssignmentBoardAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var papers = await context.Papers
            .AsNoTracking()
            .Where(p => p.Status == PaperStatus.Submitted ||
                        p.Status == PaperStatus.UnderReview)
            .Select(p => new PaperAssignmentDTO
            {
                PaperId = p.Id,
                Title = p.Title,
                Authors = p.Authors
                    .OrderBy(a => a.AuthorOrder)
                    .Select(a => a.FirstName + " " + a.LastName)
                    .ToList(),

                AssignedReviewers = p.ReviewAssignments
                    .Select(r => new AssignedReviewerDTO
                    {
                        AssignmentId = r.Id,
                        ReviewerId = r.ReviewerId,
                        Name = r.Reviewer.FullName,
                        Status = r.Status
                    })
                    .ToList()
            })
            .ToListAsync();

        // Precompute assignment counts in one query
        var assignmentCounts = await context.ReviewAssignments
            .AsNoTracking()
            .GroupBy(a => a.ReviewerId)
            .Select(g => new { ReviewerId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ReviewerId, x => x.Count);

        var reviewerIds = await GetReviewerIdsAsync();

        var reviewers = await context.Users
            .AsNoTracking()
            .Where(u => reviewerIds.Contains(u.Id))
            .Select(u => new ReviewerPoolDTO
            {
                ReviewerId = u.Id,
                Name = u.FullName,
                AssignmentCount = assignmentCounts.GetValueOrDefault(u.Id, 0)
            })
            .ToListAsync();

        return new ReviewAssignmentBoardDTO
        {
            Papers = papers,
            Reviewers = reviewers
        };
    }

    ///<inheritdoc/>
    public async Task<AssignmentResult> AssignReviewerAsync(Guid paperId, Guid reviewerId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        // Optional but recommended: verify paper exists
        bool paperExists = await context.Papers
            .AnyAsync(p => p.Id == paperId);

        if (!paperExists)
            return AssignmentResult.NotFound;

        var reviewerIds = await GetReviewerIdsAsync();

        if (!reviewerIds.Contains(reviewerId))
            return AssignmentResult.NotFound;

        var reviewer = await context.Users
            .Where(u => u.Id == reviewerId)
            .Select(u => new { u.Id, u.Email })
            .FirstAsync();
        
        // Conflict check (author)
        bool isAuthor = await context.Authors.AnyAsync(a =>
            a.PaperId == paperId &&
            (
                (a.UserId != null && a.UserId == reviewer.Id) ||
                (a.UserId == null && a.Email == reviewer.Email)
            ));

        if (isAuthor)
            return AssignmentResult.ReviewerIsAuthor;

        // Duplicate check
        bool exists = await context.ReviewAssignments
            .AnyAsync(a => a.PaperId == paperId && a.ReviewerId == reviewerId);

        if (exists)
            return AssignmentResult.AlreadyAssigned;

        var assignment = new ReviewAssignment
        {
            Id = Guid.NewGuid(),
            PaperId = paperId,
            ReviewerId = reviewerId,
            Status = ReviewStatus.Pending
        };

        context.ReviewAssignments.Add(assignment);

        await context.SaveChangesAsync();

        return AssignmentResult.Success;
    }


    ///<inheritdoc/>
    public async Task<AssignmentResult> RemoveReviewerAsync(Guid assignmentId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var assignment = await context.ReviewAssignments
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment == null)
            return AssignmentResult.NotFound;

        context.ReviewAssignments.Remove(assignment);

        await context.SaveChangesAsync();

        return AssignmentResult.Success;
    }
    
    ///<inheritdoc/>
    public async Task<List<ReviewerCandidateDTO>> GetCandidatesForPaperAsync(Guid paperId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        // Ensure paper exists
        bool paperExists = await context.Papers
            .AsNoTracking()
            .AnyAsync(p => p.Id == paperId);

        if (!paperExists)
            return new List<ReviewerCandidateDTO>();

        // Get author user IDs (for conflict detection)
        var authorUserIds = await context.Authors
            .AsNoTracking()
            .Where(a => a.PaperId == paperId && a.UserId != null)
            .Select(a => a.UserId!.Value)
            .ToHashSetAsync();

        var authorEmails = await context.Authors
            .AsNoTracking()
            .Where(a => a.PaperId == paperId && a.UserId == null)
            .Select(a => a.Email)
            .ToHashSetAsync();

        // Get already assigned reviewers (to exclude from candidates)
        var assignedReviewerIds = await context.ReviewAssignments
            .AsNoTracking()
            .Where(a => a.PaperId == paperId)
            .Select(a => a.ReviewerId)
            .ToHashSetAsync();

        // Precompute assignment counts (avoids per-user query)
        var assignmentCounts = await context.ReviewAssignments
            .AsNoTracking()
            .GroupBy(a => a.ReviewerId)
            .Select(g => new { ReviewerId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ReviewerId, x => x.Count);

        // Load reviewers and project into candidates
        var reviewerIds = await GetReviewerIdsAsync();

        var users = await context.Users
            .AsNoTracking()
            .Where(u => reviewerIds.Contains(u.Id) &&
                        !assignedReviewerIds.Contains(u.Id))
            .ToListAsync();

        var candidates = users
            .Select(u =>
            {
                bool hasConflict =
                    authorUserIds.Contains(u.Id) ||
                    (u.Email != null && authorEmails.Contains(u.Email));

                return new ReviewerCandidateDTO
                {
                    ReviewerId = u.Id,
                    Name = u.FullName,
                    AssignmentCount = assignmentCounts.GetValueOrDefault(u.Id, 0),
                    HasConflict = hasConflict,
                    ConflictReason = hasConflict
                        ? "Author of this paper"
                        : null
                };
            })
            .OrderBy(c => c.HasConflict)
            .ThenBy(c => c.AssignmentCount)
            .ThenBy(c => c.Name)
            .ToList();

        return candidates;
    }
    
    ///<inheritdoc/>
    public async Task<List<AutoAssignmentPreviewDTO>> PreviewAutoAssignAsync(int reviewersPerPaper)
    {
        var (preview, _) = await RunAutoAssignmentAsync(reviewersPerPaper, persist: false);
        return preview;
    }
    
    ///<inheritdoc/>
    public async Task<int> AutoAssignReviewersAsync(int reviewersPerPaper)
    {
        var (_, created) = await RunAutoAssignmentAsync(reviewersPerPaper, persist: true);
        return created;
    }
    
    private async Task<(List<AutoAssignmentPreviewDTO> preview, int created)> RunAutoAssignmentAsync(
        int reviewersPerPaper, bool persist)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var papers = await context.Papers
            .Where(p => p.Status == PaperStatus.Submitted ||
                        p.Status == PaperStatus.UnderReview)
            .Select(p => new { p.Id, p.Title })
            .ToListAsync();

        // Global assignment counts (mutable for fairness)
        var assignmentCounts = await context.ReviewAssignments
            .GroupBy(a => a.ReviewerId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());

        var results = new List<AutoAssignmentPreviewDTO>();
        var created = 0;

        foreach (var paper in papers)
        {
            var existingReviewerIds = await context.ReviewAssignments
                .Where(a => a.PaperId == paper.Id)
                .Select(a => a.ReviewerId)
                .ToHashSetAsync();

            var needed = reviewersPerPaper - existingReviewerIds.Count;

            if (needed <= 0)
            {
                results.Add(new AutoAssignmentPreviewDTO
                {
                    PaperId = paper.Id,
                    Title = paper.Title,
                    SuggestedReviewers = new(),
                    IsIncomplete = false
                });

                continue;
            }

            var candidates = await GetCandidatesForPaperAsync(paper.Id);

            var selected = candidates
                .Where(c => !c.HasConflict)
                .OrderBy(c => assignmentCounts.GetValueOrDefault(c.ReviewerId))
                .ThenBy(c => c.AssignmentCount)
                .Take(needed)
                .ToList();

            // Update in-memory counts (critical for fairness)
            foreach (var reviewer in selected)
            {
                assignmentCounts[reviewer.ReviewerId] =
                    assignmentCounts.GetValueOrDefault(reviewer.ReviewerId) + 1;

                if (!existingReviewerIds.Contains(reviewer.ReviewerId))
                {
                    if (persist)
                    {
                        context.ReviewAssignments.Add(new ReviewAssignment
                        {
                            Id = Guid.NewGuid(),
                            PaperId = paper.Id,
                            ReviewerId = reviewer.ReviewerId,
                            Status = ReviewStatus.Pending
                        });

                        created++;
                    }

                    existingReviewerIds.Add(reviewer.ReviewerId);
                }
            }

            results.Add(new AutoAssignmentPreviewDTO
            {
                PaperId = paper.Id,
                Title = paper.Title,
                SuggestedReviewers = selected,
                IsIncomplete = selected.Count < needed
            });
        }

        if (persist)
            await context.SaveChangesAsync();

        return (results, created);
    }
    
    private async Task<HashSet<Guid>> GetReviewerIdsAsync()
    {
        var reviewers = await _userManager.GetUsersInRoleAsync("Reviewer");
        return reviewers.Select(u => u.Id).ToHashSet();
    }
    
    /// <inheritdoc/>
    public async Task<List<ReviewerPaperDTO>> GetReviewerPapersAsync(Guid reviewerId)
    {
        if (reviewerId == Guid.Empty)
            throw new ArgumentException("Reviewer ID cannot be empty.", nameof(reviewerId));

        Log.Information("Fetching all review assignments for reviewer {ReviewerId}", reviewerId);

        await using var dbContext = await _contextFactory.CreateDbContextAsync();

        // Eagerly load related data for mapping
        var assignments = await dbContext.ReviewAssignments
            .Where(ra => ra.ReviewerId == reviewerId)
            .Include(ra => ra.Review)
            .Include(ra => ra.Paper)
            .ThenInclude(p => p.Authors)
            .ThenInclude(a => a.User)
            .Include(ra => ra.Paper)
            .ThenInclude(p => p.Files)
            .ToListAsync();

        Log.Information("Retrieved {Count} review assignments for reviewer {ReviewerId}", assignments.Count, reviewerId);

        // Map assignments to DTOs using the extension method
        var dtos = assignments
            .Select(ra => ra.ToReviewerPaperDto())
            .ToList();

        Log.Information("Mapped {Count} review assignments to ReviewerPaperDTO for reviewer {ReviewerId}", dtos.Count, reviewerId);

        return dtos;
    }
}