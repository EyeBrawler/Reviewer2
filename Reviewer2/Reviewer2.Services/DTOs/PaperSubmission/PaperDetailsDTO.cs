using System;
using System.Collections.Generic;

namespace Reviewer2.Services.DTOs.PaperSubmission
{
    /// <summary>
    /// Represents a detailed view of a paper submission, including metadata,
    /// authors, and associated files.
    /// Intended for displaying full paper details or previewing files in a UI.
    /// </summary>
    public class PaperDetailsDTO
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
        /// Gets or sets the abstract text summarizing the content of the paper.
        /// </summary>
        public string Abstract { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current workflow status of the paper (e.g., Draft, Submitted, Accepted).
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the UTC date and time when the paper was submitted.
        /// Null if the paper has not yet been submitted.
        /// </summary>
        public DateTime? SubmittedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time when a final decision was made for the paper.
        /// Null if no decision has been recorded.
        /// </summary>
        public DateTime? DecisionMadeAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the name of the user who submitted the paper.
        /// </summary>
        public string SubmitterName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a list of authors for the paper.
        /// Each entry is formatted as "Name (Role)" for display purposes.
        /// </summary>
        public List<string> Authors { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of files associated with the paper.
        /// Includes file type, name, and optional URL for preview or download.
        /// </summary>
        public List<PaperFileSummaryDTO> Files { get; set; } = new();
    }
}