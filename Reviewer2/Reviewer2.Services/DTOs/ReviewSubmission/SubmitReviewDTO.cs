using System;
using System.Collections.Generic;
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
    /// Gets or sets the collection of dynamic field values submitted as part
    /// of the review, keyed by the corresponding review template field name.
    /// </summary>
    [Required]
    public Dictionary<string, ReviewValue> Values { get; set; } = new();
}