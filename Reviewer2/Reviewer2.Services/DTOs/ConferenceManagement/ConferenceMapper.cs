using System;
using System.Linq;
using Reviewer2.Data.Models;

namespace Reviewer2.Services.DTOs.ConferenceManagement;

/// <summary>
/// Provides extension methods to map Conference domain entities to DTOs.
/// </summary>
public static class ConferenceMapper
{
    /// <summary>
    /// Maps a <see cref="Conference"/> to a <see cref="ConferenceDTO"/>.
    /// Deadlines are sorted descending by priority.
    /// </summary>
    public static ConferenceDTO ToDTO(this Conference model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        return new ConferenceDTO
        {
            Name = model.Name,
            Description = model.Description,
            CallForPapers = model.CallForPapers,
            Deadlines = model.Deadlines?
                .OrderByDescending(d => d.Priority)
                .Select(d => d.ToSummary())
                .ToList() ?? new()
        };
    }

    /// <summary>
    /// Maps a <see cref="Deadline"/> to a <see cref="DeadlineSummaryDTO"/>.
    /// </summary>
    public static DeadlineSummaryDTO ToSummary(this Deadline model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        return new DeadlineSummaryDTO
        {
            Name = model.Name,
            Date = model.Date,
            Priority = model.Priority
        };
    }
}
