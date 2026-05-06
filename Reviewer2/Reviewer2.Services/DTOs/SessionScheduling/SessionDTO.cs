using System;
using System.Collections.Generic;

/// <summary>
/// Data transfer object used to create or update a session in the scheduling system.
/// 
/// This DTO is sent from the UI to the backend when saving drag-and-drop
/// scheduling changes. It contains only lightweight identifiers for papers
/// rather than full paper objects.
/// </summary>
public class SessionDTO
{
    /// <summary>
    /// Optional unique identifier for the session.
    /// If null, a new session will be created.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Display name of the session (e.g., "Session A", "Morning Block").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Physical or logical location of the session (e.g., room name or track).
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Start time of the session.
    /// </summary>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// End time of the session.
    /// </summary>
    public DateTimeOffset EndTime { get; set; }

    /// <summary>
    /// List of paper identifiers assigned to this session.
    /// These are mapped to full Paper entities in the service layer.
    /// </summary>
    public List<Guid> PaperIds { get; set; } = new();
}