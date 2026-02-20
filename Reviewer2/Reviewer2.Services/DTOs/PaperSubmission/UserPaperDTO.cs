using System;
using System.Collections.Generic;

namespace Reviewer2.Services.DTOs.PaperSubmission
{
    /// <summary>
    /// Represents a paper submission for display in a user's submissions table.
    /// Contains metadata, authors, and associated files for preview or download.
    /// </summary>
    public class UserPaperDTO
    {
        /// <summary>
        /// Gets or sets the unique identifier of the paper.
        /// </summary>
        public Guid PaperId { get; set; }

        /// <summary>
        /// Gets or sets the title of the paper.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current workflow status of the paper
        /// (e.g., Draft, Submitted, Accepted, Rejected).
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the UTC date and time when the paper was submitted.
        /// Null if the paper has not yet been submitted.
        /// </summary>
        public DateTime? SubmittedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time when a final decision was made on the paper.
        /// Null if no decision has been recorded yet.
        /// </summary>
        public DateTime? DecisionMadeAtUtc { get; set; }

        /// <summary>
        /// Gets or sets a comma-separated, formatted string of all authors for display.
        /// Example: "Alice Smith (Corresponding), Bob Jones (Co-Author)".
        /// </summary>
        public string Authors { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of files associated with the paper,
        /// including types available for preview or download (e.g., PDF, CameraReady).
        /// </summary>
        public List<PaperFileSummaryDTO> Files { get; set; } = new();
    }
}