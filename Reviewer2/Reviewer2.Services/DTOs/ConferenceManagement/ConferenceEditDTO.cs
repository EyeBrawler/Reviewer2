using System;
using System.Collections.Generic;

namespace Reviewer2.Services.DTOs.ConferenceManagement
{
    /// <summary>
    /// Data transfer object used for editing conference details.
    /// </summary>
    public class ConferenceEditDTO
    {
        /// <summary>
        /// Gets or sets the basic information about the conference.
        /// </summary>
        public ConferenceInfo Info { get; set; } = new();

        /// <summary>
        /// Gets or sets the call for papers content.
        /// </summary>
        public CallForPapersInfo CallForPapers { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of deadlines associated with the conference.
        /// </summary>
        public List<DeadlineInfo> Deadlines { get; set; } = new();

        /// <summary>
        /// Represents general conference information.
        /// </summary>
        public class ConferenceInfo
        {
            /// <summary>
            /// Gets or sets the name of the conference.
            /// </summary>
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the description of the conference.
            /// </summary>
            public string Description { get; set; } = string.Empty;
        }

        /// <summary>
        /// Represents call for papers content.
        /// </summary>
        public class CallForPapersInfo
        {
            /// <summary>
            /// Gets or sets the call for papers text.
            /// </summary>
            public string Content { get; set; } = string.Empty;
        }

        /// <summary>
        /// Represents a deadline for the conference.
        /// </summary>
        public class DeadlineInfo
        {
            /// <summary>
            /// Gets or sets the name of the deadline.
            /// </summary>
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the date of the deadline.
            /// </summary>
            public DateTime Date { get; set; }

            /// <summary>
            /// Gets or sets the priority of the deadline (higher means more important).
            /// </summary>
            public int Priority { get; set; }
        }
    }
}