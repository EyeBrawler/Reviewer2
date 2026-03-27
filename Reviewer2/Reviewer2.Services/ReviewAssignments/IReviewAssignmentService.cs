using System;
using System.Threading.Tasks;
using Reviewer2.Services.DTOs.ReviewAssignments;

namespace Reviewer2.Services.ReviewAssignments;

/// <summary>
/// Defines operations for managing reviewer assignments to papers
/// within the conference review workflow.
/// 
/// This service acts as the intermediary between the presentation layer
/// and the domain model, handling business rules such as conflict
/// detection, duplicate prevention, and assignment lifecycle management.
/// 
/// It is primarily used to support reviewer assignment interfaces,
/// including drag-and-drop or grid-based management views for
/// conference chairs.
/// </summary>
public interface IReviewAssignmentService
{
    /// <summary>
    /// Retrieves the complete review assignment board data.
    /// 
    /// This includes all relevant papers and their current reviewer
    /// assignments, along with the pool of available reviewers.
    /// 
    /// The returned data is optimized for rendering assignment
    /// interfaces in a single request.
    /// </summary>
    /// <returns>
    /// A <see cref="ReviewAssignmentBoardDTO"/> containing papers,
    /// assigned reviewers, and available reviewer pool data.
    /// </returns>
    Task<ReviewAssignmentBoardDTO> GetAssignmentBoardAsync();

    /// <summary>
    /// Assigns a reviewer to a specified paper.
    /// 
    /// This operation creates a new review assignment if one does not
    /// already exist and enforces business rules such as preventing
    /// authors from reviewing their own papers and avoiding duplicate
    /// assignments.
    /// </summary>
    /// <param name="paperId">
    /// The unique identifier of the paper to which the reviewer
    /// will be assigned.
    /// </param>
    /// <param name="reviewerId">
    /// The unique identifier of the reviewer (user) being assigned.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the assignment violates business rules, such as
    /// assigning an author to their own paper.
    /// </exception>
    Task AssignReviewerAsync(Guid paperId, Guid reviewerId);

    /// <summary>
    /// Removes an existing reviewer assignment.
    /// 
    /// This operation deletes or deactivates the assignment,
    /// depending on implementation, and updates the assignment
    /// board accordingly.
    /// </summary>
    /// <param name="assignmentId">
    /// The unique identifier of the review assignment to remove.
    /// </param>
    Task RemoveReviewerAsync(Guid assignmentId);
}