using System;
using System.Collections.Generic;

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
    /// Gets or sets the collection of dynamic review field values associated with this review.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This dictionary contains the structured responses to fields defined by the associated
    /// <see cref="Reviewer2.Data.Models.ReviewTemplate"/>. Each entry maps a template-defined
    /// field key to a corresponding <see cref="ReviewValue"/> instance representing the
    /// reviewer’s input.
    /// </para>
    ///
    /// <para>
    /// The set of valid keys and the expected data types for each value are determined by the
    /// template referenced by <see cref="ReviewTemplateId"/>. The application is responsible
    /// for ensuring that:
    /// <list type="bullet">
    /// <item>
    /// <description>All required template fields are present.</description>
    /// </item>
    /// <item>
    /// <description>Each value conforms to the type and validation rules defined in the template.</description>
    /// </item>
    /// <item>
    /// <description>Only one underlying value is set per <see cref="ReviewValue"/> instance.</description>
    /// </item>
    /// </list>
    /// </para>
    ///
    /// <para>
    /// This property enables flexible, schema-driven review forms without requiring database
    /// schema changes, while core review metrics (e.g., <see cref="OverallScore"/>,
    /// <see cref="ConfidenceScore"/>, <see cref="Recommendation"/>) remain strongly typed
    /// for efficient querying and analysis.
    /// </para>
    /// </remarks>
    public Dictionary<string, ReviewValue> Values { get; set; } = new();
}