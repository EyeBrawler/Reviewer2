using System.IO;

namespace Reviewer2.Services.DTOs.FileStorage;

/// <summary>
/// Represents the result of attempting to read a file from storage.
/// Encapsulates both successful reads and failure scenarios without
/// requiring the caller to rely on exceptions for control flow.
/// </summary>
public sealed class FileReadResult
{
    /// <summary>
    /// Gets a value indicating whether the file was successfully located
    /// and opened for reading.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the readable <see cref="Stream"/> for the file if the operation
    /// was successful. This will be <c>null</c> when <see cref="Success"/> is <c>false</c>.
    /// </summary>
    /// <remarks>
    /// The caller is responsible for disposing the returned stream.
    /// </remarks>
    public Stream? Stream { get; }

    /// <summary>
    /// Gets an error message describing why the file could not be read.
    /// This will be <c>null</c> when <see cref="Success"/> is <c>true</c>.
    /// </summary>
    /// <remarks>
    /// Intended for logging or safe user-facing display. Should not expose
    /// sensitive filesystem details in production environments.
    /// </remarks>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileReadResult"/> class.
    /// </summary>
    /// <param name="success">Indicates whether the read operation succeeded.</param>
    /// <param name="stream">The file stream if successful; otherwise <c>null</c>.</param>
    /// <param name="errorMessage">An error message if unsuccessful; otherwise <c>null</c>.</param>
    private FileReadResult(bool success, Stream? stream, string? errorMessage)
    {
        Success = success;
        Stream = stream;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result containing the opened file stream.
    /// </summary>
    /// <param name="stream">The readable stream for the file.</param>
    /// <returns>A successful <see cref="FileReadResult"/>.</returns>
    public static FileReadResult Found(Stream stream)
        => new(true, stream, null);

    /// <summary>
    /// Creates a failure result indicating that the file could not be found
    /// or accessed.
    /// </summary>
    /// <param name="message">
    /// A descriptive message explaining why the file could not be read.
    /// </param>
    /// <returns>An unsuccessful <see cref="FileReadResult"/>.</returns>
    public static FileReadResult NotFound(string message)
        => new(false, null, message);
}
