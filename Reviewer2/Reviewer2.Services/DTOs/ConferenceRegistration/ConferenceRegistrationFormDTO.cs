using System.ComponentModel.DataAnnotations;

namespace Reviewer2.Services.DTOs.ConferenceRegistration;

/// <summary>
/// Represents user-editable conference registration
/// data submitted through a Blazor form.
/// Intended for creating or updating a user's
/// conference registration.
/// </summary>
public class ConferenceRegistrationFormDTO
{
    /// <summary>
    /// Gets or sets the registrant's institutional
    /// affiliation.
    /// </summary>
    [StringLength(200)]
    public string? Affiliation { get; set; }

    /// <summary>
    /// Gets or sets dietary restrictions or food
    /// allergies.
    /// Example: Vegetarian, Gluten-Free,
    /// Peanut Allergy.
    /// </summary>
    [StringLength(500)]
    public string? DietaryRestrictions { get; set; }

    /// <summary>
    /// Gets or sets requested accessibility
    /// accommodations.
    /// </summary>
    [StringLength(500)]
    public string? AccessibilityNeeds { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// registrant plans to attend banquet or
    /// social events.
    /// </summary>
    public bool AttendingBanquet { get; set; }

    /// <summary>
    /// Gets or sets optional additional notes
    /// provided by the registrant.
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }
}