using System.Collections.Generic;

namespace Reviewer2.Data.Models;

/// <summary>
/// UI-only container used to group papers by workflow status
/// in the reviewer scheduling interface.
/// 
/// This model supports drag-and-drop behavior and visual grouping
/// (e.g., Accepted, Needs Review, In Progress).
/// 
/// It is NOT persisted to the database and does NOT represent domain state.
/// </summary>
public class ReviewerBin
{
    /// <summary>
    /// Internal key used for drag-and-drop routing and UI logic.
    /// This value is not displayed to the user.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable label displayed in the UI.
    /// Examples: "Accepted", "Needs Review", "In Progress".
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Collection of papers currently displayed in this bin.
    /// This is a UI projection and does not represent persistent state.
    /// </summary>
    public List<Paper> Papers { get; set; } = new();

    /// <summary>
    /// Initializes a new empty instance of the ReviewerBin class.
    /// </summary>
    public ReviewerBin() { }

    /// <summary>
    /// Initializes a new instance of the ReviewerBin class with key and label.
    /// </summary>
    /// <param name="key">Internal identifier used for UI routing and logic.</param>
    /// <param name="label">Display name shown in the UI.</param>
    public ReviewerBin(string key, string label)
    {
        Key = key;
        Label = label;
    }
}