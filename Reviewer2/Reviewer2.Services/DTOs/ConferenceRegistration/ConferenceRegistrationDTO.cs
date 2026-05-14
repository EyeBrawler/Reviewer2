using System;

namespace Reviewer2.Services.DTOs.ConferenceRegistration;

/// <summary>
/// Represents conference registration information
/// for display purposes.
/// Intended for attendee lists, administrative views,
/// and export operations.
/// </summary>
public class ConferenceRegistrationDTO
{
    /// <summary>
    /// Gets or sets the unique registration identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user
    /// who submitted the registration.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the full name of the registrant.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the registrant.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the registration was submitted.
    /// </summary>
    public DateTimeOffset RegisteredAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the registrant's institutional
    /// affiliation.
    /// </summary>
    public string? Affiliation { get; set; }

    /// <summary>
    /// Gets or sets dietary restrictions or allergies.
    /// </summary>
    public string? DietaryRestrictions { get; set; }

    /// <summary>
    /// Gets or sets accessibility accommodations
    /// requested by the registrant.
    /// </summary>
    public string? AccessibilityNeeds { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// registrant plans to attend banquet or social
    /// events.
    /// </summary>
    public bool AttendingBanquet { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// registration is currently active.
    /// </summary>
    public bool IsActive { get; set; }
}