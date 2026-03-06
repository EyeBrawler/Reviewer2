using System;
using System.Threading.Tasks;
using Reviewer2.Data.Models;

namespace Reviewer2.Services.ReviewAssignments;

public class ReviewAssignmentService
{
    public async Task AssignReviewerAsync(Guid paperId, Guid reviewerId)
    {
        if (await ReviewerIsAuthor(paperId, reviewerId))
            throw new InvalidOperationException(
                "Authors cannot review their own paper.");

        bool exists = await _context.ReviewAssignments
            .AnyAsync(a => a.PaperId == paperId && a.ReviewerId == reviewerId);

        if (exists)
            return;

        var assignment = new ReviewAssignment
        {
            Id = Guid.NewGuid(),
            PaperId = paperId,
            ReviewerId = reviewerId,
            Status = ReviewStatus.Pending
        };

        _context.ReviewAssignments.Add(assignment);

        await _context.SaveChangesAsync();
    }
    
    private async Task<bool> ReviewerIsAuthor(Guid paperId, Guid reviewerId)
    {
        return await _context.Authors
            .AnyAsync(a => a.PaperId == paperId && a.UserId == reviewerId);
    }
    
    public async Task RemoveReviewerAsync(Guid assignmentId)
    {
        var assignment = await _context.ReviewAssignments
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment == null)
            return;

        _context.ReviewAssignments.Remove(assignment);

        await _context.SaveChangesAsync();
    }
}