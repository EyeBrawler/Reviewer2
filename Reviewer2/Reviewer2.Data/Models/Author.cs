using System;
namespace Reviewer2.Data.Models;

/// <summary>
/// Represents an author associated with a specific paper submission.
/// 
/// An Author is a paper-scoped entity and does not necessarily correspond
/// to a registered system user. The submitting author must have a user account,
/// but co-authors may exist without one.
/// 
/// Author records store snapshot information (name, email, institution)
/// at the time of submission to preserve historical accuracy even if a linked
/// user later updates their profile.
/// </summary>
public class Author
{
    /// <summary>
    /// Unique identifier for the author record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The identifier of the paper this author is associated with.
    /// </summary>
    public Guid PaperId { get; set; }

    /// <summary>
    /// Optional reference to a registered user account.
    /// 
    /// This will be populated when the author is a registered system user.
    /// Co-authors without accounts will have this value set to null.
    /// </summary>
    public Guid? UserId { get; set; }
    
    /// <summary>
    /// Navigation property to the associated user account, if one exists.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// The author's given (first) name as provided during submission.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// The author's family (last) name as provided during submission.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The author's contact email address at the time of submission.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The institutional affiliation of the author at the time of submission.
    /// </summary>
    public string Institution { get; set; } = string.Empty;

    /// <summary>
    /// Determines the display order of authors for the paper.
    /// 
    /// AuthorOrder is a zero-based or one-based sequential index (depending on
    /// system convention) that defines the official authorship order as it
    /// should appear in publications and proceedings.
    /// 
    /// Lower values indicate earlier authorship position. For example:
    ///     0 (or 1) = first author
    ///     1 (or 2) = second author
    /// 
    /// The system should enforce uniqueness of AuthorOrder per Paper and
    /// prevent duplicate ordering values.
    /// </summary>
    public int AuthorOrder { get; set; }

    /// <summary>
    /// Indicates whether this author is the corresponding author.
    /// 
    /// The corresponding author is typically responsible for communication
    /// with conference organizers and may receive system notifications.
    /// Only one author per paper should have this flag set to true.
    /// </summary>
    public bool IsCorrespondingAuthor { get; set; }

    /// <summary>
    /// Indicates whether this author is designated as the presenter.
    /// 
    /// The presenter must typically be a registered user and have completed
    /// conference registration before the paper can be scheduled.
    /// Only one author per paper should have this flag set to true.
    /// </summary>
    public bool IsPresenter { get; set; }

    /// <summary>
    /// Navigation property to the associated paper submission.
    /// </summary>
    public Paper Paper { get; set; } = null!;
}
