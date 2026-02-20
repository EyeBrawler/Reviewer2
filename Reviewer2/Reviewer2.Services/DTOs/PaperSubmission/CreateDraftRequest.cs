using System.Collections.Generic;

namespace Reviewer2.Services.DTOs.PaperSubmission;

/// <summary>
/// Represents the data required to create a new paper draft.
/// 
/// This request initializes a <c>Paper</c> aggregate in Draft state,
/// including its basic metadata and associated authors.
/// </summary>
public class CreateDraftRequest
{
    /// <summary>
    /// The proposed title of the paper.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The abstract summarizing the paper's content.
    /// </summary>
    public string Abstract { get; set; } = string.Empty;

    /// <summary>
    /// The ordered list of authors associated with the draft.
    /// 
    /// Author order is significant and should be preserved
    /// during aggregate construction.
    /// </summary>
    public List<AuthorInputDTO> Authors { get; set; } = new();
}