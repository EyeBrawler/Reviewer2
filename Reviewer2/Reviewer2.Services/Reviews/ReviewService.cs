using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
using Reviewer2.Data.Models;
using Reviewer2.Services.DTOs.ReviewSubmission;

namespace Reviewer2.Services.Reviews;

/// <inheritdoc/>
public class ReviewService : IReviewService 
{
    private readonly IDbContextFactory<ApplicationContext> _dbContextFactory;
    
    // This will have to move later
    private const int RequiredReviewsPerPaper = 3;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ReviewService"/> class.
    /// </summary>
    /// <param name="dbContextFactory">
    /// Factory used to create <see cref="ApplicationContext"/> instances for database operations.
    /// </param>
    public ReviewService(IDbContextFactory<ApplicationContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    /// <inheritdoc/>
    public async Task<ReviewDTO?> GetReviewAsync(
        Guid reviewAssignmentId,
        Guid reviewerUserId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var review = await db.Reviews
            .AsNoTracking()
            .Where(r => r.ReviewAssignmentId == reviewAssignmentId &&
                        r.ReviewAssignment.ReviewerId == reviewerUserId)
            .Select(r => r.ToDTO())
            .FirstOrDefaultAsync(cancellationToken);

        if (review != null)
        {
            return review;
        }

        var assignment = await db.ReviewAssignments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == reviewAssignmentId, cancellationToken);

        if (assignment == null)
        {
            throw new KeyNotFoundException("Review assignment not found.");
        }

        if (assignment.ReviewerId != reviewerUserId)
        {
            throw new UnauthorizedAccessException("You are not authorized to access this review.");
        }

        return null;
    }
    
    /// <inheritdoc/>
    public async Task<ReviewDTO> SaveDraftAsync(
        SubmitReviewDTO dto,
        Guid reviewerUserId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var assignment = await db.ReviewAssignments
            .Include(a => a.Review)
            .FirstOrDefaultAsync(a => a.Id == dto.ReviewAssignmentId, cancellationToken);


        if (assignment == null)
        {
            throw new KeyNotFoundException("Review assignment not found.");
        }

        if (assignment.ReviewerId != reviewerUserId)
        {
            throw new UnauthorizedAccessException("You are not authorized to modify this review.");
        }

        if (assignment.Status == ReviewStatus.Submitted)
        {
            throw new InvalidOperationException("Submitted reviews cannot be modified.");
        }

        // CREATE
        
        if (assignment.Review == null)
        {
            var template = await db.ReviewTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.IsActive, cancellationToken);

            if (template == null)
            {
                throw new InvalidOperationException("No active review template is available.");
            }
            
            var review = dto.ToEntity();
            review.Id = Guid.NewGuid();
            review.ReviewAssignmentId = assignment.Id;
            review.ReviewTemplateId = template.Id;

            db.Reviews.Add(review); 
            assignment.Review = review;
        }
        else
        {
            // update tracked entity properties only
            assignment.Review.OverallScore = dto.OverallScore;
            assignment.Review.ConfidenceScore = dto.ConfidenceScore;
            assignment.Review.Recommendation = dto.Recommendation;
            assignment.Review.JsonContent = dto.JsonContent;
        }

        // Status transition
        if (assignment.Status == ReviewStatus.Pending)
        {
            assignment.Status = ReviewStatus.InProgress;
        }

        await db.SaveChangesAsync(cancellationToken);

        return assignment.Review.ToDTO();
    }
    
    /// <inheritdoc/>
    public async Task<ReviewDTO> SubmitReviewAsync(
    SubmitReviewDTO dto,
    Guid reviewerUserId,
    CancellationToken cancellationToken = default)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var assignment = await db.ReviewAssignments
            .Include(a => a.Review)
            .Include(a => a.Paper)
                .ThenInclude(p => p.ReviewAssignments)
            .FirstOrDefaultAsync(
                a => a.Id == dto.ReviewAssignmentId,
                cancellationToken);

        if (assignment == null)
        {
            throw new KeyNotFoundException("Review assignment not found.");
        }

        if (assignment.ReviewerId != reviewerUserId)
        {
            throw new UnauthorizedAccessException("You are not authorized to submit this review.");
        }

        if (assignment.Status is ReviewStatus.Declined or ReviewStatus.Withdrawn)
        {
            throw new InvalidOperationException("Cannot submit review in current state.");
        }

        if (assignment.Status == ReviewStatus.Submitted)
        {
            throw new InvalidOperationException("Review has already been submitted.");
        }

        // Create or update review
        if (assignment.Review == null)
        {
            var template = await db.ReviewTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.IsActive, cancellationToken);

            if (template == null)
            {
                throw new InvalidOperationException("No active review template is available.");
            }

            var review = dto.ToEntity();
            review.Id = Guid.NewGuid();
            review.ReviewAssignmentId = assignment.Id;
            review.ReviewTemplateId = template.Id;

            db.Reviews.Add(review);
            assignment.Review = review;
        }
        else
        {
            dto.ToEntity(assignment.Review);
        }

        var reviewEntity = assignment.Review;

        if (string.IsNullOrWhiteSpace(reviewEntity.Recommendation))
        {
            throw new InvalidOperationException("Recommendation is required for submission.");
        }

        // Finalize review
        reviewEntity.SubmittedAtUtc = DateTimeOffset.UtcNow;
        assignment.Status = ReviewStatus.Submitted;

        // Check if paper is ready to transition
        var paper = assignment.Paper;

        var submittedReviewsCount = paper.ReviewAssignments
            .Count(ra => ra.Status == ReviewStatus.Submitted);

        if (paper.Status == PaperStatus.UnderReview &&
            submittedReviewsCount >= RequiredReviewsPerPaper)
        {
            paper.MarkReviewsCompleted();
        }

        await db.SaveChangesAsync(cancellationToken);

        return reviewEntity.ToDTO();
    }
    
    /// <inheritdoc/>
    public async Task<IReadOnlyList<ReviewDTO>> GetReviewsForPaperAsync(
        Guid paperId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // First, ensure the paper exists
        var paperExists = await db.Papers
            .AsNoTracking()
            .AnyAsync(p => p.Id == paperId, cancellationToken);

        if (!paperExists)
        {
            throw new KeyNotFoundException("Paper not found.");
        }

        // Fetch all reviews tied to the paper via assignments
        var reviews = await db.Reviews
            .AsNoTracking()
            .Where(r => r.ReviewAssignment.PaperId == paperId)
            .Select(r => r.ToDTO())
            .ToListAsync(cancellationToken);

        return reviews;
    }
    
    /// <inheritdoc/>
    public async Task<bool> ReviewExistsAsync(
        Guid reviewAssignmentId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.Reviews
            .AsNoTracking()
            .AnyAsync(r => r.ReviewAssignmentId == reviewAssignmentId, cancellationToken);
    }
}