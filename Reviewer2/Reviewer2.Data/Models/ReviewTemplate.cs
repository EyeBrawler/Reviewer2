using System;

namespace Reviewer2.Data.Models;

/// <summary>
/// Defines the structure and schema used for reviewer evaluations.
/// 
/// A ReviewTemplate specifies the questions, scoring fields, and
/// validation rules for reviews. The template's schema is stored
/// as JSON and is used to validate and render dynamic review forms.
/// 
/// Templates are versioned to ensure that previously submitted reviews
/// remain consistent with the schema that was active at the time of submission.
/// </summary>
public class ReviewTemplate
{
    /// <summary>
    /// Unique identifier for the review template.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Version number of the template.
    /// 
    /// When structural changes are made (e.g., adding or modifying questions),
    /// a new version should be created rather than modifying an existing one.
    /// This preserves historical consistency for submitted reviews.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Human-readable name of the template.
    /// 
    /// This may indicate the review type (e.g., "Full Paper Review",
    /// "Short Paper Review") or the version label.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// JSON schema defining the structure of reviews created using this template.
    /// 
    /// The schema describes required fields, scoring ranges, text sections,
    /// and any validation constraints. Review.JsonContent must conform to
    /// this schema.
    /// </summary>
    public string JsonSchema { get; set; } = "{}";

    /// <summary>
    /// Indicates whether this template is currently active and available
    /// for new review assignments.
    /// 
    /// Only one template should typically be active at a time for a given
    /// review type, although multiple active templates may be supported
    /// if different submission tracks exist.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// The date and time (UTC) when the template was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }
}