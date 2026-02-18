using System;

namespace Reviewer2.Data.Models;

public class Review
{
    public Guid Id { get; set; }
    
    public Guid ReviewTemplateId { get; set; }
    
    public ReviewTemplate ReviewTemplate { get; set; } = default!;

    public Guid ReviewAssignmentId { get; set; }
    
    public ReviewAssignment ReviewAssignment { get; set; } = default!;
    
    public DateTime? SubmittedAtUtc { get; set; }

    // Frequently queried structured fields
    public int? OverallScore { get; set; }
    public int? ConfidenceScore { get; set; }

    public string? Recommendation { get; set; }

    // Flexible review content
    public string JsonContent { get; set; } = "{}";
}
