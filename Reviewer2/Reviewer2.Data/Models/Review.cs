using System;

namespace Reviewer2.Data.Models;

/// <summary>
/// Represents a completed or in-progress review of a paper.
/// 
/// A Review is created in association with a <see cref="ReviewAssignment"/>
/// and follows a specific <see cref="ReviewTemplate"/> that defines the
/// structure and required fields.
/// 
/// The model supports both structured, queryable fields (e.g., scores,
/// recommendation) and flexible JSON-based content for customizable
/// review forms.
/// </summary>
public class Review
{
    /// <summary>
    /// Unique identifier for the review.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Identifier of the review template used to structure this review.
    /// 
    /// The template defines the schema and expected fields stored
    /// in <see cref="JsonContent"/>.
    /// </summary>
    public Guid ReviewTemplateId { get; set; }
    
    /// <summary>
    /// Navigation property to the review template that defines
    /// the structure of this review.
    /// </summary>
    public ReviewTemplate ReviewTemplate { get; set; } = default!;

    /// <summary>
    /// Identifier of the review assignment linking a reviewer
    /// to a specific paper.
    /// 
    /// Each assignment should have at most one associated review.
    /// </summary>
    public Guid ReviewAssignmentId { get; set; }
    
    /// <summary>
    /// Navigation property to the associated review assignment.
    /// </summary>
    public ReviewAssignment ReviewAssignment { get; set; } = default!;
    
    /// <summary>
    /// The date and time (UTC) when the review was formally submitted.
    /// 
    /// This value remains null while the review is in progress.
    /// </summary>
    public DateTime? SubmittedAtUtc { get; set; }

    /// <summary>
    /// The overall evaluation score assigned by the reviewer.
    /// 
    /// This value is stored separately for efficient querying,
    /// filtering, and statistical analysis.
    /// </summary>
    public int? OverallScore { get; set; }

    /// <summary>
    /// The reviewer’s self-reported confidence in their evaluation.
    /// 
    /// Stored separately to allow statistical analysis and decision support.
    /// </summary>
    public int? ConfidenceScore { get; set; }

    /// <summary>
    /// The reviewer’s final recommendation (e.g., Accept, Reject).
    /// 
    /// This is stored separately to support quick filtering and
    /// aggregation without parsing JSON content.
    /// </summary>
    public string? Recommendation { get; set; }

    /// <summary>
    /// JSON-formatted content containing the full structured review
    /// responses as defined by the associated review template.
    /// 
    /// This allows flexible and versioned review forms without
    /// requiring database schema changes. The JSON structure
    /// should conform to the schema defined in the ReviewTemplate.
    /// </summary>
    public string JsonContent { get; set; } = "{}";
}
