using System;

namespace Reviewer2.Services.DTOs.ReviewAssignments;

/// <summary>
/// Represents a reviewer available for assignment within the review
/// assignment interface.
/// 
/// This DTO is used in the presentation layer to populate the reviewer
/// pool, allowing conference chairs to select and assign reviewers
/// to papers.
/// 
/// It includes basic identity information, current workload, and
/// indicators that may affect assignment decisions, such as conflicts.
/// </summary>
public class ReviewerPoolDTO
{
    /// <summary>
    /// The unique identifier of the reviewer (user).
    /// 
    /// This value is used when creating or managing review assignments.
    /// </summary>
    public Guid ReviewerId { get; set; }

    /// <summary>
    /// The display name of the reviewer.
    /// 
    /// Typically derived from the user's full name and used for
    /// rendering in the user interface.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// The number of review assignments currently associated with
    /// this reviewer.
    /// 
    /// This value helps conference chairs balance reviewer workload
    /// when assigning papers.
    /// </summary>
    public int AssignmentCount { get; set; }

    /// <summary>
    /// Indicates whether the reviewer has a conflict with the
    /// currently selected or relevant paper.
    /// 
    /// Conflicts may arise from authorship, institutional affiliation,
    /// or other defined rules, and should typically prevent assignment.
    /// </summary>
    public bool HasConflict { get; set; }
}