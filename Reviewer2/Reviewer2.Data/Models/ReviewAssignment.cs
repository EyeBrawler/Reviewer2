using System;

namespace Reviewer2.Data.Models;

public class ReviewAssignment
{
    public Guid Id { get; set; }

    public Guid PaperId { get; set; }
    public Guid ReviewerId { get; set; }
    public ApplicationUser Reviewer { get; set; } = default!;
    
    public ReviewStatus Status { get; set; }

    public Review? Review { get; set; }
}

public enum ReviewStatus
{
    Pending = 0,          // Assigned but not started
    InProgress = 1,       // Reviewer has begun filling form
    Submitted = 2,        // Review completed
    Declined = 3,         // Reviewer declined assignment
    Withdrawn = 4,        // Chair removed assignment
    Overdue = 5           // Deadline passed without submission
}