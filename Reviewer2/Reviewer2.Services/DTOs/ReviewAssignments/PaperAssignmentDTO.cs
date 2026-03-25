using System;
using System.Collections.Generic;

namespace Reviewer2.Services.DTOs.ReviewAssignments;

/// <summary>
/// Represents a paper and its associated reviewer assignments within
/// the review assignment interface.
/// 
/// This DTO is used in the presentation layer to display key paper
/// metadata alongside the reviewers currently assigned to evaluate it.
/// 
/// It is optimized for use in assignment board views, such as
/// drag-and-drop interfaces, where conference chairs manage reviewer
/// distribution across submissions.
/// </summary>
public class PaperAssignmentDTO
{
    /// <summary>
    /// The unique identifier of the paper.
    /// 
    /// This value is used to associate reviewer assignment operations
    /// (such as adding or removing reviewers) with the correct paper.
    /// </summary>
    public Guid PaperId { get; set; }

    /// <summary>
    /// The title of the paper.
    /// 
    /// This is the primary display field used to identify the paper
    /// within the assignment interface.
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// The ordered list of author names associated with the paper.
    /// 
    /// This list is formatted for display purposes and reflects the
    /// authorship order as defined in the underlying data model.
    /// </summary>
    public List<string> Authors { get; set; } = new();

    /// <summary>
    /// The collection of reviewers currently assigned to this paper.
    /// 
    /// Each entry represents an individual review assignment and
    /// includes reviewer identity and review status information.
    /// </summary>
    public List<AssignedReviewerDTO> AssignedReviewers { get; set; } = new();
}