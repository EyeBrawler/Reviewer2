using System;

namespace Reviewer2.Services.DTOs.ReviewAssignments;

/// <summary>
/// Represents a reviewer available for assignment within the review
/// assignment interface.
/// 
/// This DTO is used to populate a global reviewer pool and includes
/// identity and workload information. It does not include conflict
/// data, as conflicts are evaluated per paper and represented in
/// <see cref="ReviewerCandidateDTO"/>.
/// </summary>
public class ReviewerPoolDTO
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
    /// </summary>
    public int AssignmentCount { get; set; }
}