using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
using Reviewer2.Data.Models;
using System.Threading.Tasks;
using Reviewer2.Services.DTOs.ConferenceManagement;

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
        /// Retrieves the first conference as a read-only DTO, including its deadlines.
        /// </summary>
        /// <returns>
        /// A <see cref="ConferenceDTO"/> representing the conference, or <c>null</c> if no conference exists.
        /// </returns>
        public async Task<ConferenceDTO?> GetConferenceAsync()
        {
            await using var context = _dbFactory.CreateDbContext();
            var entity = await context.Conferences
                .Include(c => c.Deadlines)
                .FirstOrDefaultAsync();

            return entity?.ToDTO();
        }

        /// <summary>
        /// Retrieves the first conference entity for editing, including its deadlines.
        /// </summary>
        /// <returns>
        /// A <see cref="Conference"/> entity, or <c>null</c> if no conference exists.
        /// </returns>
        public async Task<Conference?> GetConferenceEntityAsync()
        {
            await using var context = _dbFactory.CreateDbContext();
            return await context.Conferences
                .Include(c => c.Deadlines)
                .FirstOrDefaultAsync();
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
            await using var context = _dbFactory.CreateDbContext();
            var entity = await context.Conferences
                .Include(c => c.Deadlines)
                .FirstOrDefaultAsync();

            if (entity == null) return false;

            // Map DTO values to the entity
            editDto.ToEntity(entity);

            // Save changes to the database
            await context.SaveChangesAsync();
            return true;
        }
    }
}