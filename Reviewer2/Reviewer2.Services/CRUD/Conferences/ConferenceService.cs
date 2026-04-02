using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
using Reviewer2.Services.DTOs.ConferenceManagement;

namespace Reviewer2.Services.CRUD.Conferences
{
    /// <summary>
    /// Provides methods to manage conferences using <see cref="ApplicationContext"/>.
    /// </summary>
    public class ConferenceService : IConferenceService
    {
        private readonly IDbContextFactory<ApplicationContext> _contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConferenceService"/> class.
        /// </summary>
        /// <param name="contextFactory">The database context factory.</param>
        public ConferenceService(IDbContextFactory<ApplicationContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Retrieves the latest conference asynchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="ConferenceDTO"/> representing the most recent conference,
        /// or <c>null</c> if no conferences exist.
        /// </returns>
        public async Task<ConferenceDTO?> GetConferenceAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var conf = await context.Conferences
                .Include(c => c.Deadlines)
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            return conf?.ToDTO();
        }

        /// <summary>
        /// Updates the first conference using the provided <see cref="ConferenceEditDTO"/>.
        /// </summary>
        /// <param name="editDto">The DTO containing updated conference information.</param>
        /// <returns>
        /// <c>true</c> if the update was successful; <c>false</c> if no conference exists
        /// or an error occurred.
        /// </returns>
        public async Task<bool> UpdateConferenceAsync(ConferenceEditDTO editDto)
        {
            if (editDto == null)
                return false;

            await using var context = await _contextFactory.CreateDbContextAsync();

            var conference = await context.Conferences
                .Include(c => c.Deadlines)
                .OrderBy(c => c.Id)
                .FirstOrDefaultAsync();

            if (conference == null)
                return false;

            try
            {
                // Map DTO to entity
                editDto.ToEntity(conference);

                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}