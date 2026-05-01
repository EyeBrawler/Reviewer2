using System;
using System.Collections.Generic;
using Reviewer2.Data.Models;

namespace Reviewer2.Data.Models;

/// <summary>
/// UI-only container representing a scheduling time slot.
/// 
/// This model is used exclusively for Blazor UI rendering and drag-and-drop behavior.
/// It is NOT persisted to the database and does NOT represent domain state.
/// </summary>
public class ScheduleBin
{
    /// <summary>
    /// Internal key used for UI identification and drag-and-drop routing.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable label displayed in the UI.
    /// Example: "10:00 AM", "Session A", "Morning Slot".
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Start time of this scheduling slot.
    /// </summary>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// End time of this scheduling slot.
    /// </summary>
    public DateTimeOffset EndTime { get; set; }

    /// <summary>
    /// Offset in minutes used for spacing or ordering logic.
    /// </summary>
    public int OffsetMinutes { get; set; }

    /// <summary>
    /// Collection of papers currently assigned to this time slot.
    /// This represents UI state only and is not persisted directly.
    /// </summary>
    public List<Paper> Papers { get; set; } = new();
}