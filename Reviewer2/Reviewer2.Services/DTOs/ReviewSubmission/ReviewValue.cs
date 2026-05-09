namespace Reviewer2.Services.DTOs.ReviewSubmission;


/// <summary>
/// Represents a single value submitted as part of a dynamic review form.
/// </summary>
/// <remarks>
/// <para>
/// A <see cref="ReviewValue"/> is a lightweight, schema-aligned container used to store
/// user input for a field defined in a <see cref="Reviewer2.Data.Models.ReviewTemplate"/>.
/// The expected data type of the value is determined by the associated template field,
/// not by this class itself.
/// </para>
///
/// <para>
/// Only one of the value properties (<see cref="NumberValue"/>, <see cref="StringValue"/>,
/// or <see cref="BoolValue"/>) should be set for a given instance. The application layer
/// is responsible for enforcing this constraint during validation.
/// </para>
///
/// <para>
/// This design allows flexible, schema-driven review forms while maintaining a predictable
/// and strongly structured representation in application code.
/// </para>
/// </remarks>
public class ReviewValue
{
    /// <summary>
    /// Gets or sets the numeric value of the field, if the template defines the field as a number.
    /// </summary>
    /// <remarks>
    /// Typically used for scoring fields (e.g., "Overall Score", "Novelty", "Confidence").
    /// The valid range and constraints should be enforced based on the associated template definition.
    /// </remarks>
    public decimal? NumberValue { get; set; }

    /// <summary>
    /// Gets or sets the textual value of the field, if the template defines the field as text.
    /// </summary>
    /// <remarks>
    /// Used for free-form comments, explanations, or any string-based input.
    /// Length limits and formatting rules should be enforced via template validation.
    /// </remarks>
    public string? StringValue { get; set; }

    /// <summary>
    /// Gets or sets the boolean value of the field, if the template defines the field as a boolean.
    /// </summary>
    /// <remarks>
    /// Typically used for yes/no questions or binary flags within the review form.
    /// </remarks>
    public bool? BoolValue { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance contains no assigned value.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if all value properties are <see langword="null"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsEmpty()
    {
        return NumberValue is null
               && StringValue is null
               && BoolValue is null;
    }

    /// <summary>
    /// Gets a value indicating whether this instance contains more than one assigned value.
    /// </summary>
    /// <remarks>
    /// A valid <see cref="ReviewValue"/> should only have one value set. This method can be used
    /// during validation to detect invalid or conflicting input states.
    /// </remarks>
    /// <returns>
    /// <see langword="true"/> if more than one value property is set; otherwise, <see langword="false"/>.
    /// </returns>
    public bool HasMultipleValues()
    {
        var count = 0;

        if (NumberValue is not null) count++;
        if (StringValue is not null) count++;
        if (BoolValue is not null) count++;

        return count > 1;
    }

    /// <summary>
    /// Returns a string representation of the stored value for display purposes.
    /// </summary>
    /// <returns>
    /// A string representation of the underlying value, or an empty string if no value is set.
    /// </returns>
    public override string ToString()
    {
        return NumberValue?.ToString()
               ?? StringValue
               ?? BoolValue?.ToString()
               ?? string.Empty;
    }
}
