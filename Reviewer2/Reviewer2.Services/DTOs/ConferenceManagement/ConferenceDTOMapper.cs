using System;
using System.Linq;
using Reviewer2.Data.Models;

namespace Reviewer2.Services.DTOs.ConferenceManagement;

/// <summary>
/// Provides extension methods to map Conference domain entities to DTOs.
/// </summary>
public static class ConferenceDTOMapper
{
    /// <summary>
    /// Maps a <see cref="Conference"/> to a <see cref="ConferenceDTO"/>.
    /// Deadlines are sorted descending by priority.
    /// </summary>
    public static ConferenceDTO ToDTO(this Conference entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        return new ConferenceDTO
        {
            Name = entity.Name,
            Description = entity.Description,
            CallForPapers = entity.CallForPapers,
            Deadlines = entity.Deadlines?
                .OrderByDescending(d => d.Priority)
                .Select(d => d.ToSummary())
                .ToList() ?? new()
        };
    }

    /// <summary>
    /// Maps a <see cref="Deadline"/> to a <see cref="DeadlineSummaryDTO"/>.
    /// </summary>
    public static DeadlineSummaryDTO ToSummary(this Deadline entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        return new DeadlineSummaryDTO
        {
            Name = entity.Name,
            Date = entity.Date,
            Priority = entity.Priority
        };
    }
}
