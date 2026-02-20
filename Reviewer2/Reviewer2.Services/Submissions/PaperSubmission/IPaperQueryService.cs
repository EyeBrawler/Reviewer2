using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Reviewer2.Data.Models;
using Reviewer2.Services.DTOs.FileStorage;
using Reviewer2.Services.DTOs.PaperSubmission;

namespace Reviewer2.Services.Submissions.PaperSubmission
{
    /// <summary>
    /// Provides read-only access to paper submissions and associated files.
    /// </summary>
    /// <remarks>
    /// This service handles query operations for both users and administrative
    /// roles. It returns DTOs that are safe to use in UI components for
    /// table displays, previews, and downloads without exposing domain entities.
    /// </remarks>
    public interface IPaperQueryService
    {
        /// <summary>
        /// Retrieves a list of papers submitted by a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the submitter.</param>
        /// <param name="status">
        /// Optional filter by <see cref="PaperStatus"/>. 
        /// If null, all statuses are returned.
        /// </param>
        /// <returns>
        /// A collection of <see cref="UserPaperDTO"/> representing each paper
        /// with minimal metadata, authors formatted for display, and file summaries.
        /// </returns>
        Task<IEnumerable<UserPaperDTO>> GetUserPapersAsync(Guid userId, PaperStatus? status = null);

        /// <summary>
        /// Retrieves detailed information about a single paper.
        /// </summary>
        /// <param name="paperId">The unique identifier of the paper.</param>
        /// <param name="userId">The requesting user ID (used for authorization).</param>
        /// <returns>
        /// A <see cref="PaperDetailsDTO"/> containing full metadata, authors,
        /// and file summaries for preview or download.
        /// </returns>
        Task<PaperDetailsDTO> GetPaperDetailsAsync(Guid paperId, Guid userId);

        /// <summary>
        /// Retrieves the requested file for a paper submission (PDF, camera-ready, copyright form, etc.).
        /// Delegates to the underlying file storage service for reading the stream.
        /// </summary>
        /// <param name="paperId">The unique identifier of the paper.</param>
        /// <param name="fileType">The type of file being requested.</param>
        /// <param name="userId">The requesting user (for authorization).</param>
        /// <param name="cancellationToken">Optional cancellation token for aborting the operation.</param>
        /// <returns>
        /// A <see cref="FileReadResult"/> containing the file stream if found,
        /// or an error message if the file cannot be accessed.
        /// </returns>
        Task<FileReadResult> GetPaperFileAsync(
            Guid paperId,
            FileType fileType,
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all papers in the system, optionally filtered by status.
        /// Intended for conference chairs or administrators to view every submission.
        /// </summary>
        /// <param name="status">
        /// Optional <see cref="PaperStatus"/> filter to narrow the results.
        /// If null, all papers are returned.
        /// </param>
        /// <returns>
        /// A collection of <see cref="UserPaperDTO"/> with metadata, authors,
        /// and file summaries suitable for display in an overview table.
        /// </returns>
        Task<IEnumerable<UserPaperDTO>> GetAllPapersAsync(PaperStatus? status = null);
    }
}