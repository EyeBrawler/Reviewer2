using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reviewer2.Services.DTOs.ConferenceRegistration;

namespace Reviewer2.Services.CRUD.ConferenceRegistration;

/// <summary>
/// Provides operations for managing conference
/// registrations.
/// </summary>
public interface IConferenceRegistrationService
{
    /// <summary>
    /// Creates a conference registration for a user.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    /// <param name="form">
    /// The registration form data.
    /// </param>
    /// <returns>
    /// The created registration.
    /// </returns>
    Task<ConferenceRegistrationDTO> CreateAsync(
        Guid userId,
        ConferenceRegistrationFormDTO form);


    /// <summary>
    /// Gets a conference registration by user ID.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    /// <returns>
    /// The registration if found; otherwise null.
    /// </returns>
    Task<ConferenceRegistrationDTO?> GetByUserIdAsync(
        Guid userId);


    /// <summary>
    /// Gets a conference registration by
    /// registration ID.
    /// </summary>
    /// <param name="registrationId">
    /// The unique registration identifier.
    /// </param>
    /// <returns>
    /// The registration if found; otherwise null.
    /// </returns>
    Task<ConferenceRegistrationDTO?> GetByIdAsync(
        Guid registrationId);


    /// <summary>
    /// Gets all conference registrations.
    /// Intended for administrative use.
    /// </summary>
    /// <returns>
    /// A collection of conference registrations.
    /// </returns>
    Task<List<ConferenceRegistrationDTO>> GetAllAsync();


    /// <summary>
    /// Updates an existing registration.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    /// <param name="form">
    /// The updated registration form data.
    /// </param>
    Task UpdateAsync(
        Guid userId,
        ConferenceRegistrationFormDTO form);


    /// <summary>
    /// Cancels a user's registration without
    /// deleting it by marking it inactive.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    Task CancelAsync(
        Guid userId);


    /// <summary>
    /// Reactivates a canceled registration.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    Task ReactivateAsync(
        Guid userId);


    /// <summary>
    /// Determines whether a user has a
    /// conference registration.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    /// <returns>
    /// True if the user is registered;
    /// otherwise false.
    /// </returns>
    Task<bool> ExistsAsync(
        Guid userId);
}