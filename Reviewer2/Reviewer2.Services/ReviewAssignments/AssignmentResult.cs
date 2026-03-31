namespace Reviewer2.Services.ReviewAssignments;

/// <summary>
/// Represents the result of an attempted reviewer assignment operation.
/// 
/// This enum provides detailed feedback about the outcome of an
/// assignment request, enabling the UI to display meaningful
/// messages or take appropriate actions.
/// </summary>
public enum AssignmentResult
{
    /// <summary>
    /// The operation completed successfully.
    /// </summary>
    Success,

    /// <summary>
    /// The reviewer is already assigned to the specified paper.
    /// </summary>
    AlreadyAssigned,

    /// <summary>
    /// The assignment could not be completed due to a conflict,
    /// such as the reviewer being an author of the paper.
    /// </summary>
    ReviewerIsAuthor,

    /// <summary>
    /// The target entity (paper, reviewer, or assignment) was not found.
    /// </summary>
    NotFound
}