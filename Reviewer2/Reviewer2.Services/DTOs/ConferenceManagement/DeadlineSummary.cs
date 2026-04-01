using System;

namespace Reviewer2.Services.DTOs.ConferenceManagement;

/// <summary>
/// Represents a simplified view of a conference deadline.
/// Used for displaying or transferring deadline information to the front-end.
/// </summary>
public class DeadlineSummary
{
    /// <summary>
    /// Gets or sets the name of the deadline, e.g., "Submission Deadline" or "Notification Date".
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of the deadline in UTC.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the priority of the deadline.
    /// Higher values indicate more important deadlines.
    /// </summary>
    public int Priority { get; set; }
}