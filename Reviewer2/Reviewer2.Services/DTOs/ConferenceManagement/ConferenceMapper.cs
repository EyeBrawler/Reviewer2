using System;
using System.Linq;
using Reviewer2.Data.Models;

namespace Reviewer2.Services.DTOs.ConferenceManagement;

/// <summary>
/// Provides extension methods to map between Conference domain entities and their DTO representations.
/// </summary>
public static class ConferenceMapper
{
    /// <summary>
    /// Maps a <see cref="Conference"/> entity to a <see cref="ConferenceSummary"/> DTO.
    /// Deadlines are mapped and sorted by descending priority.
    /// </summary>
    /// <param name="model">The <see cref="Conference"/> entity to map.</param>
    /// <returns>A <see cref="ConferenceSummary"/> DTO containing core conference information and deadlines.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="model"/> is null.</exception>
    public static ConferenceSummary ToSummary(this Conference model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new ConferenceSummary
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
    /// Maps a <see cref="Deadline"/> entity to a <see cref="DeadlineSummary"/> DTO.
    /// </summary>
    /// <param name="model">The <see cref="Deadline"/> entity to map.</param>
    /// <returns>A <see cref="DeadlineSummary"/> DTO with name, date, and priority.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="model"/> is null.</exception>
    public static DeadlineSummary ToSummary(this Deadline model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new DeadlineSummary
        {
            Name = model.Name,
            Date = model.Date.UtcDateTime,
            Priority = model.Priority
        };
    }
}