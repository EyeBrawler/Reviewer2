using System;
using Reviewer2.Data.Models;

namespace Reviewer2.Services.DTOs.ReviewSubmission;

/// <summary>
/// Extension methods for mapping between Review entities and Review DTOs.
/// </summary>
public static class ReviewMappers
{
    /// <summary>
    /// Maps a <see cref="Review"/> entity to a <see cref="ReviewDTO"/>.
    /// </summary>
    /// <param name="review">The review entity to map.</param>
    /// <returns>A <see cref="ReviewDTO"/> representing the review.</returns>
    public static ReviewDTO ToDTO(this Review review)
    {
        if (review == null) throw new ArgumentNullException(nameof(review));

        return new ReviewDTO
        {
            Id = review.Id,
            ReviewAssignmentId = review.ReviewAssignmentId,
            ReviewTemplateId = review.ReviewTemplateId,
            SubmittedAtUtc = review.SubmittedAtUtc,
            OverallScore = review.OverallScore,
            ConfidenceScore = review.ConfidenceScore,
            Recommendation = review.Recommendation,
            JsonContent = review.JsonContent
        };
    }

    /// <summary>
    /// Maps a <see cref="SubmitReviewDTO"/> to a <see cref="Review"/> entity.
    /// Useful when creating or updating a review from client input.
    /// </summary>
    /// <param name="dto">The submit review DTO to map.</param>
    /// <param name="existingReview">Optional existing review entity for updates.</param>
    /// <returns>A <see cref="Review"/> entity populated with data from the DTO.</returns>
    public static Review ToEntity(this SubmitReviewDTO dto, Review? existingReview = null)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var review = existingReview ?? new Review
        {
            Id = Guid.NewGuid(),
            ReviewAssignmentId = dto.ReviewAssignmentId
        };

        review.OverallScore = dto.OverallScore;
        review.ConfidenceScore = dto.ConfidenceScore;
        review.Recommendation = dto.Recommendation;
        review.JsonContent = dto.JsonContent ?? "{}";

        return review;
    }
}