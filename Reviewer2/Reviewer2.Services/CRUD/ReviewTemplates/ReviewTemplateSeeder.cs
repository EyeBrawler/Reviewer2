using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reviewer2.Data.Context;
using Reviewer2.Data.Models;
using Serilog;

namespace Reviewer2.Services.CRUD.ReviewTemplates;

/// <summary>
/// Provides functionality for seeding a default <see cref="ReviewTemplate"/>
/// into the database if none currently exists.
/// </summary>
/// <remarks>
/// This seeder is idempotent and safe to run on application startup.
/// If a template already exists, no changes are made.
/// </remarks>
public static class ReviewTemplateSeeder
{
    /// <summary>
    /// Seeds a default review template into the database if none exists.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceProvider"/> used to resolve required services.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    public static async Task SeedDefaultTemplateAsync(IServiceProvider services)
    {
        await using var context = await services
            .GetRequiredService<IDbContextFactory<ApplicationContext>>()
            .CreateDbContextAsync();

        try
        {
            var exists = await context.ReviewTemplates
                .AnyAsync(t => t.IsActive);

            if (exists)
            {
                Log.Information("Review template already exists. Skipping seeding.");
                return;
            }

            Log.Information("Seeding default review template...");

            var template = new ReviewTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Default Review Template v1",
                Version = 1,
                IsActive = true,
                CreatedAtUtc = DateTimeOffset.UtcNow,

                // Empty Schema
                JsonSchema = GetEmptySchema()
            };

            context.ReviewTemplates.Add(template);
            await context.SaveChangesAsync();

            Log.Information("Default review template seeded successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error seeding review template");
            throw;
        }
    }
    
    private static string GetEmptySchema() => "{}";
}