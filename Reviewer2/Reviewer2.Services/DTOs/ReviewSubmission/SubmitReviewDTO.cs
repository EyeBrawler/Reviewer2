using System;
using System.ComponentModel.DataAnnotations;

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
    [Required]
    public Guid ReviewAssignmentId { get; set; }

    /// <summary>
    /// The overall evaluation score assigned by the reviewer. Optional.
    /// </summary>
    [Range(1, 10, ErrorMessage = "Score must be between 1 and 10.")]
    public int? OverallScore { get; set; }

    /// <summary>
    /// Self-reported confidence score for the review. Optional.
    /// </summary>
    [Range(1, 5, ErrorMessage = "Confidence must be between 1 and 5.")]
    public int? ConfidenceScore { get; set; }

    /// <summary>
    /// The final recommendation for the paper (e.g., "Accept", "Reject"). Optional.
    /// </summary>
    [Required(ErrorMessage = "Recommendation is required.")]
    public string? Recommendation { get; set; }

    /// <summary>
    /// JSON-formatted content containing the structured responses to the review template.
    /// Must conform to the schema defined by the associated <see cref="Reviewer2.Data.Models.ReviewTemplate"/>.
    /// </summary>
    [Required(ErrorMessage = "Review content is required.")]
    public string JsonContent { get; set; } = "{}";
}