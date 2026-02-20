using System;
using System.IO;
using Reviewer2.Data.Models;

namespace Reviewer2.Services.DTOs.PaperSubmission;

/// <summary>
/// Represents a request to upload a file associated with a paper submission.
/// 
/// This DTO contains the file stream and related metadata required
/// for storage and persistence.
/// </summary>
public class UploadPaperFileRequest
{
    /// <summary>
    /// The identifier of the paper to which the file belongs.
    /// </summary>
    public Guid PaperId { get; set; }

    /// <summary>
    /// The classification of the uploaded file
    /// (e.g., InitialSubmission, CameraReady, Supplementary).
    /// </summary>
    public FileType FileType { get; set; }

    /// <summary>
    /// The file content stream.
    /// 
    /// The stream should be positioned at the beginning before processing.
    /// The caller is responsible for disposing of the stream if required.
    /// </summary>
    public Stream Content { get; set; } = Stream.Null;

    /// <summary>
    /// The original filename as provided by the uploader.
    /// 
    /// This may be stored for display purposes but should not be trusted
    /// for filesystem paths.
    /// </summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// The size of the file in bytes.
    /// 
    /// This may be used for validation and storage quota enforcement.
    /// </summary>
    public long SizeInBytes { get; set; }
}