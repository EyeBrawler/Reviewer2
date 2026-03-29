using System;
using System.Collections.Generic;

namespace Reviewer2.Services.DTOs.ReviewAssignments;

/// <summary>
/// Represents the result of a simulated auto-assignment for a single paper.
/// 
/// This DTO is used in preview mode to show which reviewers would be
/// assigned without actually modifying the database.
/// </summary>
public class AutoAssignmentPreviewDTO
{
    /// <summary>
    /// The unique identifier of the paper.
    /// </summary>
    public Guid PaperId { get; set; }

    /// <summary>
    /// The title of the paper.
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// The reviewers that would be assigned to this paper.
    /// </summary>
    public List<ReviewerCandidateDTO> SuggestedReviewers { get; set; } = new();

    /// <summary>
    /// Indicates whether the paper could not be fully assigned
    /// due to insufficient eligible reviewers.
    /// </summary>
    public bool IsIncomplete { get; set; }
}