using System;
using System.Collections.Generic;
using System.Linq;
using Reviewer2.Data.Models;
using Reviewer2.Services.DTOs.PaperSubmission;

namespace Reviewer2.Services.DTOs.ReviewAssignments;

/// <summary>
/// Represents a paper from the perspective of a reviewer,
/// including both paper metadata and reviewer-specific assignment details.
/// 
/// This DTO is designed for reviewer dashboards and workflows where
/// the focus is on the review assignment lifecycle rather than
/// the submission process itself.
/// 
/// Each instance corresponds to a single <see cref="ReviewAssignment"/>
/// and its associated paper.
/// </summary>
public class ReviewerPaperDTO
{
    /// <summary>
    /// Unique identifier of the paper being reviewed.
    /// </summary>
    public Guid PaperId { get; set; }

    /// <summary>
    /// Title of the paper.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the paper in the submission lifecycle
    /// (e.g., Submitted, UnderReview, Accepted, Rejected).
    /// 
    /// This reflects the overall conference decision process and is
    /// independent of the review assignment status.
    /// </summary>
    public string PaperStatus { get; set; } = string.Empty;

    /// <summary>
    /// The date and time (UTC) when the paper was originally submitted.
    /// </summary>
    public DateTime? SubmittedAtUtc { get; set; }

    /// <summary>
    /// Comma-separated list of authors for display purposes.
    /// 
    /// Authors are typically ordered according to their declared
    /// author order in the submission. They will not always be displayed
    /// </summary>
    public string Authors { get; set; } = string.Empty;

    /// <summary>
    /// Collection of files associated with the paper, such as
    /// the main manuscript or supplementary materials.
    /// 
    /// Each file includes metadata and a URL for retrieval.
    /// </summary>
    public List<PaperFileSummaryDTO> Files { get; set; } = new();
    
    /// <summary>
    /// Gets the URL of the initial submission manuscript file associated with this paper, if available.
    /// 
    /// This property is intended for reviewer workflows, where the original submitted
    /// version of the paper is typically used for evaluation.
    /// 
    /// Returns <c>null</c> if no file with the type <c>InitialSubmission</c> is present.
    /// </summary>
    public string? InitialSubmissionUrl =>
        Files?
            .FirstOrDefault(f => f.FileType == "InitialSubmission")
            ?.FileUrl;

    /// <summary>
    /// Gets the URL of the camera-ready (final) version of the manuscript, if available.
    /// 
    /// This property is primarily used in post-acceptance workflows, where the finalized
    /// version of the paper is required for publication or archival purposes.
    /// 
    /// Returns <c>null</c> if no file with the type <c>CameraReady</c> is present.
    /// </summary>
    public string? CameraReadyUrl =>
        Files?
            .FirstOrDefault(f => f.FileType == "CameraReady")
            ?.FileUrl;
    
    /// <summary>
    /// Unique identifier of the review assignment linking the reviewer
    /// to this paper.
    /// 
    /// This value should be used when performing actions such as
    /// submitting, updating, or declining a review.
    /// </summary>
    public Guid ReviewAssignmentId { get; set; }

    /// <summary>
    /// Current status of the review assignment.
    /// </summary>
    public ReviewStatus ReviewStatus { get; set; }

    /// <summary>
    /// The date and time (UTC) when the review was formally submitted, if submitted.
    /// </summary>
    public DateTimeOffset? ReviewSubmittedAtUtc { get; set; }

    /// <summary>
    /// The deadline (UTC) by which the review should be completed.
    /// 
    /// This value may be null if no explicit deadline is set
    /// for the assignment.
    /// </summary>
    public DateTimeOffset? ReviewDeadlineUtc { get; set; }

    /// <summary>
    /// Indicates whether the reviewer is currently allowed to edit
    /// the review.
    /// 
    /// Typically true when the assignment is in Pending or InProgress
    /// states and false once submitted, declined, or withdrawn.
    /// </summary>
    public bool CanEditReview { get; set; }

    /// <summary>
    /// Indicates whether the reviewer is allowed to submit the review.
    /// 
    /// This is generally true when the assignment is in progress
    /// and before the deadline has passed.
    /// </summary>
    public bool CanSubmitReview { get; set; }

    /// <summary>
    /// Indicates whether the reviewer is allowed to decline
    /// the assignment.
    /// 
    /// Typically only allowed when the assignment is still pending
    /// and has not yet been started.
    /// </summary>
    public bool CanDeclineReview { get; set; }
}