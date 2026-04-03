using System;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
using Reviewer2.Data.Models;
using System.Threading.Tasks;
using Reviewer2.Services.DTOs.ConferenceManagement;
using Serilog;

namespace Reviewer2.Services.CRUD.Conferences
{
    /// <summary>
    /// Provides CRUD operations for conferences, including retrieval and updating
    /// conference details and associated deadlines.
    /// </summary>
    public class ConferenceService : IConferenceService
    {
        private readonly IDbContextFactory<ApplicationContext> _dbFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConferenceService"/> class.
        /// </summary>
        /// <param name="dbFactory">The database context factory used to create <see cref="ApplicationContext"/> instances.</param>
        public ConferenceService(IDbContextFactory<ApplicationContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        /// <summary>
        /// Retrieves the conference as a read-only DTO, including its deadlines.
        /// </summary>
        /// <returns>
        /// A <see cref="ConferenceDTO"/> representing the conference.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no conference exists in the database.
        /// </exception>
        public async Task<ConferenceDTO> GetConferenceAsync()
        {
            Log.Information("Fetching conference DTO");

            try
            {
                await using var context = await _dbFactory.CreateDbContextAsync();

                var entity = await context.Conferences
                    .Include(c => c.Deadlines)
                    .SingleAsync();

                Log.Information(
                    "Conference retrieved successfully (Id: {ConferenceId}, Deadlines: {DeadlineCount})",
                    entity.Id,
                    entity.Deadlines?.Count ?? 0
                );

                return entity.ToDTO();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving conference DTO");
                throw;
            }
        }

        /// <summary>
        /// Retrieves the first conference entity for editing, including its deadlines.
        /// </summary>
        /// <returns>
        /// A <see cref="Conference"/> entity, or <c>null</c> if no conference exists.
        /// </returns>
        public async Task<Conference?> GetConferenceEntityAsync()
        {
            Log.Information("Fetching conference entity");

            try
            {
                await using var context = await _dbFactory.CreateDbContextAsync();

                var entity = await context.Conferences
                    .Include(c => c.Deadlines)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    Log.Warning("No conference entity found");
                }
                else
                {
                    Log.Information("Conference entity retrieved (Id: {ConferenceId}, Deadlines: {DeadlineCount})",
                        entity.Id,
                        entity.Deadlines?.Count ?? 0);
                }

                return entity;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving conference entity");
                throw;
            }
        }

        /// <summary>
        /// Updates the first conference entity in the database using values from a <see cref="ConferenceEditDTO"/>.
        /// </summary>
        /// <param name="editDto">The DTO containing updated conference information.</param>
        /// <returns>
        /// <c>true</c> if the update was successful; <c>false</c> if no conference entity was found.
        /// </returns>
        public async Task<bool> UpdateConferenceAsync(ConferenceEditDTO editDto)
        {
            Log.Information("Starting conference update");

            try
            {
                await using var context = await _dbFactory.CreateDbContextAsync();

                var entity = await context.Conferences
                    .Include(c => c.Deadlines)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    Log.Warning("Update failed: no conference entity found");
                    return false;
                }

                Log.Information("Mapping DTO to entity (ConferenceId: {ConferenceId})", entity.Id);

                // Optional: log incoming data snapshot (be careful with sensitive data)
                Log.Debug("Incoming DTO: {@EditDto}", editDto);

                // Map DTO values to the entity
                editDto.ToEntity(entity);

                Log.Information("Saving changes to database");

                var changes = await context.SaveChangesAsync();

                Log.Information("Conference update complete (ConferenceId: {ConferenceId}, Changes: {ChangeCount})",
                    entity.Id,
                    changes);

                return true;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "Database update error while saving conference");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while updating conference");
                throw;
            }
        }
    }
}