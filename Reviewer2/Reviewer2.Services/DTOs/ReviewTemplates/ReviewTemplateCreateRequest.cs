using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reviewer2.Services.DTOs.ReviewTemplates;

/// <summary>
/// DTO used when creating or versioning a review template.
/// Represents input from the template builder UI.
/// </summary>
/// <remarks>
/// <para>
/// This DTO captures the structured definition of a review template as configured
/// by a chair or administrator. The provided field definitions are serialized
/// into the <c>JsonSchema</c> property of the underlying data model.
/// </para>
/// 
/// <para>
/// This request is used for both initial template creation and creating new
/// versions of existing templates.
/// </para>
/// </remarks>
public class ReviewTemplateCreateRequest
{
    /// <summary>
    /// Gets or sets the human-readable name of the template.
    /// </summary>
    /// <remarks>
    /// This value is typically displayed in administrative interfaces and may
    /// indicate the type or purpose of the review (e.g., "Full Paper Review").
    /// </remarks>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of fields that define the structure of the review form.
    /// </summary>
    /// <remarks>
    /// Each field describes a single input element (e.g., score, text area, selection).
    /// The collection is serialized into JSON and stored in the template's schema.
    /// 
    /// The order of fields in this list determines the rendering order in the review UI.
    /// </remarks>
    [Required]
    public List<TemplateFieldDTO> Fields { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the newly created template
    /// should be marked as active.
    /// </summary>
    /// <remarks>
    /// If set to <see langword="true"/>, the system should deactivate any currently
    /// active template before activating this one.
    /// </remarks>
    public bool SetAsActive { get; set; }
}