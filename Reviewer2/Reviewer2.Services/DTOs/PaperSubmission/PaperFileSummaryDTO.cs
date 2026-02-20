using System;

namespace Reviewer2.Services.DTOs.PaperSubmission
{
    /// <summary>
    /// Represents a summary of a file associated with a paper submission.
    /// Intended for display in tables or for providing preview/download links.
    /// </summary>
    public class PaperFileSummaryDTO
    {
        /// <summary>
        /// Gets or sets the unique identifier of the file record.
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// Gets or sets the original name of the uploaded file.
        /// This is typically displayed to users in the UI.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of file (e.g., InitialSubmission, CameraReady, CopyrightForm).
        /// </summary>
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets an optional URL for previewing or downloading the file.
        /// This URL may be generated dynamically by the service when returning the DTO.
        /// </summary>
        public string? FileUrl { get; set; }
    }
}