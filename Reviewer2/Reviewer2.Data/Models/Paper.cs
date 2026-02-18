using System;
using System.Collections.Generic;

namespace Reviewer2.Data.Models;

/// <summary>
/// Represents a paper submission within a conference.
/// 
/// A Paper is the central aggregate root of the submission and review workflow.
/// It maintains relationships to authors, review assignments, uploaded files,
/// and decision metadata.
/// 
/// The paper progresses through a defined lifecycle represented by <see cref="PaperStatus"/>.
/// </summary>
public class Paper
{
    /// <summary>
    /// Unique identifier for the paper.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identifier of the conference to which this paper was submitted.
    /// 
    /// Although the current system supports a single conference,
    /// this property ensures future extensibility.
    /// </summary>
    public Guid ConferenceId { get; set; }

    /// <summary>
    /// The official title of the paper as provided by the submitter.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The abstract summarizing the paper’s content.
    /// 
    /// Depending on the conference workflow, this may be submitted
    /// before the full paper file.
    /// </summary>
    public string Abstract { get; set; } = string.Empty;

    /// <summary>
    /// Represents the current lifecycle state of the paper.
    /// 
    /// The status determines what actions are permitted, such as editing,
    /// reviewing, decision-making, or scheduling.
    /// </summary>
    public PaperStatus Status { get; set; }

    /// <summary>
    /// The date and time (UTC) when the paper was officially submitted.
    /// 
    /// This value is typically set when transitioning from Draft
    /// to Submitted.
    /// </summary>
    public DateTime SubmittedAtUtc { get; set; }
    
    /// <summary>
    /// The identifier of the user who originally submitted the paper.
    /// 
    /// The submitter is responsible for managing the submission,
    /// uploading revisions, and receiving system notifications.
    /// </summary>
    public string SubmitterUserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the submitting user.
    /// </summary>
    public ApplicationUser Submitter { get; set; } = default!;
    
    /// <summary>
    /// The date and time (UTC) when an acceptance or rejection decision
    /// was made by the conference leadership.
    /// 
    /// This value is null until a final decision is recorded.
    /// </summary>
    public DateTime? DecisionMadeAtUtc { get; set; }

    /// <summary>
    /// The identifier of the user (typically a ConferenceChair)
    /// who made the final decision on the paper.
    /// </summary>
    public string? DecisionMadeByUserId { get; set; }

    /// <summary>
    /// Optional comments associated with the final decision.
    /// 
    /// This may contain summary remarks, justification, or
    /// additional instructions for the authors.
    /// </summary>
    public string? DecisionComment { get; set; }

    /// <summary>
    /// Collection of authors associated with the paper.
    /// 
    /// Author order is significant and preserved via Author.AuthorOrder.
    /// </summary>
    public List<Author> Authors { get; set; } = new();

    /// <summary>
    /// Collection of reviewer assignments for this paper.
    /// 
    /// Each assignment represents a reviewer-paper relationship
    /// and may contain an associated review.
    /// </summary>
    public List<ReviewAssignment> ReviewAssignments { get; set; } = new();

    /// <summary>
    /// Collection of uploaded files related to the paper submission,
    /// such as initial submission, camera-ready version, or copyright form.
    /// </summary>
    public List<PaperFile> Files { get; set; } = new();
}

/// <summary>
/// Represents the lifecycle state of a paper submission.
/// 
/// Status transitions are controlled by the submission and review workflow.
/// Certain actions are only permitted in specific states.
/// </summary>
public enum PaperStatus
{
    /// <summary>
    /// The paper has been created but not officially submitted.
    /// Authors may edit metadata and upload files.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Only the abstract has been submitted (if the conference
    /// supports a two-phase submission process).
    /// </summary>
    AbstractSubmitted = 1,

    /// <summary>
    /// The full paper has been officially submitted and is awaiting review assignment.
    /// </summary>
    Submitted = 2,

    /// <summary>
    /// The paper has active reviewer assignments and is currently under review.
    /// </summary>
    UnderReview = 3,

    /// <summary>
    /// The minimum required number of reviews has been submitted.
    /// The paper is awaiting a decision.
    /// </summary>
    ReviewsCompleted = 4,

    /// <summary>
    /// The paper has been accepted for presentation or publication.
    /// </summary>
    Accepted = 5,

    /// <summary>
    /// The paper has been rejected.
    /// </summary>
    Rejected = 6,

    /// <summary>
    /// The paper has been withdrawn by the submitter prior to final decision.
    /// </summary>
    Withdrawn = 7,

    /// <summary>
    /// The final camera-ready version has been submitted following acceptance.
    /// </summary>
    CameraReadySubmitted = 8,

    /// <summary>
    /// The paper has been assigned to a conference session.
    /// </summary>
    Scheduled = 9,

    /// <summary>
    /// The paper has been presented at the conference.
    /// </summary>
    Presented = 10
}