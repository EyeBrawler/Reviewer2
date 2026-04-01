using System;

namespace Reviewer2.Services.DTOs.ConferenceManagement
{
    /// <summary>
    /// Represents a single deadline for a conference.
    /// </summary>
    public class DeadlineSummary
    {
        /// <summary>
        /// The deadline name (e.g., submission, review).
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The deadline date in UTC.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The relative priority of this deadline.
        /// </summary>
        public int Priority { get; set; }
    }
}