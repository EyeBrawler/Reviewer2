using System;

namespace Reviewer2.Services.DTOs.ConferenceRegistration;

using Data.Models;

/// <summary>
/// Provides extension methods for mapping
/// conference registration entities and DTOs.
/// </summary>
public static class ConferenceRegistrationDTOMapper
{
    /// <summary>
    /// Converts a conference registration entity
    /// to a display DTO.
    /// </summary>
    /// <param name="entity">
    /// The registration entity.
    /// </param>
    /// <returns>
    /// A display DTO containing registration data.
    /// </returns>
    public static ConferenceRegistrationDTO ToDTO(this ConferenceRegistration entity)
    {
        return new ConferenceRegistrationDTO
        {
            Id = entity.Id,
            UserId = entity.UserId,
            FullName = entity.User.FullName,
            Email = entity.User.Email ?? string.Empty,
            RegisteredAtUtc = entity.RegisteredAtUtc,
            Affiliation = entity.Affiliation,
            DietaryRestrictions = entity.DietaryRestrictions,
            AccessibilityNeeds = entity.AccessibilityNeeds,
            AttendingBanquet = entity.AttendingBanquet,
            IsActive = entity.IsActive
        };
    }


    /// <summary>
    /// Converts form data into a new conference
    /// registration entity.
    /// </summary>
    /// <param name="form">
    /// The registration form data.
    /// </param>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    /// <returns>
    /// A new conference registration entity.
    /// </returns>
    public static ConferenceRegistration ToEntity(this ConferenceRegistrationFormDTO form, Guid userId)
    {
        return new ConferenceRegistration
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RegisteredAtUtc = DateTimeOffset.UtcNow,
            Affiliation = form.Affiliation,
            DietaryRestrictions = form.DietaryRestrictions,
            AccessibilityNeeds = form.AccessibilityNeeds,
            AttendingBanquet = form.AttendingBanquet,
            Notes = form.Notes,
            IsActive = true
        };
    }
    
    /// <summary>
    /// Applies form data to an existing
    /// conference registration entity.
    /// Intended for update operations.
    /// </summary>
    /// <param name="form">
    /// The updated form data.
    /// </param>
    /// <param name="entity">
    /// The existing registration entity.
    /// </param>
    public static void ApplyToEntity(this ConferenceRegistrationFormDTO form, ConferenceRegistration entity)
    {
        entity.Affiliation = form.Affiliation;
        entity.DietaryRestrictions = form.DietaryRestrictions;
        entity.AccessibilityNeeds = form.AccessibilityNeeds;
        entity.AttendingBanquet = form.AttendingBanquet;
        entity.Notes = form.Notes;
    }
}