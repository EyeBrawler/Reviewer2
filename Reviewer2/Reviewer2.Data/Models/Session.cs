using System;
using System.Collections.Generic;

namespace Reviewer2.Data.Models;

/// <summary>
/// Represents a persistent scheduling assignment for a paper.
/// 
/// This model defines when and where a paper is scheduled
/// within a conference or review session.
/// </summary>
public class Session
{
    /// <summary>
    /// Primary key identifier for the schedule record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Arbitrary name or identifier for the session.
    /// Examples: "Session A", "Room 1", "Morning Block".
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Physical or logical location where the session is held
    /// (e.g., room name, building, or virtual meeting link identifier).
    /// </summary>
    public string Location { get; set; } = string.Empty;
    
    /// <summary>
    /// Collection of papers assigned to this session.
    /// 
    /// Represents a one-to-many relationship between sessions and papers,
    /// used for scheduling and presentation grouping.
    /// </summary>
    public List<Paper> Papers { get; private set; } = [];

    /// <summary>
    /// Start time of the scheduled presentation or review slot.
    /// </summary>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// End time of the scheduled presentation or review slot.
    /// </summary>
    public DateTimeOffset EndTime { get; set; }
}