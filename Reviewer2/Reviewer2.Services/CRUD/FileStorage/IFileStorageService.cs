using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Reviewer2.Services.DTOs.FileStorage;

namespace Reviewer2.Services.CRUD.FileStorage;

/// <summary>
/// Defines an abstraction for file storage operations.
/// Implementations may store files on the local filesystem,
/// cloud storage, or another backing store.
/// </summary>
/// <remarks>
/// This interface is intentionally infrastructure-focused and should not
/// contain domain logic. It provides safe, explicit result types for
/// file operations to allow the application layer to handle missing files
/// and storage errors gracefully.
/// </remarks>
public interface IFileStorageService
{
    /// <summary>
    /// Saves a file to the underlying storage system.
    /// </summary>
    /// <param name="content">
    /// The stream containing the file content to store.
    /// The caller remains responsible for disposing the stream.
    /// </param>
    /// <param name="fileName">
    /// The original file name. Implementations may use this to generate
    /// a storage path or normalize the name for safe storage.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="StoredFileResult"/> describing whether the file
    /// was successfully stored and where it was saved.
    /// </returns>
    /// <remarks>
    /// Implementations should ensure file names are sanitized and
    /// storage paths are safe to prevent path traversal vulnerabilities.
    /// </remarks>
    Task<StoredFileResult> SaveAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from the underlying storage system.
    /// </summary>
    /// <param name="storagePath">
    /// The storage-relative path or identifier of the file to delete.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    /// <remarks>
    /// Implementations should not throw if the file does not exist.
    /// Deletion of a non-existent file should be treated as a no-op.
    /// </remarks>
    Task DeleteAsync(
        string storagePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to open a stored file for reading.
    /// </summary>
    /// <param name="storagePath">
    /// The storage-relative path or identifier of the file to open.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="FileReadResult"/> indicating whether the file
    /// was successfully opened. If successful, the result will contain
    /// a readable <see cref="Stream"/>.
    /// </returns>
    /// <remarks>
    /// The caller is responsible for disposing the returned stream.
    /// Missing files should not cause exceptions; instead, a failure
    /// result should be returned so the application can display an
    /// appropriate message.
    /// </remarks>
    Task<FileReadResult> TryOpenReadAsync(
        string storagePath,
        CancellationToken cancellationToken = default);
}