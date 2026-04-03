using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reviewer2.Data.Context;
using Reviewer2.Data.Models;
using Serilog;

namespace Reviewer2.Services.CRUD.Conferences;

/// <summary>
/// Provides functionality for seeding a default <see cref="Conference"/> into the database
/// if no conference currently exists.
/// </summary>
/// <remarks>
/// This seeder is designed to be idempotent and safe to run on application startup.
/// If a conference already exists, no changes are made.
/// </remarks>
public static class ConferenceSeeder
{
    /// <summary>
    /// Seeds a default conference into the database if none exists.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceProvider"/> used to resolve required services,
    /// including the <see cref="IDbContextFactory{TContext}"/> for creating
    /// an <see cref="ApplicationContext"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous seeding operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if required services cannot be resolved from the service provider.
    /// </exception>
    /// <exception cref="DbUpdateException">
    /// Thrown if an error occurs while saving the seeded conference to the database.
    /// </exception>
    /// <exception cref="Exception">
    /// Rethrows any unexpected exception encountered during the seeding process.
    /// </exception>
    /// <remarks>
    /// This method:
    /// <list type="bullet">
    /// <item><description>Checks if any <see cref="Conference"/> records exist.</description></item>
    /// <item><description>If none exist, creates a default conference with sample data.</description></item>
    /// <item><description>Persists the new conference and its associated <see cref="Deadline"/> entries.</description></item>
    /// <item><description>Logs progress and errors using <see cref="Log"/>.</description></item>
    /// </list>
    /// </remarks>
    public static async Task SeedDefaultConferenceAsync(IServiceProvider services)
    {
        await using var context = await services
            .GetRequiredService<IDbContextFactory<ApplicationContext>>()
            .CreateDbContextAsync();

        try
        {
            // Check if any conference already exists
            var exists = await context.Conferences.AnyAsync();

            if (exists)
            {
                Log.Information("Conference already exists. Skipping seeding.");
                return;
            }

            Log.Information("Seeding default conference...");

            var conference = new Conference
            {
                Name = "Sample Conference 2026",
                Description = "This is a default seeded conference. You can edit or replace it.",
                CallForPapers = "Submit your best work! Topics include software engineering, AI, and systems.",
                Deadlines =
                [
                    new Deadline
                    {
                        Name = "Submission Deadline",
                        Date = DateTime.UtcNow.AddMonths(1),
                        Priority = 1
                    },

                    new Deadline
                    {
                        Name = "Review Deadline",
                        Date = DateTime.UtcNow.AddMonths(2),
                        Priority = 2
                    },

                    new Deadline
                    {
                        Name = "Notification Date",
                        Date = DateTime.UtcNow.AddMonths(3),
                        Priority = 3
                    }
                ]
            };

            context.Conferences.Add(conference);
            await context.SaveChangesAsync();

            Log.Information("Default conference seeded successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error seeding default conference");
            throw;
        }
    }
}