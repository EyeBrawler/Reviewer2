using System;
using System.Collections.Generic;
using System.Linq;
using Reviewer2.Data.Models;

namespace Reviewer2.Services.DTOs.ConferenceManagement
{
    /// <summary>
    /// Extension methods for mapping between Conference entities and ConferenceEditDTO.
    /// </summary>
    public static class ConferenceEditMapper
    {
        /// <summary>
        /// Converts a <see cref="Conference"/> entity to a <see cref="ConferenceEditDTO"/>.
        /// </summary>
        /// <param name="conference">The conference entity.</param>
        /// <returns>A populated <see cref="ConferenceEditDTO"/>.</returns>
        public static ConferenceEditDTO ToEditDTO(this Conference conference)
        {
            return new ConferenceEditDTO
            {
                Info = new ConferenceEditDTO.ConferenceInfo
                {
                    Name = conference.Name ?? string.Empty,
                    Description = conference.Description ?? string.Empty
                },
                CallForPapers = new ConferenceEditDTO.CallForPapersInfo
                {
                    Content = conference.CallForPapers ?? string.Empty
                },
                Deadlines = conference.Deadlines?
                    .OrderByDescending(d => d.Priority)
                    .Select(d => new ConferenceEditDTO.DeadlineInfo
                    {
                        Name = d.Name ?? string.Empty,
                        Date = d.Date.UtcDateTime, // DateTimeOffset -> DateTime
                        Priority = d.Priority
                    })
                    .ToList()
                    ?? new List<ConferenceEditDTO.DeadlineInfo>()
            };
        }

        /// <summary>
        /// Updates a <see cref="Conference"/> entity using values from a <see cref="ConferenceEditDTO"/>.
        /// </summary>
        /// <param name="dto">The source DTO.</param>
        /// <param name="conference">The target entity to update.</param>
        public static void ToEntity(this ConferenceEditDTO dto, Conference conference)
        {
            UpdateConferenceInfo(dto, conference);
            UpdateCallForPapers(dto, conference);
            UpdateDeadlines(dto, conference);
        }

        /// <summary>
        /// Updates basic conference information.
        /// </summary>
        private static void UpdateConferenceInfo(ConferenceEditDTO dto, Conference conference)
        {
            conference.Name = dto.Info?.Name ?? string.Empty;
            conference.Description = dto.Info?.Description ?? string.Empty;
        }

        /// <summary>
        /// Updates call for papers content.
        /// </summary>
        private static void UpdateCallForPapers(ConferenceEditDTO dto, Conference conference)
        {
            conference.CallForPapers = dto.CallForPapers?.Content ?? string.Empty;
        }

        /// <summary>
        /// Updates deadlines by replacing the existing collection.
        /// </summary>
        private static void UpdateDeadlines(ConferenceEditDTO dto, Conference conference)
        {
            conference.Deadlines ??= new List<Deadline>();
            conference.Deadlines.Clear();

            if (dto.Deadlines == null)
                return;

            foreach (var d in dto.Deadlines)
            {
                conference.Deadlines.Add(new Deadline
                {
                    Name = d.Name ?? string.Empty,
                    Date = new DateTimeOffset(d.Date), // DateTime -> DateTimeOffset
                    Priority = d.Priority
                });
            }
        }
    }
}