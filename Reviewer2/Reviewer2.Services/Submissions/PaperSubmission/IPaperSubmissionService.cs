using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Reviewer2.Data.Models;
using Reviewer2.Services.DTOs.FileStorage;
using Reviewer2.Services.DTOs.PaperSubmission;

namespace Reviewer2.Services.Submissions.PaperSubmission;

/// <summary>
/// Defines application-level operations for managing paper submissions.
/// </summary>
/// <remarks>
/// This service coordinates domain logic, persistence, and file storage
/// concerns for the paper submission workflow. It enforces authorization,
/// validates workflow rules, and ensures consistency between database
/// records and external file storage.
/// </remarks>
public interface IPaperSubmissionService
{
    /// <summary>
    /// Creates a new draft paper submission for the specified user.
    /// </summary>
    /// <param name="request">
    /// The data required to initialize the draft submission.
    /// </param>
    /// <param name="userId">
    /// The identifier of the user creating the draft.
    /// </param>
    /// <returns>
    /// The unique identifier of the newly created paper draft.
    /// </returns>
    /// <remarks>
    /// The created paper will be initialized in the Draft state.
    /// </remarks>
    Task<Guid> CreateDraftAsync(CreateDraftRequest request, Guid userId);

    /// <summary>
    /// Updates the metadata of an existing draft paper submission.
    /// </summary>
    /// <param name="paperId">
    /// The identifier of the paper to update.
    /// </param>
    /// <param name="request">
    /// The updated metadata values for the draft.
    /// </param>
    /// <param name="userId">
    /// The identifier of the user performing the update.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// Metadata updates are only permitted while the paper
    /// is in the Draft state.
    /// </remarks>
    Task UpdateDraftAsync(Guid paperId, UpdateDraftRequest request, Guid userId);

    /// <summary>
    /// Uploads or replaces a file associated with a paper submission.
    /// </summary>
    /// <param name="paperId">
    /// The unique identifier of the paper the file belongs to.
    /// </param>
    /// <param name="fileType">
    /// The logical type and purpose of the uploaded file.
    /// </param>
    /// <param name="stream">
    /// A readable stream containing the file contents. The caller is responsible
    /// for disposing the stream.
    /// </param>
    /// <param name="originalFileName">
    /// The original file name provided by the user.
    /// </param>
    /// <param name="userId">
    /// The identifier of the user attempting to perform the upload.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="StoredFileResult"/> indicating whether the file was
    /// successfully stored and associated with the paper, or describing
    /// why the operation failed.
    /// </returns>
    /// <remarks>
    /// If a file of the same <see cref="FileType"/> already exists for the paper,
    /// it will be replaced according to the paper's workflow rules.
    ///
    /// Implementations must ensure consistency between file storage and
    /// persistence so that partial state is not left behind if the operation fails.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if required arguments are <c>null</c>.
    /// </exception>
    public Task<StoredFileResult> UploadFileAsync(
        Guid paperId,
        FileType fileType,
        Stream stream,
        string originalFileName,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Submits a draft paper for review.
    /// </summary>
    /// <param name="paperId">
    /// The identifier of the paper to submit.
    /// </param>
    /// <param name="userId">
    /// The identifier of the user performing the submission.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// Submission transitions the paper from Draft to Submitted state.
    /// All required metadata, authors, and files must be present
    /// before submission is allowed.
    /// </remarks>
    Task SubmitAsync(Guid paperId, Guid userId);
}