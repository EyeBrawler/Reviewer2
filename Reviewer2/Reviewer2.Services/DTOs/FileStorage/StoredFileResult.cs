namespace Reviewer2.Services.DTOs.FileStorage;

/// <summary>
/// Represents the result of attempting to retrieve or inspect a stored file.
/// This object allows the application to distinguish between:
/// - A file that exists and is accessible
/// - A file that is missing from disk
/// - A file that could not be accessed due to an error
/// </summary>
public sealed class StoredFileResult
{
    private StoredFileResult(
        bool exists,
        string? physicalPath,
        string? errorMessage)
    {
        Exists = exists;
        PhysicalPath = physicalPath;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Gets a value indicating whether the file exists on disk.
    /// </summary>
    public bool Exists { get; }

    /// <summary>
    /// Gets the resolved physical path of the file if it exists.
    /// Will be <c>null</c> if the file does not exist or could not be resolved.
    /// </summary>
    public string? PhysicalPath { get; }

    /// <summary>
    /// Gets an optional error message describing why the file
    /// could not be accessed. This is intended for logging or
    /// safe user-facing messaging.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Creates a successful result indicating the file exists.
    /// </summary>
    /// <param name="physicalPath">The resolved physical path of the file.</param>
    public static StoredFileResult Success(string physicalPath)
        => new StoredFileResult(true, physicalPath, null);

    /// <summary>
    /// Creates a result indicating the file does not exist on disk.
    /// </summary>
    public static StoredFileResult NotFound()
        => new StoredFileResult(false, null, null);

    /// <summary>
    /// Creates a result indicating an error occurred while attempting
    /// to access or resolve the file.
    /// </summary>
    /// <param name="errorMessage">
    /// A description of the error. Intended for logging and safe display.
    /// </param>
    public static StoredFileResult Error(string errorMessage)
        => new StoredFileResult(false, null, errorMessage);
}
