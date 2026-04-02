using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
using Reviewer2.Services.DTOs.ConferenceManagement;

namespace Reviewer2.Services.CRUD.Conferences
{
    /// <summary>
    /// Service to manage conference queries using <see cref="ApplicationContext"/> via a DbContext factory.
    /// </summary>
    public class ConferenceService : IConferenceService
    {
        private readonly IDbContextFactory<ApplicationContext> _contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConferenceService"/> class.
        /// </summary>
        public ConferenceService(IDbContextFactory<ApplicationContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Retrieves the latest conference and maps it to a <see cref="ConferenceDTO"/> asynchronously.
        /// Returns null if no conferences exist.
        /// </summary>
        public async Task<ConferenceDTO?> GetConferenceAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var conf = await context.Conferences
                .Include(c => c.Deadlines)
                .OrderByDescending(c => c.Id) // latest conference
                .FirstOrDefaultAsync();

            return conf?.ToDTO();
        }
    }
}