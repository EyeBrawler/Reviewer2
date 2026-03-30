using System.Collections.Generic;

namespace Reviewer2.Services.DTOs.ReviewAssignments;

/// <summary>
/// Represents the complete data required to render the review assignment
/// interface for conference chairs.
/// 
/// This DTO aggregates all papers eligible for reviewer assignment along
/// with the pool of available reviewers. It is intended to support
/// drag-and-drop or grid-based assignment workflows in the UI.
/// 
/// The data structure is optimized for a single payload that enables
/// efficient rendering of the assignment board without requiring
/// multiple round trips to the server.
/// </summary>
public class ReviewAssignmentBoardDTO
{
    /// <summary>
    /// The collection of papers included in the assignment board.
    /// 
    /// Each paper contains its metadata and the set of reviewers
    /// currently assigned to it.
    /// </summary>
    public List<PaperAssignmentDTO> Papers { get; set; } = new();

    /// <summary>
    /// The pool of reviewers available for assignment.
    /// 
    /// This includes reviewer identity, current workload, and
    /// any indicators (such as conflicts) that may influence
    /// assignment decisions.
    /// </summary>
    public List<ReviewerPoolDTO> Reviewers { get; set; } = new();
}