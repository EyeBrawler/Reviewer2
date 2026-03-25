using System;
using Reviewer2.Data.Models;

namespace Reviewer2.Services.DTOs.ReviewAssignments;

/// <summary>
/// Represents a reviewer currently assigned to a paper within the
/// review assignment interface.
/// 
/// This DTO is used in the presentation layer to display reviewer
/// information associated with a specific paper, including the
/// assignment identifier and current review status.
/// </summary>
public class AssignedReviewerDTO
{
    /// <summary>
    /// The unique identifier of the review assignment.
    /// 
    /// This value is used to manage the assignment lifecycle,
    /// including removal or status updates.
    /// </summary>
    public Guid AssignmentId { get; set; }

    /// <summary>
    /// The unique identifier of the reviewer (user).
    /// 
    /// This corresponds to the <see cref="ApplicationUser"/> 
    /// associated with the assignment.
    /// </summary>
    public Guid ReviewerId { get; set; }

    /// <summary>
    /// The display name of the reviewer.
    /// 
    /// Typically derived from the user's full name and used
    /// for rendering in the user interface.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// The current status of the review assignment.
    /// 
    /// This indicates the reviewer's progress in the review
    /// lifecycle, such as pending, in progress, or submitted.
    /// </summary>
    public ReviewStatus Status { get; set; }
}