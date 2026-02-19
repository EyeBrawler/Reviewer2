using System;

namespace Reviewer2.Data.Models;

/// <summary>
/// Represents the assignment of a reviewer to a specific paper.
/// 
/// A ReviewAssignment establishes the relationship between a paper
/// and a reviewer and tracks the lifecycle of the review process.
/// 
/// Each assignment may have at most one associated <see cref="Review"/>.
/// The assignment controls the review status independently of the
/// review content itself.
/// </summary>
public class ReviewAssignment
{
    /// <summary>
    /// Unique identifier for the review assignment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identifier of the paper being reviewed.
    /// </summary>
    public Guid PaperId { get; set; }

    /// <summary>
    /// Identifier of the user assigned to review the paper.
    /// 
    /// The user must have the appropriate Reviewer role to
    /// access and submit a review.
    /// </summary>
    public Guid ReviewerId { get; set; }

    /// <summary>
    /// Navigation property to the assigned reviewer.
    /// </summary>
    public ApplicationUser Reviewer { get; set; } = null!;
    
    /// <summary>
    /// Represents the current state of the review assignment.
    /// 
    /// The status determines whether the reviewer may submit,
    /// edit, or decline the review.
    /// </summary>
    public ReviewStatus Status { get; set; }

    /// <summary>
    /// The review submitted for this assignment, if one exists.
    /// 
    /// This property remains null until the reviewer begins
    /// or submits their review.
    /// </summary>
    public Review? Review { get; set; }
}

/// <summary>
/// Represents the lifecycle state of a review assignment.
/// 
/// Status transitions are controlled by reviewer actions and
/// conference leadership decisions.
/// </summary>
public enum ReviewStatus
{
    /// <summary>
    /// The review has been assigned but the reviewer has not yet started.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// The reviewer has begun filling out the review form
    /// but has not formally submitted it.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// The review has been completed and formally submitted.
    /// </summary>
    Submitted = 2,

    /// <summary>
    /// The reviewer declined the assignment before submission.
    /// </summary>
    Declined = 3,

    /// <summary>
    /// The assignment was removed by a chair or administrator.
    /// </summary>
    Withdrawn = 4,

    /// <summary>
    /// The review deadline has passed without submission.
    /// </summary>
    Overdue = 5
}