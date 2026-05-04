using System;

namespace Reviewer2.Data.Models;

/// <summary>
/// Represents a persistent scheduling assignment for a paper.
/// 
/// This model defines when and where a paper is scheduled
/// within a conference or review session.
/// 
/// It is stored in the database and represents real scheduling state,
/// not UI grouping or temporary drag-and-drop behavior.
/// </summary>
public class PaperSchedule
{
    /// <summary>
    /// Primary key identifier for the schedule record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key referencing the associated paper.
    /// </summary>
    public Guid PaperId { get; set; }
    
    /// <summary>
    /// Navigation property to the associated paper.
    /// </summary>
    public Paper Paper { get; set; } = default!;

    /// <summary>
    /// Identifier for the session or room where the paper is scheduled.
    /// Examples: "Session A", "Room 1", "Morning Block".
    /// </summary>
    public string SessionKey { get; set; } = string.Empty;

    /// <summary>
    /// Start time of the scheduled presentation or review slot.
    /// </summary>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// End time of the scheduled presentation or review slot.
    /// </summary>
    public DateTimeOffset EndTime { get; set; }
}