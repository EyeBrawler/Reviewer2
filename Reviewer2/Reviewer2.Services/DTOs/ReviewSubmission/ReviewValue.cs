using System.Globalization;
using Reviewer2.Services.DTOs.ReviewTemplates;

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
    /// Gets or sets the numeric value of the field if the template defines the field as a number.
    /// </summary>
    /// <remarks>
    /// Typically used for scoring fields (e.g., "Overall Score", "Novelty", "Confidence").
    /// The valid range and constraints should be enforced based on the associated template definition.
    /// </remarks>
    public decimal? NumberValue { get; set; }

    /// <summary>
    /// Gets or sets the textual value of the field if the template defines the field as text.
    /// </summary>
    /// <remarks>
    /// Used for free-form comments, explanations, or any string-based input.
    /// Length limits and formatting rules should be enforced via template validation.
    /// </remarks>
    public string? StringValue { get; set; }

    /// <summary>
    /// Gets or sets the boolean value of the field if the template defines the field as a boolean.
    /// </summary>
    /// <remarks>
    /// Typically used for yes/no questions or binary flags within the review form.
    /// </remarks>
    public bool BoolValue { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance contains no assigned value.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if all value properties are <see langword="null"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsEmpty()
    {
        return NumberValue is null
               && StringValue is null;
    }

    
    /// <summary>
    /// Determines whether a <see cref="ReviewValue"/> contains data that does not
    /// match the expected field type defined by the review template.
    /// </summary>
    /// <param name="value">
    /// The submitted value to inspect.
    /// </param>
    /// <param name="expectedType">
    /// The field type defined by the associated review template.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the value contains data in properties that are
    /// incompatible with the expected field type; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// A <see cref="ReviewValue"/> is designed to hold only one logical value at a time,
    /// but because it contains multiple storage properties, invalid states may occur
    /// where data is populated in properties that do not correspond to the template's
    /// expected field type.
    /// </para>
    ///
    /// <para>
    /// This method validates the structural consistency of the value by checking for
    /// incompatible data. For example, a field defined as
    /// <c><see cref="ReviewFieldType.Boolean"/></c> should not contain numeric or
    /// textual data.
    /// </para>
    ///
    /// <para>
    /// This method validates shape only and does not enforce business rules such as
    /// required values, numeric ranges, or string length constraints.
    /// </para>
    /// </remarks>
    private static bool HasWrongShape(
        ReviewValue value,
        ReviewFieldType expectedType)
    {
        return expectedType switch
        {
            ReviewFieldType.Text =>
                value.NumberValue is not null,

            ReviewFieldType.Number =>
                !string.IsNullOrWhiteSpace(value.StringValue),

            ReviewFieldType.Boolean =>
                value.NumberValue is not null ||
                !string.IsNullOrWhiteSpace(value.StringValue),

            _ => false
        };
    }

    /// <summary>
    /// Returns a string representation of the stored value for display purposes.
    /// </summary>
    /// <returns>
    /// A string representation of the underlying value, or an empty string if no value is set.
    /// </returns>
    public override string ToString()
    {
        if (NumberValue is not null)
            return NumberValue.Value.ToString(CultureInfo.CurrentCulture);

        return !string.IsNullOrWhiteSpace(StringValue) ? StringValue : BoolValue.ToString();
    }
    
    /// <summary>
    /// Method to clear the set review value.
    /// </summary>
    public void Clear()
    {
        NumberValue = null;
        StringValue = null;
        BoolValue = false;
    }
}
