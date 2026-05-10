using System;
using System.Collections.Generic;

namespace Reviewer2.Services.DTOs.ReviewTemplates;

/// <summary>
/// Represents a single field in a review template.
/// </summary>
/// <remarks>
/// <para>
/// A template field defines one input element in the review form, including its
/// type, validation rules, and display metadata.
/// </para>
/// 
/// <para>
/// The interpretation of each property depends on the <see cref="Type"/> of the field.
/// For example, numeric fields may use <see cref="Min"/> and <see cref="Max"/>, while
/// selection fields may use <see cref="Options"/>.
/// </para>
/// </remarks>
public class TemplateFieldDTO
{
    /// <summary>
    /// Gets or sets the unique key identifying this field within the template.
    /// </summary>
    /// <remarks>
    /// This key is used to map submitted review values to the corresponding field.
    /// It should remain stable across template versions if the field is preserved.
    /// </remarks>
    public string Key { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the human-readable label displayed to reviewers.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the field.
    /// </summary>
    /// <remarks>
    /// Determines how the field is rendered in the UI and how its value is validated.
    /// </remarks>
    public ReviewFieldType Type { get; set; } = ReviewFieldType.Text;

    /// <summary>
    /// Gets or sets a value indicating whether this field is required.
    /// </summary>
    /// <remarks>
    /// Required fields must have a value when a review is submitted.
    /// </remarks>
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets the minimum allowed value for numeric fields.
    /// </summary>
    /// <remarks>
    /// This property is only applicable when <see cref="Type"/> is "Number".
    /// </remarks>
    public int? Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed value for numeric fields.
    /// </summary>
    /// <remarks>
    /// This property is only applicable when <see cref="Type"/> is "Number".
    /// </remarks>
    public int? Max { get; set; }

    /// <summary>
    /// Gets or sets the list of selectable options for selection-based fields.
    /// </summary>
    /// <remarks>
    /// This property is only applicable when <see cref="Type"/> is "Select".
    /// Each entry represents a valid selectable value.
    /// </remarks>
    public List<string>? Options { get; set; }
}