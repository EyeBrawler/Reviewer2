using System;
using System.Collections.Generic;
using Reviewer2.Services.DTOs.PaperSubmission;

/// <summary>
/// Represents a session along with fully resolved paper data for UI display.
/// 
/// This DTO is used when loading scheduling data into the UI. Unlike
/// <see cref="SessionDTO"/>, it includes full paper metadata for rendering
/// cards, tables, and drag-and-drop interfaces.
/// </summary>
public class SessionWithPapersDTO
{
    /// <summary>
    /// Unique identifier of the session.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Display name of the session.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Location or room where the session takes place.
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
    /// Fully populated list of papers assigned to this session.
    /// These are enriched DTOs used for UI rendering.
    /// </summary>
    public List<UserPaperDTO> Papers { get; set; } = new();
}