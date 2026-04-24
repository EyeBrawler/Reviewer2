using System;
using System.Collections.Generic;

namespace Reviewer2.Services.DTOs.ReviewTemplates;

/// <summary>
/// DTO representing a review template returned to clients.
/// </summary>
/// <remarks>
/// <para>
/// This DTO provides a structured, UI-friendly representation of a review template,
/// including metadata and the deserialized field definitions.
/// </para>
/// 
/// <para>
/// It is typically used in administrative views and when rendering dynamic review forms.
/// </para>
/// </remarks>
public class ReviewTemplateDTO
{
    /// <summary>
    /// Gets the unique identifier of the review template.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the version number of the template.
    /// </summary>
    /// <remarks>
    /// Each structural change to a template should result in a new version.
    /// </remarks>
    public int Version { get; init; }

    /// <summary>
    /// Gets the human-readable name of the template.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether this template is currently active.
    /// </summary>
    /// <remarks>
    /// Only active templates are used when assigning new reviews.
    /// </remarks>
    public bool IsActive { get; init; }

    /// <summary>
    /// Gets the date and time (UTC) when the template was created.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; init; }

    /// <summary>
    /// Gets the collection of fields that define the structure of the review form.
    /// </summary>
    /// <remarks>
    /// These fields are deserialized from the underlying JSON schema and are used
    /// to dynamically render review forms and validate submissions.
    /// </remarks>
    public List<TemplateFieldDTO> Fields { get; init; } = new();
}