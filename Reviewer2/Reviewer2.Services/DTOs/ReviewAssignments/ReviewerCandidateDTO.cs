using System;

namespace Reviewer2.Services.DTOs.ReviewAssignments;

/// <summary>
/// Represents a reviewer as a candidate for assignment to a specific paper.
/// 
/// This DTO extends basic reviewer information with context-specific
/// evaluation data, such as conflict status, making it suitable for
/// use in assignment decision interfaces.
/// 
/// Unlike <see cref="ReviewerPoolDTO"/>, this DTO is scoped to a specific
/// paper and reflects whether the reviewer is eligible to review it.
/// </summary>
public class ReviewerCandidateDTO
{
    /// <summary>
    /// The unique identifier of the reviewer (user).
    /// </summary>
    public Guid ReviewerId { get; set; }

    /// <summary>
    /// The display name of the reviewer.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// The number of review assignments currently associated with
    /// this reviewer.
    /// 
    /// This value helps conference chairs balance reviewer workload.
    /// </summary>
    public int AssignmentCount { get; set; }

    /// <summary>
    /// Indicates whether the reviewer has a conflict with the
    /// associated paper.
    /// 
    /// Conflicts may arise from authorship, institutional affiliation,
    /// or other defined rules.
    /// </summary>
    public bool HasConflict { get; set; }

    /// <summary>
    /// Provides a human-readable explanation of the conflict, if one exists.
    /// 
    /// This value is null when <see cref="HasConflict"/> is false.
    /// </summary>
    public string? ConflictReason { get; set; }
}