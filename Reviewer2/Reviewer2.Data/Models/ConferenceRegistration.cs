using System;

namespace Reviewer2.Data.Models;

/// <summary>
/// Represents a user's registration for the conference.
/// Each application user may have at most one registration.
/// </summary>
public class ConferenceRegistration
{
    /// <summary>
    /// Unique registration identifier.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The registered user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Navigation property to the registered user.
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// When the registration was submitted.
    /// </summary>
    public DateTimeOffset RegisteredAtUtc { get; set; }
    
    /// <summary>
    /// Optional institutional affiliation.
    /// Example: University of Arkansas.
    /// </summary>
    public string? Affiliation { get; set; }
    
    /// <summary>
    /// Dietary restrictions or food allergies.
    /// Example: Vegetarian, Gluten-Free, Peanut Allergy.
    /// </summary>
    public string? DietaryRestrictions { get; set; }
    
    /// <summary>
    /// Optional accessibility accommodations.
    /// </summary>
    public string? AccessibilityNeeds { get; set; }
    
    /// <summary>
    /// Whether the attendee plans to attend banquet/social events.
    /// </summary>
    public bool AttendingBanquet { get; set; }
    
    /// <summary>
    /// Optional administrative notes.
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Whether this registration is currently active.
    /// Allows cancellation without deleting history.
    /// </summary>
    public bool IsActive { get; set; } = true;
}