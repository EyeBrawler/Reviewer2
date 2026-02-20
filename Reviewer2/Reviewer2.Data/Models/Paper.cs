using System;
using System.Collections.Generic;
using System.Linq;

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
    public PaperStatus Status { get; private set; } = PaperStatus.Draft;

    /// <summary>
    /// The date and time (UTC) when the paper was officially submitted.
    /// 
    /// This value is typically set when transitioning from Draft
    /// to Submitted.
    /// </summary>
    public DateTime? SubmittedAtUtc { get; private set; }
    
    /// <summary>
    /// The identifier of the user who originally submitted the paper.
    /// 
    /// The submitter is responsible for managing the submission,
    /// uploading revisions, and receiving system notifications.
    /// </summary>
    public Guid SubmitterUserId { get; set; }

    /// <summary>
    /// Navigation property to the submitting user.
    /// </summary>
    public ApplicationUser Submitter { get; set; } = null!;
    
    /// <summary>
    /// The date and time (UTC) when an acceptance or rejection decision
    /// was made by the conference leadership.
    /// 
    /// This value is null until a final decision is recorded.
    /// </summary>
    public DateTime? DecisionMadeAtUtc { get; private set; }

    /// <summary>
    /// The identifier of the user (typically a ConferenceChair)
    /// who made the final decision on the paper.
    /// </summary>
    public Guid? DecisionMadeByUserId { get; private set; }

    /// <summary>
    /// Comments associated with the final decision.
    /// 
    /// This may contain summary remarks, justification, or
    /// additional instructions for the authors.
    /// </summary>
    public string? DecisionComment { get; private set; }

    /// <summary>
    /// Collection of authors associated with the paper.
    /// 
    /// Author order is significant and preserved via Author.AuthorOrder.
    /// </summary>
    public List<Author> Authors { get; private set; } = [];

    /// <summary>
    /// Collection of reviewer assignments for this paper.
    /// 
    /// Each assignment represents a reviewer-paper relationship
    /// and may contain an associated review.
    /// </summary>
    public List<ReviewAssignment> ReviewAssignments { get; private set; } = [];

    /// <summary>
    /// Collection of uploaded files related to the paper submission,
    /// such as initial submission, camera-ready version, or copyright form.
    /// </summary>
    public List<PaperFile> Files { get; private set; } = [];
    
    /// <summary>
    /// Transitions the paper from <see cref="PaperStatus.Draft"/> 
    /// to <see cref="PaperStatus.Submitted"/>.
    /// 
    /// This operation validates that all required submission criteria are met,
    /// including metadata, author invariants, and the presence of an initial
    /// submission file.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the paper is not in Draft state or if required submission
    /// invariants are not satisfied.
    /// </exception>
    public void Submit()
    {
        if (Status != PaperStatus.Draft)
            throw new InvalidOperationException("Only drafts can be submitted.");

        if (string.IsNullOrWhiteSpace(Title))
            throw new InvalidOperationException("Title is required.");

        if (string.IsNullOrWhiteSpace(Abstract))
            throw new InvalidOperationException("Abstract is required.");

        ValidateAuthorRules();

        if (Files.All(f => f.Type != FileType.InitialSubmission))
            throw new InvalidOperationException("Initial submission file is required.");

        Status = PaperStatus.Submitted;
        SubmittedAtUtc = DateTime.UtcNow;
    }


    /// <summary>
    /// Transitions the paper from <see cref="PaperStatus.Submitted"/> 
    /// to <see cref="PaperStatus.UnderReview"/>.
    /// 
    /// This indicates that reviewer assignments may now actively evaluate
    /// the submission.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the paper is not currently in Submitted state.
    /// </exception>
    public void MoveToUnderReview()
    {
        if (Status != PaperStatus.Submitted)
            throw new InvalidOperationException("Paper must be submitted before review.");

        Status = PaperStatus.UnderReview;
    }
    
    /// <summary>
    /// Transitions the paper from <see cref="PaperStatus.UnderReview"/> 
    /// to <see cref="PaperStatus.ReviewsCompleted"/>.
    /// 
    /// This indicates that the required number of reviews has been received
    /// and the paper is ready for final decision.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the paper is not currently under review.
    /// </exception>
    public void MarkReviewsCompleted()
    {
        if (Status != PaperStatus.UnderReview)
            throw new InvalidOperationException("Paper must be under review.");

        Status = PaperStatus.ReviewsCompleted;
    }
    
    /// <summary>
    /// Records an acceptance decision for the paper and transitions it to 
    /// <see cref="PaperStatus.Accepted"/>.
    /// 
    /// This sets the decision metadata, including timestamp, decision maker,
    /// and optional comment.
    /// </summary>
    /// <param name="chairUserId">
    /// The identifier of the conference chair (or authorized decision-maker)
    /// recording the acceptance.
    /// </param>
    /// <param name="comment">
    /// Optional decision remarks or instructions for the authors.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the paper is not in ReviewsCompleted state.
    /// </exception>
    public void Accept(Guid chairUserId, string? comment)
    {
        if (Status != PaperStatus.ReviewsCompleted)
            throw new InvalidOperationException("Paper must have completed reviews.");

        Status = PaperStatus.Accepted;
        DecisionMadeAtUtc = DateTime.UtcNow;
        DecisionMadeByUserId = chairUserId;
        DecisionComment = comment;
    }
    
    /// <summary>
    /// Records a rejection decision for the paper and transitions it to 
    /// <see cref="PaperStatus.Rejected"/>.
    /// 
    /// This sets the decision metadata, including timestamp, decision maker,
    /// and optional comment.
    /// </summary>
    /// <param name="chairUserId">
    /// The identifier of the conference chair (or authorized decision-maker)
    /// recording the rejection.
    /// </param>
    /// <param name="comment">
    /// Optional decision remarks explaining the rejection.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the paper is not in ReviewsCompleted state.
    /// </exception>
    public void Reject(Guid chairUserId, string? comment)
    {
        if (Status != PaperStatus.ReviewsCompleted)
            throw new InvalidOperationException("Paper must have completed reviews.");

        Status = PaperStatus.Rejected;
        DecisionMadeAtUtc = DateTime.UtcNow;
        DecisionMadeByUserId = chairUserId;
        DecisionComment = comment;
    }

    /// <summary>
    /// Transitions the paper from <see cref="PaperStatus.Accepted"/> 
    /// to <see cref="PaperStatus.CameraReadySubmitted"/>.
    /// 
    /// This requires that a camera-ready file has been uploaded.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the paper is not accepted or if no camera-ready file exists.
    /// </exception>
    public void SubmitCameraReady()
    {
        if (Status != PaperStatus.Accepted)
            throw new InvalidOperationException("Only accepted papers can submit camera-ready versions.");

        if (Files.All(f => f.Type != FileType.CameraReady))
            throw new InvalidOperationException("Camera-ready file is required.");

        Status = PaperStatus.CameraReadySubmitted;
    }

    /// <summary>
    /// Transitions the paper from <see cref="PaperStatus.CameraReadySubmitted"/> 
    /// to <see cref="PaperStatus.Scheduled"/>.
    /// 
    /// Indicates that the paper has been assigned to a conference session.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the camera-ready version has not yet been submitted.
    /// </exception>
    public void Schedule()
    {
        if (Status != PaperStatus.CameraReadySubmitted)
            throw new InvalidOperationException("Camera-ready version must be submitted first.");

        Status = PaperStatus.Scheduled;
    }

    /// <summary>
    /// Transitions the paper from <see cref="PaperStatus.Scheduled"/> 
    /// to <see cref="PaperStatus.Presented"/>.
    /// 
    /// Indicates that the paper has been formally presented at the conference.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the paper has not been scheduled.
    /// </exception>
    public void MarkPresented()
    {
        if (Status != PaperStatus.Scheduled)
            throw new InvalidOperationException("Paper must be scheduled first.");

        Status = PaperStatus.Presented;
    }
    
    /// <summary>
    /// Withdraws the paper from the submission process and transitions it to 
    /// <see cref="PaperStatus.Withdrawn"/>.
    /// 
    /// Withdrawal is permitted only before a final decision has been recorded.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the paper has already received a final decision
    /// or progressed beyond that point in the workflow.
    /// </exception>
    public void Withdraw()
    {
        // Any state at or beyond Accepted is considered final/post-decision.
        if (Status >= PaperStatus.Accepted)
            throw new InvalidOperationException("Cannot withdraw after a final decision has been recorded.");

        Status = PaperStatus.Withdrawn;
    }
    
    /// <summary>
    /// Replaces the entire author collection for a draft paper.
    /// 
    /// Author order is preserved according to the order provided.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the paper is not in Draft state.
    /// </exception>
    public void ReplaceAuthors(IEnumerable<Author> authors)
    {
        if (Status != PaperStatus.Draft)
            throw new InvalidOperationException("Authors can only be modified while in Draft state.");

        Authors.Clear();

        int order = 0;
        foreach (var author in authors)
        {
            author.AuthorOrder = order++;
            Authors.Add(author);
        }

        ValidateAuthorRules();
    }
    
    private void ValidateAuthorRules()
    {
        if (!Authors.Any())
            throw new InvalidOperationException("At least one author is required.");

        if (Authors.Count(a => a.IsCorrespondingAuthor) != 1)
            throw new InvalidOperationException("Exactly one corresponding author is required.");

        if (Authors.Count(a => a.IsPresenter) > 1)
            throw new InvalidOperationException("Only one presenter is allowed.");
    }
    
    /// <summary>
    /// Updates the paper's title and abstract while in 
    /// <see cref="PaperStatus.Draft"/> state.
    /// 
    /// Metadata modifications are restricted to drafts to ensure
    /// submission integrity once the paper has entered the review workflow.
    /// </summary>
    /// <param name="title">
    /// The updated title of the paper.
    /// </param>
    /// <param name="abstract">
    /// The updated abstract describing the paper.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the paper is not currently in Draft state.
    /// </exception>
    public void UpdateMetadata(string title, string @abstract)
    {
        if (Status != PaperStatus.Draft)
            throw new InvalidOperationException("Metadata can only be modified while in Draft state.");

        Title = title;
        Abstract = @abstract;
    }
    
    /// <summary>
    /// Replaces an existing file of the same <see cref="PaperFile.Type"/>
    /// with the provided file.
    /// 
    /// If a file of the same type already exists, it is removed before
    /// the new file is added. This ensures that only one file per type
    /// is associated with the paper at any time.
    /// </summary>
    /// <param name="file">
    /// The new <see cref="PaperFile"/> instance to associate with the paper.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="file"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if file modifications are not permitted in the current state.
    /// </exception>
    public void ReplaceFile(PaperFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        // No modifications allowed once the paper has been presented.
        if (Status >= PaperStatus.Presented)
            throw new InvalidOperationException("Files cannot be modified after presentation.");

        Files.RemoveAll(f => f.Type == file.Type);
        Files.Add(file);
    }
    
    /// <summary>
    /// Creates a new <see cref="Paper"/> in the <see cref="PaperStatus.Draft"/> state
    /// using the provided metadata and a collection of <see cref="Author"/> entities.
    /// </summary>
    /// <param name="submitterUserId">
    /// The unique identifier of the user submitting the draft. Must not be <see cref="Guid.Empty"/>.
    /// </param>
    /// <param name="title">
    /// The title of the paper. Cannot be null, empty, or whitespace.
    /// </param>
    /// <param name="abstractText">
    /// The abstract text summarizing the paper's content. Cannot be null, empty, or whitespace.
    /// </param>
    /// <param name="authors">
    /// A collection of <see cref="Author"/> entities representing the paper's authors.
    /// Must contain at least one author, with exactly one corresponding author and at most one presenter.
    /// </param>
    /// <returns>
    /// A new <see cref="Paper"/> instance initialized in Draft state, including the provided authors.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="submitterUserId"/> is <see cref="Guid.Empty"/>,
    /// if <paramref name="title"/> or <paramref name="abstractText"/> are null or whitespace,
    /// or if <paramref name="authors"/> is null or empty.
    /// </exception>
    /// <remarks>
    /// This method enforces author-related business rules via <see cref="Paper.ValidateAuthorRules"/>.
    /// The returned paper is ready to be added to the persistence context for saving.
    /// </remarks>
    public static Paper CreateDraft(
        Guid submitterUserId,
        string title,
        string abstractText,
        IEnumerable<Author> authors)
    {
        if (submitterUserId == Guid.Empty)
            throw new ArgumentException("SubmitterUserId cannot be empty.", nameof(submitterUserId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(abstractText))
            throw new ArgumentException("Abstract is required.", nameof(abstractText));

        if (authors == null || !authors.Any())
            throw new ArgumentException("At least one author is required.", nameof(authors));

        var draftPaper = new Paper
        {
            Id = Guid.NewGuid(),
            SubmitterUserId = submitterUserId,
            Title = title,
            Abstract = abstractText,
            Status = PaperStatus.Draft,
            Authors = authors.ToList()
        };

        draftPaper.ValidateAuthorRules();

        return draftPaper;
    }
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