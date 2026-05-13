using System;
using System.Linq;
using Reviewer2.Data.Models;
using Reviewer2.Services.DTOs.PaperSubmission;

namespace Reviewer2.Services.DTOs.ReviewAssignments
{
    /// <summary>
    /// Provides extension methods for mapping <see cref="ReviewAssignment"/>
    /// entities to <see cref="ReviewerPaperDTO"/> objects.
    /// </summary>
    public static class ReviewAssignmentMapper
    {
        /// <summary>
        /// Maps a <see cref="ReviewAssignment"/> to a <see cref="ReviewerPaperDTO"/>
        /// suitable for reviewer dashboards.
        /// </summary>
        /// <param name="assignment">The review assignment to map.</param>
        /// <returns>A fully populated <see cref="ReviewerPaperDTO"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assignment"/> or its <see cref="ReviewAssignment.Paper"/> is null.</exception>
        public static ReviewerPaperDTO ToReviewerPaperDTO(this ReviewAssignment assignment)
        {
            ArgumentNullException.ThrowIfNull(assignment);
            if (assignment.Paper == null)
                throw new ArgumentNullException(nameof(assignment.Paper));

            var paper = assignment.Paper;

            return new ReviewerPaperDTO
            {
                PaperId = paper.Id,
                Title = paper.Title,
                PaperStatus = paper.Status.ToString(),
                SubmittedAtUtc = paper.SubmittedAtUtc,
                Authors = string.Join(", ", paper.Authors
                    .OrderBy(a => a.AuthorOrder)
                    .Select(a =>
                    {
                        var name = a.User != null
                            ? $"{a.User.FirstName} {a.User.LastName}"
                            : $"{a.FirstName} {a.LastName}";

                        var role = a.IsCorrespondingAuthor ? "Corresponding" :
                                   a.IsPresenter ? "Presenter" : "";

                        return string.IsNullOrEmpty(role) ? name : $"{name} ({role})";
                    })),
                ReviewerId = assignment.ReviewerId,
                ReviewerDisplayName = assignment.Reviewer.FullName,
                Files = paper.Files.Select(f => new PaperFileSummaryDTO
                {
                    FileId = f.Id,
                    FileName = f.OriginalFileName,
                    FileType = f.Type.ToString(),
                    FileUrl = $"/api/papers/{paper.Id}/files/{f.Type}"
                }).ToList(),

                ReviewAssignmentId = assignment.Id,
                ReviewStatus = assignment.Status,
                ReviewSubmittedAtUtc = assignment.Review?.SubmittedAtUtc,

                CanEditReview = assignment.Status is ReviewStatus.Pending or ReviewStatus.InProgress,
                CanSubmitReview = assignment.Status is ReviewStatus.Pending or ReviewStatus.InProgress,
                CanDeclineReview = assignment.Status == ReviewStatus.Pending
            };
        }
    }
}