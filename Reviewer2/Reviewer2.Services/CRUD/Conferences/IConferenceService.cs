using System.Threading.Tasks;
using Reviewer2.Services.DTOs.ConferenceManagement;

namespace Reviewer2.Services.CRUD.Conferences
{
    /// <summary>
    /// Defines methods for managing conferences.
    /// </summary>
    public interface IConferenceService
    {
        /// <summary>
        /// Retrieves the latest conference asynchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="ConferenceDTO"/> representing the most recent conference, 
        /// or <c>null</c> if no conferences exist.
        /// </returns>
        Task<ConferenceDTO?> GetConferenceAsync();

        /// <summary>
        /// Updates the first conference with the values from <see cref="ConferenceEditDTO"/>.
        /// Returns true if the update was successful; false otherwise.
        /// </summary>
        Task<bool> UpdateConferenceAsync(ConferenceEditDTO editDto);
    }
}