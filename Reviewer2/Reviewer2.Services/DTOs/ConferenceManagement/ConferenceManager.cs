using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
using Reviewer2.Data.Models;

namespace Reviewer2.Services.DTOs.ConferenceManagement
{
    /// <summary>
    /// Service to manage conference queries using <see cref="ApplicationContext"/> via a DbContext factory.
    /// </summary>
    public class ConferenceManager
    {
        private readonly IDbContextFactory<ApplicationContext> _contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConferenceManager"/> class.
        /// </summary>
        public ConferenceManager(IDbContextFactory<ApplicationContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Retrieves the latest conference and maps it to a <see cref="ConferenceSummary"/> asynchronously.
        /// Returns null if no conferences exist.
        /// </summary>
        public async Task<ConferenceSummary?> GetActiveConferenceAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var conf = await context.Conferences
                .Include(c => c.Deadlines)
                .OrderByDescending(c => c.Id) // latest conference
                .FirstOrDefaultAsync();

            return conf?.ToSummary();
        }

        /// <summary>
        /// Synchronous wrapper for <see cref="GetActiveConferenceAsync"/>.
        /// </summary>
        public ConferenceSummary? GetActiveConference()
        {
            return GetActiveConferenceAsync().GetAwaiter().GetResult();
        }
    }
}