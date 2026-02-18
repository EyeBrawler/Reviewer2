using System;

namespace Reviewer2.Data.Models;

public class PaperFile
{
    public Guid Id { get; set; }

    public Guid PaperId { get; set; }

    public FileType Type { get; set; }

    public string StoredFileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;

    public long SizeInBytes { get; set; }

    public DateTime UploadedAtUtc { get; set; }
}

public enum FileType
{
    InitialSubmission = 0,
    CameraReady = 1,
    CopyrightForm = 2
}
