using System.Collections.Generic;

namespace Reviewer2.Services.DTOs.ConferenceManagement;

/// <summary>
/// Represents a simplified view of a conference for display or API purposes.
/// Contains core descriptive fields and associated deadlines.
/// </summary>
public class ConferenceSummary
{
    /// <summary>
    /// Gets or sets the official name of the conference.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets the description of the conference, including scope, purpose, or theme.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the call-for-papers (CFP) text providing submission guidelines for authors.
    /// </summary>
    public string? CallForPapers { get; set; }

    /// <summary>
    /// Gets or sets the collection of deadlines associated with the conference.
    /// </summary>
    public List<DeadlineSummary> Deadlines { get; set; } = new();
}