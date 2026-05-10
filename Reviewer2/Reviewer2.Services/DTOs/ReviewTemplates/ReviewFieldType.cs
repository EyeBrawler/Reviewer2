namespace Reviewer2.Services.DTOs.ReviewTemplates;

/// <summary>
/// Represents the supported field types for review template fields.
/// </summary>
public enum ReviewFieldType
{
    /// <summary>
    /// A free-form text input.
    /// </summary>
    Text,

    /// <summary>
    /// A numeric input (integer or decimal).
    /// </summary>
    Number,

    /// <summary>
    /// A boolean (true/false) input.
    /// </summary>
    Boolean,

    /// <summary>
    /// A selection from a predefined list of options.
    /// </summary>
    Select
}