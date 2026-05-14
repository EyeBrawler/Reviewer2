using Reviewer2.Data.Models;

namespace Reviewer2.Services.Submissions.PaperSubmission;

/// <summary>
/// Provides centralized visibility rules for determining when
/// paper-related information may be shown to users based on
/// the current lifecycle state of a paper.
/// 
/// This policy is intended to keep paper access rules
/// consistent across UI components, API endpoints, and
/// service-layer authorization checks.
/// </summary>
public static class PaperVisibilityPolicy
{
    /// <summary>
    /// Determines whether an author may view the anonymous
    /// reviews associated with their paper based on the
    /// paper's current status.
    /// 
    /// Authors may view reviews only after the review
    /// process has reached a stage where reviewer feedback
    /// is intended to be disclosed, such as when revisions
    /// are requested or a final decision has been made.
    /// </summary>
    /// <param name="status">
    /// The current workflow status of the paper.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the author is permitted
    /// to view reviews for the specified status;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool CanAuthorViewReviews(PaperStatus status)
    {
        return status switch
        {
            PaperStatus.RevisionRequired => true,
            PaperStatus.Accepted => true,
            PaperStatus.Rejected => true,
            PaperStatus.CameraReadySubmitted => true,
            PaperStatus.Scheduled => true,
            PaperStatus.Presented => true,

            _ => false
        };
    }
}