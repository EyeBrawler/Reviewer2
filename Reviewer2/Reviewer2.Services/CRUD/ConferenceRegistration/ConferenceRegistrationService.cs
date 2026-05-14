using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
using Reviewer2.Services.DTOs.ConferenceRegistration;
using Serilog;

namespace Reviewer2.Services.CRUD.ConferenceRegistration;

using Data.Models;

/// <inheritdoc />
public class ConferenceRegistrationService : IConferenceRegistrationService
{
    private readonly IDbContextFactory<ApplicationContext> _contextFactory;
    private readonly UserManager<ApplicationUser> _userManager;
    
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ConferenceRegistrationService"/>
    /// class.
    /// </summary>
    /// <param name="contextFactory">
    /// Factory used to create database contexts.
    /// </param>
    /// <param name="userManager">
    /// Provides access to application users.
    /// </param>
    public ConferenceRegistrationService(
        IDbContextFactory<ApplicationContext> contextFactory,
        UserManager<ApplicationUser> userManager)
    {
        _contextFactory = contextFactory;
        _userManager = userManager;
    }
    
    /// <inheritdoc />
    public async Task<ConferenceRegistrationDTO> CreateAsync(Guid userId, ConferenceRegistrationFormDTO form)
    {
        Log.Information("Creating conference registration for user {UserId}.", userId);

        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            Log.Warning("Conference registration failed. " + "User {UserId} was not found.", userId);

            throw new InvalidOperationException(
                "The specified user does not exist.");
        }
        
        var alreadyRegistered = await context.ConferenceRegistrations
            .AnyAsync(r => r.UserId == userId);

        if (alreadyRegistered)
        {
            Log.Warning(
                "Conference registration failed. " +
                "User {UserId} is already registered.",
                userId);

            throw new InvalidOperationException("The user is already registered.");
        }
        
        var registration = form.ToEntity(userId);

        context.ConferenceRegistrations.Add(registration);

        await context.SaveChangesAsync();
        
        // Needed for ToDTO().
        registration.User = user;
        
        Log.Information("Conference registration {RegistrationId} " + "created successfully for user {UserId}.",
            registration.Id,
            userId);

        return registration.ToDTO();
    }

    /// <inheritdoc />
    public async Task<ConferenceRegistrationDTO?> GetByUserIdAsync(Guid userId)
    {
        Log.Information("Retrieving conference registration for user {UserId}.", userId);

        await using var context = await _contextFactory.CreateDbContextAsync();

        var registration = await context.ConferenceRegistrations
                .Include(r => r.User)
                .FirstOrDefaultAsync(
                    r => r.UserId == userId);
        
        if (registration is null)
        {
            Log.Information("No conference registration found for user {UserId}.", userId);
            return null;
        }
        
        Log.Information("Conference registration {RegistrationId} " + "retrieved for user {UserId}.", 
            registration.Id,
            userId);

        return registration.ToDTO();
    }

    /// <inheritdoc />
    public async Task<ConferenceRegistrationDTO?> GetByIdAsync(Guid registrationId)
    {
        Log.Information("Retrieving conference registration {RegistrationId}.", registrationId);

        await using var context = await _contextFactory.CreateDbContextAsync();

        var registration =
            await context.ConferenceRegistrations
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == registrationId);

        if (registration is null)
        {
            Log.Information("No conference registration found with ID {RegistrationId}.", registrationId);
            return null;
        }

        Log.Information("Conference registration {RegistrationId} successfully retrieved for user {UserId}.",
            registration.Id,
            registration.UserId);

        return registration.ToDTO();
    }

    /// <inheritdoc />
    public async Task<List<ConferenceRegistrationDTO>> GetAllAsync()
    {
        Log.Information("Retrieving all conference registrations.");

        await using var context = await _contextFactory.CreateDbContextAsync();

        var registrations =
            await context.ConferenceRegistrations
                .Include(r => r.User)
                .OrderByDescending(r => r.RegisteredAtUtc)
                .ToListAsync();

        Log.Information(
            "Retrieved {Count} conference registrations.",
            registrations.Count);

        return registrations
            .Select(r => r.ToDTO())
            .ToList();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Guid userId, ConferenceRegistrationFormDTO form)
    {
        Log.Information("Updating conference registration for user {UserId}.", userId);

        await using var context = await _contextFactory.CreateDbContextAsync();

        var registration =
            await context.ConferenceRegistrations
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId);

        if (registration is null)
        {
            Log.Warning("Update failed. No conference registration found for user {UserId}.", userId);

            throw new InvalidOperationException(
                "Conference registration not found for user.");
        }

        form.ApplyToEntity(registration);

        await context.SaveChangesAsync();

        Log.Information(
            "Conference registration updated successfully for user {UserId} (Registration {RegistrationId}).",
            userId,
            registration.Id);
    }

    /// <inheritdoc />
    public async Task CancelAsync(Guid userId)
    {
        Log.Information("Cancelling conference registration for user {UserId}.", userId);

        await using var context = await _contextFactory.CreateDbContextAsync();

        var registration =
            await context.ConferenceRegistrations
                .FirstOrDefaultAsync(r => r.UserId == userId);

        if (registration is null)
        {
            Log.Warning("Cancel failed. No conference registration found for user {UserId}.", userId);

            throw new InvalidOperationException(
                "Conference registration not found for user.");
        }

        if (!registration.IsActive)
        {
            Log.Information("Conference registration for user {UserId} is already cancelled.", userId);
            return;
        }

        registration.IsActive = false;

        await context.SaveChangesAsync();

        Log.Information(
            "Conference registration cancelled for user {UserId} (Registration {RegistrationId}).",
            userId,
            registration.Id);
    }

    /// <inheritdoc />
    public async Task ReactivateAsync(Guid userId)
    {
        Log.Information("Reactivating conference registration for user {UserId}.", userId);

        await using var context = await _contextFactory.CreateDbContextAsync();

        var registration =
            await context.ConferenceRegistrations
                .FirstOrDefaultAsync(r => r.UserId == userId);

        if (registration is null)
        {
            Log.Warning("Reactivation failed. No conference registration found for user {UserId}.", userId);

            throw new InvalidOperationException(
                "Conference registration not found for user.");
        }

        if (registration.IsActive)
        {
            Log.Information("Conference registration for user {UserId} is already active.", userId);
            return;
        }

        registration.IsActive = true;

        await context.SaveChangesAsync();

        Log.Information(
            "Conference registration reactivated for user {UserId} (Registration {RegistrationId}).",
            userId,
            registration.Id);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Guid userId)
    {
        Log.Information("Checking if conference registration exists for user {UserId}.", userId);

        await using var context = await _contextFactory.CreateDbContextAsync();

        var exists = await context.ConferenceRegistrations
                .AnyAsync(r => r.UserId == userId);

        Log.Information("Conference registration existence check for user {UserId}: {Exists}.", userId, exists);

        return exists;
    }
}