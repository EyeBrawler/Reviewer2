using System.Collections.Generic;

namespace Reviewer2.Services.DTOs.PaperSubmission;

/// <summary>
/// Represents updated metadata for an existing draft paper.
/// 
/// When processed, this request replaces the current draft's
/// title, abstract, and author collection.
/// </summary>
public class UpdateDraftRequest
{
    /// <summary>
    /// The updated title of the paper.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The updated abstract describing the paper.
    /// </summary>
    public string Abstract { get; set; } = string.Empty;

    /// <summary>
    /// The complete replacement list of authors for the draft.
    /// 
    /// Existing authors should be removed and replaced
    /// with this collection in the order provided.
    /// </summary>
    public List<AuthorInputDTO> Authors { get; set; } = new();
}