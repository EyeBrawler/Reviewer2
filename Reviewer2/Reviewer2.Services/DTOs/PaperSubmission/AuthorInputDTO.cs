using System;

namespace Reviewer2.Services.DTOs.PaperSubmission;

/// <summary>
/// Represents author information provided during paper creation
/// or draft update operations.
/// 
/// This DTO is used to transfer author data from the presentation layer
/// to the application/service layer. It does not represent a persisted entity.
/// </summary>
public class AuthorInputDTO
{
    /// <summary>
    /// Optional identifier of an existing user in the system.
    /// 
    /// If provided, the author may be linked to a registered user account.
    /// If null, the author is treated as an external contributor.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// The author's given (first) name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// The author's family (last) name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The email address associated with the author.
    /// 
    /// This may be used for notifications or correspondence.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The institutional affiliation of the author.
    /// </summary>
    public string Institution { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether this author is the corresponding author.
    /// 
    /// Exactly one corresponding author is typically required per submission.
    /// </summary>
    public bool IsCorrespondingAuthor { get; set; }

    /// <summary>
    /// Indicates whether this author will present the paper
    /// at the conference if accepted.
    /// </summary>
    public bool IsPresenter { get; set; }
}