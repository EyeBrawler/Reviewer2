using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
using Reviewer2.Data.Models;
using Reviewer2.Services.DTOs.PaperSubmission;
using Reviewer2.Services.Submissions.PaperSubmission;

/// <summary>
/// Service responsible for managing session scheduling for conference review.
/// 
/// This service handles persistence of drag-and-drop scheduling data from the UI
/// and retrieval of sessions with their associated papers for display.
/// </summary>
public class SessionService : ISessionService
{
    private readonly IDbContextFactory<ApplicationContext> _contextFactory;
    private readonly IPaperQueryService _paperQueryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionService"/> class.
    /// </summary>
    /// <param name="contextFactory">
    /// Factory used to create database contexts for session persistence operations.
    /// </param>
    /// <param name="paperQueryService">
    /// Service used to retrieve enriched paper DTOs for session display.
    /// </param>
    public SessionService(
        IDbContextFactory<ApplicationContext> contextFactory,
        IPaperQueryService paperQueryService)
    {
        _contextFactory = contextFactory;
        _paperQueryService = paperQueryService;
    }

    // =========================
    // SAVE
    // =========================

    /// <summary>
    /// Saves the full session scheduling state from the UI.
    /// 
    /// This method treats the provided list as the authoritative source of truth:
    /// existing sessions are updated, new sessions are created, and missing sessions
    /// are removed.
    /// </summary>
    /// <param name="sessionDtos">
    /// List of session DTOs representing the current UI scheduling state.
    /// </param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public async Task SaveSessionsAsync(List<SessionDTO> sessionDtos)
    {
        await using var db = await _contextFactory.CreateDbContextAsync();

        var existingSessions = await db.Sessions
            .Include(s => s.Papers)
            .ToListAsync();

        var dtoIds = sessionDtos
            .Where(d => d.Id.HasValue)
            .Select(d => d.Id!.Value)
            .ToHashSet();

        // Remove deleted sessions
        var toRemove = existingSessions
            .Where(s => !dtoIds.Contains(s.Id))
            .ToList();

        db.Sessions.RemoveRange(toRemove);

        foreach (var dto in sessionDtos)
        {
            Session session;

            if (dto.Id.HasValue)
            {
                session = existingSessions.First(s => s.Id == dto.Id.Value);
                session.Papers.Clear();
            }
            else
            {
                session = new Session
                {
                    Id = Guid.NewGuid()
                };
                db.Sessions.Add(session);
            }

            session.Name = dto.Name;
            session.Location = dto.Location;
            session.StartTime = dto.StartTime;
            session.EndTime = dto.EndTime;

            var papers = await db.Papers
                .Where(p => dto.PaperIds.Contains(p.Id))
                .ToListAsync();

            foreach (var paper in papers)
            {
                session.Papers.Add(paper);
            }
        }

        await db.SaveChangesAsync();
    }

    // =========================
    // GET
    // =========================

    /// <summary>
    /// Retrieves all sessions with their associated papers for UI rendering.
    /// 
    /// This method enriches session data with full <see cref="UserPaperDTO"/> objects
    /// to support drag-and-drop scheduling views.
    /// </summary>
    /// <returns>
    /// A list of sessions containing fully populated paper DTOs.
    /// </returns>
    public async Task<List<SessionWithPapersDTO>> GetSessionsAsync()
    {
        await using var db = await _contextFactory.CreateDbContextAsync();

        var sessions = await db.Sessions
            .Include(s => s.Papers)
            .ToListAsync();

        var allPapers = (await _paperQueryService.GetAllPapersAsync())
            .ToDictionary(p => p.PaperId);

        var result = sessions.Select(s => new SessionWithPapersDTO
        {
            Id = s.Id,
            Name = s.Name,
            Location = s.Location,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            Papers = s.Papers
                .Where(p => allPapers.ContainsKey(p.Id))
                .Select(p => allPapers[p.Id])
                .ToList()
        }).ToList();

        return result;
    }
}