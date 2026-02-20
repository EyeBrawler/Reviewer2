using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Reviewer2.Services.DTOs.FileStorage;

namespace Reviewer2.Services.CRUD.FileStorage;

/// <summary>
/// Local filesystem implementation of <see cref="IFileStorageService"/>.
/// </summary>
/// <remarks>
/// Files are stored under a configured root directory using
/// generated unique names to prevent collisions.
/// </remarks>
public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalFileStorageService"/> class.
    /// </summary>
    /// <param name="options">
    /// The <see cref="FileStorageOptions"/> instance providing the root path configuration.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the root path is invalid.
    /// </exception>
    public LocalFileStorageService(IOptions<FileStorageOptions> options)
    {
        if (options?.Value == null || string.IsNullOrWhiteSpace(options.Value.BasePath))
            throw new ArgumentException("Root path is required in FileStorageOptions.", nameof(options));

        _rootPath = Path.GetFullPath(options.Value.BasePath);
        Directory.CreateDirectory(_rootPath);
    }

    /// <inheritdoc/>
    public async Task<StoredFileResult> SaveAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required.", nameof(fileName));

        try
        {
            var safeExtension = Path.GetExtension(fileName);
            var generatedName = $"{Guid.NewGuid()}{safeExtension}";

            var now = DateTime.UtcNow;
            var year = now.Year.ToString();
            var month = now.Month.ToString("D2");

            var relativeDirectory = Path.Combine(year, month);
            var absoluteDirectory = Path.Combine(_rootPath, relativeDirectory);

            Directory.CreateDirectory(absoluteDirectory);

            var absolutePath = Path.Combine(absoluteDirectory, generatedName);

            await using (var fileStream = new FileStream(
                absolutePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                81920,
                useAsync: true))
            {
                await content.CopyToAsync(fileStream, cancellationToken);
            }

            var relativePath = Path.Combine(relativeDirectory, generatedName);

            return StoredFileResult.Success(relativePath);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return StoredFileResult.Error($"Failed to store file: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public Task DeleteAsync(
        string storagePath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
            return Task.CompletedTask;

        try
        {
            var absolutePath = ResolveAbsolutePath(storagePath);

            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }
        }
        catch
        {
            // Intentionally swallow exceptions to match interface contract.
            // Consider logging here in a real implementation.
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<FileReadResult> TryOpenReadAsync(
        string storagePath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
            return Task.FromResult(FileReadResult.NotFound("Storage path is invalid."));

        try
        {
            var absolutePath = ResolveAbsolutePath(storagePath);

            if (!File.Exists(absolutePath))
            {
                return Task.FromResult(
                    FileReadResult.NotFound("File does not exist in storage."));
            }

            var stream = new FileStream(
                absolutePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                81920,
                useAsync: true);

            return Task.FromResult(FileReadResult.Found(stream));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                FileReadResult.NotFound($"Failed to open file: {ex.Message}"));
        }
    }

    private string ResolveAbsolutePath(string storagePath)
    {
        var combined = Path.GetFullPath(Path.Combine(_rootPath, storagePath));

        if (!combined.StartsWith(_rootPath, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Invalid storage path detected.");

        return combined;
    }
}