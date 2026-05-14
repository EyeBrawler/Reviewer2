using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
using Reviewer2.Data.Models;
using Reviewer2.Services.DTOs.ReviewTemplates;
using Serilog;

namespace Reviewer2.Services.CRUD.ReviewTemplates;

/// <inheritdoc/>
public class ReviewTemplateService : IReviewTemplateService
{
    private readonly IDbContextFactory<ApplicationContext> _contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReviewTemplateService"/> class.
    /// </summary>
    /// <param name="contextFactory">
    /// Factory used to create <see cref="ApplicationContext"/> instances for each operation.
    /// </param>
    public ReviewTemplateService(IDbContextFactory<ApplicationContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }


    /// <inheritdoc/>
    public async Task<ReviewTemplateDTO?> GetByIdAsync(Guid templateId)
    {
        Log.Information("Retrieving ReviewTemplate with Id {TemplateId}", templateId);

        await using var context = await _contextFactory.CreateDbContextAsync();

        try
        {
            var entity = await context.ReviewTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == templateId);

            if (entity is null)
            {
                Log.Warning("ReviewTemplate with Id {TemplateId} was not found", templateId);
                return null;
            }

            var fields = DeserializeFields(entity.JsonSchema, entity.Id);

            var dto = new ReviewTemplateDTO
            {
                Id = entity.Id,
                Version = entity.Version,
                Name = entity.Name,
                IsActive = entity.IsActive,
                CreatedAtUtc = entity.CreatedAtUtc,
                Fields = fields
            };

            Log.Information("Successfully retrieved ReviewTemplate {TemplateId}", templateId);

            return dto;
        }
        catch (Exception ex)
        {
            Log.Error(ex,
                "An error occurred while retrieving ReviewTemplate {TemplateId}",
                templateId);

            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ReviewTemplateDTO>> GetAllAsync()
    {
        Log.Information("Retrieving all ReviewTemplates");

        await using var context = await _contextFactory.CreateDbContextAsync();

        try
        {
            var entities = await context.ReviewTemplates
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedAtUtc)
                .ToListAsync();

            var results = new List<ReviewTemplateDTO>(entities.Count);
            
            results.AddRange(from entity in entities
            let fields = DeserializeFields(entity.JsonSchema, entity.Id)
            select new ReviewTemplateDTO
            {
                Id = entity.Id,
                Version = entity.Version,
                Name = entity.Name,
                IsActive = entity.IsActive,
                CreatedAtUtc = entity.CreatedAtUtc,
                Fields = fields
            });

            Log.Information("Successfully retrieved {Count} ReviewTemplates", results.Count);

            return results;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while retrieving all ReviewTemplates");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<ReviewTemplateDTO?> GetActiveTemplateAsync()
    {
        Log.Information("Retrieving active ReviewTemplate");

        await using var context = await _contextFactory.CreateDbContextAsync();

        try
        {
            var entity = await context.ReviewTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.IsActive);

            if (entity is null)
            {
                Log.Warning("No active ReviewTemplate found");
                return null;
            }

            var fields = DeserializeFields(entity.JsonSchema, entity.Id);

            var dto = new ReviewTemplateDTO
            {
                Id = entity.Id,
                Version = entity.Version,
                Name = entity.Name,
                IsActive = entity.IsActive,
                CreatedAtUtc = entity.CreatedAtUtc,
                Fields = fields
            };

            Log.Information("Successfully retrieved active ReviewTemplate {TemplateId}", entity.Id);

            return dto;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while retrieving the active ReviewTemplate");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<ReviewTemplateDTO> CreateAsync(ReviewTemplateCreateRequest request)
    {
        Log.Information("Creating new ReviewTemplate with name {Name}", request.Name);

        await using var context = await _contextFactory.CreateDbContextAsync();

        try
        {
            // Determine next version (simple global versioning)
            var maxVersion = await context.ReviewTemplates
                .Select(t => (int?)t.Version)
                .MaxAsync() ?? 0;

            var newVersion = maxVersion + 1;

            // Build schema object
            var schemaObject = new
            {
                fields = request.Fields
            };

            var jsonSchema = JsonSerializer.Serialize(schemaObject);

            var entity = new ReviewTemplate
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = newVersion,
                JsonSchema = jsonSchema,
                IsActive = false, // handled below
                CreatedAtUtc = DateTimeOffset.UtcNow
            };

            // Handle activation
            if (request.SetAsActive)
            {
                Log.Information("Setting new ReviewTemplate {TemplateId} as active", entity.Id);

                var activeTemplates = await context.ReviewTemplates
                    .Where(t => t.IsActive)
                    .ToListAsync();

                foreach (var t in activeTemplates)
                {
                    t.IsActive = false;
                }

                entity.IsActive = true;
            }

            context.ReviewTemplates.Add(entity);

            await context.SaveChangesAsync();

            // Using the deserialization helper
            var fields = DeserializeFields(entity.JsonSchema, entity.Id);

            var dto = new ReviewTemplateDTO
            {
                Id = entity.Id,
                Version = entity.Version,
                Name = entity.Name,
                IsActive = entity.IsActive,
                CreatedAtUtc = entity.CreatedAtUtc,
                Fields = fields
            };

            Log.Information("Successfully created ReviewTemplate {TemplateId} (Version {Version})",
                entity.Id, entity.Version);

            return dto;
        }
        catch (Exception ex)
        {
            Log.Error(ex,
                "An error occurred while creating ReviewTemplate with name {Name}",
                request.Name);

            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<ReviewTemplateDTO> CreateNewVersionAsync(
        Guid baseTemplateId,
        ReviewTemplateCreateRequest request)
    {
        Log.Information(
            "Creating new version of ReviewTemplate {BaseTemplateId} with name {Name}",
            baseTemplateId, request.Name);

        await using var context = await _contextFactory.CreateDbContextAsync();

        try
        {
            var baseTemplate = await context.ReviewTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == baseTemplateId);

            if (baseTemplate is null)
            {
                Log.Warning(
                    "Base ReviewTemplate {BaseTemplateId} not found",
                    baseTemplateId);

                throw new InvalidOperationException("Base template not found.");
            }

            var newVersion = baseTemplate.Version + 1;

            // Build schema from request
            var schemaObject = new
            {
                fields = request.Fields
            };

            var jsonSchema = JsonSerializer.Serialize(schemaObject);

            var entity = new ReviewTemplate
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = newVersion,
                JsonSchema = jsonSchema,
                IsActive = false, // handled below
                CreatedAtUtc = DateTimeOffset.UtcNow
            };

            // Handle activation
            if (request.SetAsActive)
            {
                Log.Information(
                    "Setting new version {TemplateId} as active (based on {BaseTemplateId})",
                    entity.Id, baseTemplateId);

                var activeTemplates = await context.ReviewTemplates
                    .Where(t => t.IsActive)
                    .ToListAsync();

                foreach (var t in activeTemplates)
                {
                    t.IsActive = false;
                }

                entity.IsActive = true;
            }

            context.ReviewTemplates.Add(entity);

            await context.SaveChangesAsync();

            var fields = DeserializeFields(entity.JsonSchema, entity.Id);

            var dto = new ReviewTemplateDTO
            {
                Id = entity.Id,
                Version = entity.Version,
                Name = entity.Name,
                IsActive = entity.IsActive,
                CreatedAtUtc = entity.CreatedAtUtc,
                Fields = fields
            };

            Log.Information(
                "Successfully created new version {TemplateId} (Version {Version}) from base {BaseTemplateId}",
                entity.Id, entity.Version, baseTemplateId);

            return dto;
        }
        catch (Exception ex)
        {
            Log.Error(ex,
                "An error occurred while creating a new version of ReviewTemplate {BaseTemplateId}",
                baseTemplateId);

            throw;
        }
    }

    /// <inheritdoc/>
    public async Task SetActiveAsync(Guid templateId)
    {
        Log.Information("Setting ReviewTemplate {TemplateId} as active", templateId);

        await using var context = await _contextFactory.CreateDbContextAsync();

        try
        {
            var template = await context.ReviewTemplates
                .FirstOrDefaultAsync(t => t.Id == templateId);

            if (template is null)
            {
                Log.Warning("ReviewTemplate {TemplateId} not found", templateId);
                throw new InvalidOperationException("Template not found.");
            }

            // If already active, no-op
            if (template.IsActive)
            {
                Log.Information("ReviewTemplate {TemplateId} is already active", templateId);
                return;
            }

            // Deactivate all currently active templates
            var activeTemplates = await context.ReviewTemplates
                .Where(t => t.IsActive)
                .ToListAsync();

            foreach (var t in activeTemplates)
            {
                t.IsActive = false;
            }

            // Activate the requested template
            template.IsActive = true;

            await context.SaveChangesAsync();

            Log.Information("Successfully set ReviewTemplate {TemplateId} as active", templateId);
        }
        catch (Exception ex)
        {
            Log.Error(ex,
                "An error occurred while setting ReviewTemplate {TemplateId} as active",
                templateId);

            throw;
        }
    }
    
    /// <inheritdoc/>
    public async Task DeactivateAsync(Guid templateId)
    {
        Log.Information("Deactivating ReviewTemplate {TemplateId}", templateId);

        await using var context = await _contextFactory.CreateDbContextAsync();

        try
        {
            var template = await context.ReviewTemplates
                .FirstOrDefaultAsync(t => t.Id == templateId);

            if (template is null)
            {
                Log.Warning("ReviewTemplate {TemplateId} not found", templateId);
                return; // no-op (consistent with typical deactivate semantics)
            }

            if (!template.IsActive)
            {
                Log.Information("ReviewTemplate {TemplateId} is already inactive", templateId);
                return;
            }

            template.IsActive = false;

            await context.SaveChangesAsync();

            Log.Information("Successfully deactivated ReviewTemplate {TemplateId}", templateId);
        }
        catch (Exception ex)
        {
            Log.Error(ex,
                "An error occurred while deactivating ReviewTemplate {TemplateId}",
                templateId);

            throw;
        }
    }
    
    private static List<TemplateFieldDTO> DeserializeFields(string jsonSchema, Guid templateId)
    {
        try
        {
            using var doc = JsonDocument.Parse(jsonSchema);

            if (doc.RootElement.TryGetProperty("fields", out var fieldsElement))
                return fieldsElement.Deserialize<List<TemplateFieldDTO>>()
                       ?? [];
            Log.Warning(
                "ReviewTemplate {TemplateId} has no 'fields' property in JsonSchema",
                templateId);

            return [];

        }
        catch (Exception ex)
        {
            Log.Error(ex,
                "Failed to deserialize JsonSchema for ReviewTemplate {TemplateId}",
                templateId);

            return [];
        }
    }
}