using System;

namespace Reviewer2.Services.DTOs.ReviewSubmission;

/// <summary>
/// Data Transfer Object used when submitting a new review.
/// Contains the information required to create or update a review.
/// </summary>
public class SubmitReviewDTO
{
    /// <summary>
    /// The ID of the review assignment being submitted.
    /// This links the review to a specific paper and reviewer.
    /// </summary>
    public Guid ReviewAssignmentId { get; init; }

    /// <summary>
    /// The overall evaluation score assigned by the reviewer. Optional.
    /// </summary>
    public int? OverallScore { get; init; }

    /// <summary>
    /// Self-reported confidence score for the review. Optional.
    /// </summary>
    public int? ConfidenceScore { get; init; }

    /// <summary>
    /// The final recommendation for the paper (e.g., "Accept", "Reject"). Optional.
    /// </summary>
    public string? Recommendation { get; init; }

    /// <summary>
    /// JSON-formatted content containing the structured responses to the review template.
    /// Must conform to the schema defined by the associated <see cref="Reviewer2.Data.Models.ReviewTemplate"/>.
    /// </summary>
    public string JsonContent { get; init; } = "{}";
}