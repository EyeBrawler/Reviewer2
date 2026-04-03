using System;
using System.Linq;
using System.Collections.Generic;
using Reviewer2.Data.Models;

namespace Reviewer2.Services.DTOs.ConferenceManagement
{
    /// <summary>
    /// Provides extension methods to map between <see cref="Conference"/> entities
    /// and <see cref="ConferenceEditDTO"/> objects.
    /// </summary>
    public static class ConferenceMappingExtensions
    {
        /// <summary>
        /// Converts a <see cref="Conference"/> entity to a <see cref="ConferenceEditDTO"/>.
        /// </summary>
        /// <param name="entity">The conference entity to map.</param>
        /// <returns>A new <see cref="ConferenceEditDTO"/> populated with values from the entity.</returns>
        public static ConferenceEditDTO ToEditDTO(this Conference entity)
        {
            return new ConferenceEditDTO
            {
                Info = new ConferenceEditDTO.ConferenceInfo
                {
                    Name = entity.Name ?? string.Empty,
                    Description = entity.Description ?? string.Empty
                },
                CallForPapers = new ConferenceEditDTO.CallForPapersInfo
                {
                    Content = entity.CallForPapers ?? string.Empty
                },
                Deadlines = entity.Deadlines?.Select(d => new ConferenceEditDTO.DeadlineInfo
                {
                    Id = d.Id,
                    Name = d.Name ?? string.Empty,
                    // Convert DateTimeOffset to DateTime for MudDatePicker
                    Date = d.Date.UtcDateTime,
                    Priority = d.Priority
                }).ToList() ?? new List<ConferenceEditDTO.DeadlineInfo>()
            };
        }

        /// <summary>
        /// Updates a <see cref="Conference"/> entity with values from a <see cref="ConferenceEditDTO"/>.
        /// </summary>
        /// <param name="dto">The source DTO containing updated values.</param>
        /// <param name="entity">The target conference entity to update.</param>
        public static void ToEntity(this ConferenceEditDTO dto, Conference entity)
        {
            entity.Name = dto.Info?.Name ?? string.Empty;
            entity.Description = dto.Info?.Description ?? string.Empty;
            entity.CallForPapers = dto.CallForPapers?.Content ?? string.Empty;

            entity.Deadlines ??= new List<Deadline>();
            entity.Deadlines.Clear();

            if (dto.Deadlines == null) return;

            foreach (var d in dto.Deadlines)
            {
                entity.Deadlines.Add(new Deadline
                {
                    Id = d.Id ?? 0,
                    Name = d.Name ?? string.Empty,
                    // Convert DateTime? back to DateTimeOffset
                    Date = d.Date.HasValue ? new DateTimeOffset(d.Date.Value) : DateTimeOffset.Now,
                    Priority = d.Priority
                });
            }
        }
    }
}