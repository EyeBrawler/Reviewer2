using System;
using System.Collections.Generic;

namespace Reviewer2.Data.Models;

public class Paper
{
    public Guid Id { get; set; }

    public Guid ConferenceId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;

    public PaperStatus Status { get; set; }

    public DateTime SubmittedAtUtc { get; set; }
    
    public string SubmitterUserId { get; set; } = string.Empty;
    public ApplicationUser Submitter { get; set; } = default!;
    
    public DateTime? DecisionMadeAtUtc { get; set; }
    public string? DecisionMadeByUserId { get; set; }
    public string? DecisionComment { get; set; }

    public List<Author> Authors { get; set; } = new();
    public List<ReviewAssignment> ReviewAssignments { get; set; } = new();
    public List<PaperFile> Files { get; set; } = new();
}

public enum PaperStatus
{
    Draft = 0,                 // Author started but not submitted
    AbstractSubmitted = 1,     // Abstract-only phase (optional)
    Submitted = 2,             // Full paper submitted
    UnderReview = 3,           // Review assignments active
    ReviewsCompleted = 4,      // Min reviews satisfied
    Accepted = 5,              // Decision: accept
    Rejected = 6,              // Decision: reject
    Withdrawn = 7,             // Author withdrew
    CameraReadySubmitted = 8,  // Final version uploaded
    Scheduled = 9,             // Placed in session
    Presented = 10             // Talk given
}

