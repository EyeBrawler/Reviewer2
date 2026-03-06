using System;
using System.Threading.Tasks;

namespace Reviewer2.Services.ReviewAssignments;

public interface IReviewAssignmentService
{
    Task<ReviewAssignmentBoardDto> GetAssignmentBoardAsync();

    Task AssignReviewerAsync(Guid paperId, Guid reviewerId);

    Task RemoveReviewerAsync(Guid assignmentId);
}