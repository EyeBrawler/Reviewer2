namespace Reviewer2.Services.CRUD.FileStorage;

/// <summary>
/// Represents configuration settings for the <see cref="IFileStorageService"/> implementation.
/// Provides options to specify where files should be physically stored.
/// </summary>
public class FileStorageOptions
{
    /// <summary>
    /// Gets or sets the base directory path where files managed by the <see cref="IFileStorageService"/>
    /// will be stored. This path should be accessible by the application and writable.
    /// 
    /// Example: "/var/app/files" on Linux or "C:\App\Files" on Windows.
    /// </summary>
    public string BasePath { get; set; } = string.Empty;
}
