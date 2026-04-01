using System.Collections.Generic;

namespace Reviewer2.Services.DTOs.ConferenceManagement
{
    /// <summary>
    /// Represents a conference for presentation to the client or UI.
    /// </summary>
    public class ConferenceSummary
    {
        /// <summary>
        /// The conference name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The conference description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The call for papers text.
        /// </summary>
        public string? CallForPapers { get; set; }

        /// <summary>
        /// The deadlines associated with the conference, ordered descending by priority.
        /// </summary>
        public List<DeadlineSummary> Deadlines { get; set; } = new();
    }
}