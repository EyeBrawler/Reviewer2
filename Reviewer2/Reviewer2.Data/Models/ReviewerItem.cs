using System;

namespace Reviewer2.Data.Models;

/// <summary>
/// Represents a lightweight UI model for a reviewer used in scheduling and drag-and-drop operations.
/// </summary>
public class ReviewerItem
{
    /// <summary>
    /// Unique identifier of the reviewer.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Display name of the reviewer.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Track how many papers are currently assigned to this reviewer.
    /// </summary>
    public int CurrentAssignmentCount { get; set; }
}