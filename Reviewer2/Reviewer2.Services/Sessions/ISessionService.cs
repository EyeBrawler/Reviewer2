using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Provides operations for managing session scheduling.
/// 
/// This service is responsible for persisting drag-and-drop scheduling state
/// and retrieving sessions with their associated papers for UI rendering.
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Persists a full set of session scheduling data.
    /// 
    /// This method treats the provided list as the complete source of truth,
    /// updating existing sessions, creating new ones, and removing deleted sessions.
    /// </summary>
    /// <param name="sessions">The full list of sessions from the UI.</param>
    Task SaveSessionsAsync(List<SessionDTO> sessions);

    /// <summary>
    /// Retrieves all sessions with their associated papers for UI display.
    /// </summary>
    /// <returns>
    /// A list of sessions with fully populated paper data.
    /// </returns>
    Task<List<SessionWithPapersDTO>> GetSessionsAsync();
}