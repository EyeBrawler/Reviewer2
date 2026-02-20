namespace Reviewer2.Services.DTOs.PaperSubmission;

/// <summary>
/// DTO for richer admin/chair views including submitter info.
/// </summary>
public class AdminPaperDTO : UserPaperDTO
{
    /// <summary>
    /// Name or email of the user who submitted the paper.
    /// </summary>
    public string SubmitterName { get; set; } = string.Empty;

    // You can add more metadata here in the future if needed.
    // Example: Number of reviews, last modified date, etc.
}