using System.Collections.Generic;

namespace Reviewer2.Data.Models;

/// <summary>
/// Represents an academic conference within the Reviewer2 system.
/// Stores core descriptive information along with extensible metadata
/// and associated important dates.
/// </summary>
public class Conference
{
    /// <summary>
    /// Gets or sets the unique identifier for the conference.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the official name of the conference.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a general description of the conference,
    /// including its purpose, scope, or theme.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the call for papers (CFP) text, which provides
    /// submission guidelines and information for prospective authors.
    /// </summary>
    public string? CallForPapers { get; set; }

    /// <summary>
    /// Gets or sets additional extensible metadata for the conference,
    /// stored as a JSON string. This field allows new properties to be
    /// introduced without requiring database schema changes.
    /// </summary>
    public string? JsonData { get; set; }

    /// <summary>
    /// Gets or sets the collection of important deadlines associated
    /// with the conference, such as submission, review, and notification dates.
    /// </summary>
    public List<Deadline> Deadlines { get; set; } = new();
}