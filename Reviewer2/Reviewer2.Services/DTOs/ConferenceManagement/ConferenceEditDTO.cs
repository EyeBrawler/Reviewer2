using System;
using System.Collections.Generic;

namespace Reviewer2.Services.DTOs.ConferenceManagement
{
    /// <summary>
    /// Data Transfer Object for editing a conference, including basic info,
    /// call for papers content, and associated deadlines.
    /// </summary>
    public class ConferenceEditDTO
    {
        /// <summary>
        /// Gets or sets the basic information of the conference (name and description).
        /// </summary>
        public ConferenceInfo Info { get; set; } = new ConferenceInfo();

        /// <summary>
        /// Gets or sets the call for papers information.
        /// </summary>
        public CallForPapersInfo CallForPapers { get; set; } = new CallForPapersInfo();

        /// <summary>
        /// Gets or sets the collection of deadlines associated with the conference.
        /// </summary>
        public List<DeadlineInfo> Deadlines { get; set; } = new List<DeadlineInfo>();

        /// <summary>
        /// Represents the basic information of a conference.
        /// </summary>
        public class ConferenceInfo
        {
            /// <summary>
            /// Gets or sets the name of the conference.
            /// </summary>
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets a description of the conference.
            /// </summary>
            public string Description { get; set; } = string.Empty;
        }

        /// <summary>
        /// Represents the call for papers content of a conference.
        /// </summary>
        public class CallForPapersInfo
        {
            /// <summary>
            /// Gets or sets the content of the call for papers.
            /// </summary>
            public string Content { get; set; } = string.Empty;
        }

        /// <summary>
        /// Represents a single deadline for a conference.
        /// </summary>
        public class DeadlineInfo
        {
            /// <summary>
            /// Gets or sets the unique identifier for the deadline.
            /// Nullable for new deadlines that have not been saved yet.
            /// </summary>
            public int? Id { get; set; }

            /// <summary>
            /// Gets or sets the name of the deadline (e.g., "Submission Deadline").
            /// </summary>
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// Nullable DateTime to work with MudDatePicker.
            /// </summary>
            public DateTime? Date { get; set; } = DateTime.Now;

            /// <summary>
            /// Gets or sets the priority of the deadline for ordering purposes.
            /// </summary>
            public int Priority { get; set; } = 0;
        }
    }
}