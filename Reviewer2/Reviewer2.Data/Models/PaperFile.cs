using System;

namespace Reviewer2.Data.Models;

/// <summary>
/// Represents a file uploaded in association with a paper submission.
/// 
/// Paper files are stored separately from the Paper entity to allow
/// multiple uploads (e.g., initial submission, camera-ready version,
/// copyright forms) and to preserve upload history and metadata.
/// 
/// The actual file contents are stored in external storage (e.g., file system,
/// cloud storage), while this entity maintains the metadata required
/// for retrieval, auditing, and validation.
/// </summary>
public class PaperFile
{
    /// <summary>
    /// Unique identifier for the file record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identifier of the paper this file is associated with.
    /// </summary>
    public Guid PaperId { get; set; }

    /// <summary>
    /// Indicates the purpose or category of the uploaded file
    /// (e.g., initial submission, camera-ready version).
    /// </summary>
    public FileType Type { get; set; }

    /// <summary>
    /// The internal file name used for storage.
    /// 
    /// This is typically a generated unique name to prevent collisions
    /// and should not be shown directly to end users.
    /// </summary>
    public string StoredFileName { get; set; } = string.Empty;

    /// <summary>
    /// The original file name as provided by the user at upload time.
    /// 
    /// This value is displayed to users for download and reference.
    /// </summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// The size of the file in bytes at the time of upload.
    /// 
    /// This can be used for validation, display, or storage auditing.
    /// </summary>
    public long SizeInBytes { get; set; }

    /// <summary>
    /// The date and time (UTC) when the file was uploaded.
    /// </summary>
    public DateTime UploadedAtUtc { get; set; }
}

/// <summary>
/// Defines the type and purpose of a file uploaded for a paper.
/// 
/// The file type determines when the file is required in the workflow
/// and who may access it.
/// </summary>
public enum FileType
{
    /// <summary>
    /// The initial full paper submission provided for review.
    /// This version is typically visible to reviewers.
    /// </summary>
    InitialSubmission = 0,

    /// <summary>
    /// The final camera-ready version submitted after acceptance.
    /// This version is typically used for proceedings publication.
    /// </summary>
    CameraReady = 1,

    /// <summary>
    /// The signed copyright or licensing agreement required
    /// prior to publication.
    /// </summary>
    CopyrightForm = 2
}