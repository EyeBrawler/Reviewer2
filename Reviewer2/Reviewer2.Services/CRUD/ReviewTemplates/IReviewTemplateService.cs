using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reviewer2.Services.DTOs.ReviewTemplates;

namespace Reviewer2.Services.CRUD.ReviewTemplates;

/// <summary>
/// Defines operations for managing and retrieving review templates.
/// </summary>
/// <remarks>
/// <para>
/// A review template defines the structure and validation rules for dynamic
/// review forms. Templates are versioned to ensure that previously submitted
/// reviews remain consistent with the schema that was active at the time of submission.
/// </para>
///
/// <para>
/// This service operates on DTOs rather than data models to ensure a clean
/// separation between persistence and application layers.
/// </para>
/// </remarks>
public interface IReviewTemplateService
{
    /// <summary>
    /// Retrieves a review template by its unique identifier.
    /// </summary>
    /// <param name="templateId">The unique identifier of the template.</param>
    /// <returns>
    /// A <see cref="ReviewTemplateDTO"/> if found; otherwise, <see langword="null"/>.
    /// </returns>
    Task<ReviewTemplateDTO?> GetByIdAsync(Guid templateId);

    /// <summary>
    /// Retrieves all review templates in the system.
    /// </summary>
    /// <remarks>
    /// This includes both active and inactive templates, as well as all versions.
    /// </remarks>
    /// <returns>A collection of all <see cref="ReviewTemplateDTO"/> instances.</returns>
    Task<IReadOnlyList<ReviewTemplateDTO>> GetAllAsync();

    /// <summary>
    /// Retrieves the currently active review template.
    /// </summary>
    /// <remarks>
    /// The active template is used when creating new review assignments.
    /// Typically, only one template should be active at a time within a given scope.
    /// </remarks>
    /// <returns>
    /// The active <see cref="ReviewTemplateDTO"/>, or <see langword="null"/> if none is active.
    /// </returns>
    Task<ReviewTemplateDTO?> GetActiveTemplateAsync();

    /// <summary>
    /// Creates a new review template.
    /// </summary>
    /// <param name="request">
    /// The request containing template metadata and field definitions.
    /// </param>
    /// <returns>
    /// The created <see cref="ReviewTemplateDTO"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method converts the provided field definitions into a JSON schema
    /// for storage and assigns a new version number.
    /// </para>
    ///
    /// <para>
    /// If <see cref="ReviewTemplateCreateRequest.SetAsActive"/> is <see langword="true"/>,
    /// any previously active template should be deactivated.
    /// </para>
    /// </remarks>
    Task<ReviewTemplateDTO> CreateAsync(ReviewTemplateCreateRequest request);

    /// <summary>
    /// Creates a new version of an existing review template.
    /// </summary>
    /// <param name="baseTemplateId">
    /// The identifier of the template to base the new version on.
    /// </param>
    /// <param name="request">
    /// The request containing the updated template definition.
    /// </param>
    /// <returns>
    /// The newly created <see cref="ReviewTemplateDTO"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method creates a new template version using the provided field definitions
    /// while preserving the original template unchanged.
    /// </para>
    ///
    /// <para>
    /// The version number should be incremented relative to the base template.
    /// </para>
    ///
    /// <para>
    /// If <see cref="ReviewTemplateCreateRequest.SetAsActive"/> is <see langword="true"/>,
    /// the new version should be activated and any previously active template should be deactivated.
    /// </para>
    /// </remarks>
    Task<ReviewTemplateDTO> CreateNewVersionAsync(
        Guid baseTemplateId,
        ReviewTemplateCreateRequest request);

    /// <summary>
    /// Sets a specific review template as the active template.
    /// </summary>
    /// <param name="templateId">The identifier of the template to activate.</param>
    /// <remarks>
    /// This method should ensure that only one template is marked as active
    /// within the relevant scope.
    /// </remarks>
    Task SetActiveAsync(Guid templateId);

    /// <summary>
    /// Deactivates a specific review template.
    /// </summary>
    /// <param name="templateId">The identifier of the template to deactivate.</param>
    /// <remarks>
    /// After deactivation, there may be no active template unless another
    /// template is explicitly activated.
    /// </remarks>
    Task DeactivateAsync(Guid templateId);
}