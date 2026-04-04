using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Reviewer2.Data.Models;
using Reviewer2.Services.DTOs.ReviewSubmission;

namespace Reviewer2.Services.Reviews;

/// <summary>
/// Defines operations for managing reviews within the system.
/// 
/// This service is responsible for:
/// - Creating and updating review drafts
/// - Submitting finalized reviews
/// - Retrieving review data for reviewers, papers, and chairs
/// - Enforcing authorization and review lifecycle rules
/// </summary>
public interface IReviewService
{
    /// <summary>
    /// Retrieves the review associated with a specific review assignment
    /// for the given reviewer.
    /// 
    /// This method ensures that only the assigned reviewer may access
    /// their review.
    /// </summary>
    /// <param name="reviewAssignmentId">
    /// The unique identifier of the review assignment.
    /// </param>
    /// <param name="reviewerUserId">
    /// The unique identifier of the reviewer requesting the review.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="ReviewDTO"/> if a review exists; otherwise, null.
    /// </returns>
    Task<ReviewDTO?> GetReviewAsync(
        Guid reviewAssignmentId,
        Guid reviewerUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates a review draft for a given review assignment.
    /// 
    /// If a review does not yet exist, it will be created in an
    /// "in-progress" state. If it already exists, it will be updated.
    /// 
    /// This operation does NOT mark the review as submitted.
    /// </summary>
    /// <param name="dto">
    /// The review data to persist.
    /// </param>
    /// <param name="reviewerUserId">
    /// The unique identifier of the reviewer saving the draft.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// The updated <see cref="ReviewDTO"/> representing the saved draft.
    /// </returns>
    Task<ReviewDTO> SaveDraftAsync(
        SubmitReviewDTO dto,
        Guid reviewerUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Submits a completed review for a given review assignment.
    /// 
    /// This operation:
    /// - Validates the review content against the template schema
    /// - Marks the review as submitted
    /// - Sets the submission timestamp
    /// - Updates the associated <see cref="ReviewAssignment"/> status
    /// 
    /// Once submitted, the review is considered final and may no longer
    /// be modified (unless explicitly allowed by business rules).
    /// </summary>
    /// <param name="dto">
    /// The completed review data to submit.
    /// </param>
    /// <param name="reviewerUserId">
    /// The unique identifier of the reviewer submitting the review.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// The submitted <see cref="ReviewDTO"/>.
    /// </returns>
    Task<ReviewDTO> SubmitReviewAsync(
        SubmitReviewDTO dto,
        Guid reviewerUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all reviews associated with a specific paper.
    /// 
    /// This method is typically used by conference chairs or administrators
    /// to evaluate submissions.
    /// </summary>
    /// <param name="paperId">
    /// The unique identifier of the paper.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A collection of <see cref="ReviewDTO"/> objects representing
    /// all reviews for the paper.
    /// </returns>
    Task<IReadOnlyList<ReviewDTO>> GetReviewsForPaperAsync(
        Guid paperId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether a review exists for a given review assignment.
    /// 
    /// This can be used to quickly check if a reviewer has started
    /// or completed their review.
    /// </summary>
    /// <param name="reviewAssignmentId">
    /// The unique identifier of the review assignment.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// True if a review exists; otherwise, false.
    /// </returns>
    Task<bool> ReviewExistsAsync(
        Guid reviewAssignmentId,
        CancellationToken cancellationToken = default);
}
