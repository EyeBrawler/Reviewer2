using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
using Reviewer2.Data.Models;
using Serilog;

namespace Reviewer2.Services.PaperDecisions;

public class PaperDecisionService : IPaperDecisionService
{
    private readonly IDbContextFactory<ApplicationContext> _contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaperDecisionService"/> class.
    /// 
    /// This service coordinates decision-related operations for <see cref="Paper"/>
    /// entities, including acceptance, rejection, and revision requests. It uses
    /// an <see cref="IDbContextFactory{TContext}"/> to create database contexts
    /// on demand, ensuring proper scope management for each operation.
    /// </summary>
    /// <param name="contextFactory">
    /// A factory used to create instances of <see cref="ApplicationContext"/>.
    /// This enables safe and efficient creation of <see cref="DbContext"/> instances,
    /// particularly in environments such as Blazor where scoped lifetimes may vary.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="contextFactory"/> is null.
    /// </exception>
    public PaperDecisionService(IDbContextFactory<ApplicationContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task AcceptAsync(Guid paperId, Guid chairUserId, string? comment)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var paper = await context.Papers
                            .Include(p => p.ReviewAssignments)
                            .FirstOrDefaultAsync(p => p.Id == paperId);

            if (paper is null)
            {
                Log.Warning("Accept failed: Paper {PaperId} not found", paperId);
                throw new InvalidOperationException("Paper not found.");
            }

            Log.Information(
                "Attempting to accept paper {PaperId} in status {Status} by user {ChairUserId}",
                paperId, paper.Status, chairUserId);

            paper.Accept(chairUserId, comment);

            await context.SaveChangesAsync();

            Log.Information(
                "Paper {PaperId} accepted by user {ChairUserId}",
                paperId, chairUserId);
        }
        catch (InvalidOperationException ex)
        {
            Log.Warning(ex, "Accept failed for paper {PaperId}", paperId);
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error while accepting paper {PaperId}", paperId);
            throw;
        }
    }

    public async Task RejectAsync(Guid paperId, Guid chairUserId, string? comment)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var paper = await context.Papers
                            .FirstOrDefaultAsync(p => p.Id == paperId);

            if (paper is null)
            {
                Log.Warning("Reject failed: Paper {PaperId} not found", paperId);
                throw new InvalidOperationException("Paper not found.");
            }

            Log.Information(
                "Attempting to reject paper {PaperId} in status {Status} by user {ChairUserId}",
                paperId, paper.Status, chairUserId);

            paper.Reject(chairUserId, comment);

            await context.SaveChangesAsync();

            Log.Information(
                "Paper {PaperId} rejected by user {ChairUserId}",
                paperId, chairUserId);
        }
        catch (InvalidOperationException ex)
        {
            Log.Warning(ex, "Reject failed for paper {PaperId}", paperId);
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error while rejecting paper {PaperId}", paperId);
            throw;
        }
    }

    public async Task RequestRevisionsAsync(Guid paperId, Guid chairUserId, string? comment)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var paper = await context.Papers
                            .FirstOrDefaultAsync(p => p.Id == paperId);

            if (paper is null)
            {
                Log.Warning("Revision request failed: Paper {PaperId} not found", paperId);
                throw new InvalidOperationException("Paper not found.");
            }

            Log.Information(
                "Attempting to request revisions for paper {PaperId} in status {Status} by user {ChairUserId}",
                paperId, paper.Status, chairUserId);

            paper.RequestRevisions(chairUserId, comment);

            await context.SaveChangesAsync();

            Log.Information(
                "Revisions requested for paper {PaperId} by user {ChairUserId}",
                paperId, chairUserId);
        }
        catch (InvalidOperationException ex)
        {
            Log.Warning(ex, "Revision request failed for paper {PaperId}", paperId);
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error while requesting revisions for paper {PaperId}", paperId);
            throw;
        }
    }
}