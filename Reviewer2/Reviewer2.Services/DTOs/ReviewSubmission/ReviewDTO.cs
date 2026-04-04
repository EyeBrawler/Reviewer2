using System;
using Reviewer2.Data.Models;

namespace Reviewer2.Services.DTOs.ReviewSubmission;

/// <summary>
/// Data Transfer Object representing a review, typically returned to clients.
/// Contains both structured fields and metadata about the review assignment.
/// </summary>
public class ReviewDTO
{
    /// <summary>
    /// Unique identifier of the review.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The ID of the review template used for this review.
    /// </summary>
    public Guid ReviewTemplateId { get; init; }

    /// <summary>
    /// The ID of the associated review assignment.
    /// </summary>
    public Guid ReviewAssignmentId { get; init; }

    /// <summary>
    /// The UTC timestamp when the review was submitted. Null if in progress.
    /// </summary>
    public DateTimeOffset? SubmittedAtUtc { get; init; }

    /// <summary>
    /// Overall evaluation score assigned by the reviewer. Optional.
    /// </summary>
    public int? OverallScore { get; init; }

    /// <summary>
    /// Self-reported confidence score of the reviewer. Optional.
    /// </summary>
    public int? ConfidenceScore { get; init; }

    /// <summary>
    /// Final recommendation of the reviewer (e.g., "Accept", "Reject"). Optional.
    /// </summary>
    public string? Recommendation { get; init; }

    /// <summary>
    /// Full JSON content representing structured responses to the review template.
    /// Must conform to the associated <see cref="ReviewTemplate"/>.
    /// </summary>
    public string JsonContent { get; init; } = "{}";
}