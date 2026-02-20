using System;
using System.Collections.Generic;
using System.Linq;
using Reviewer2.Data.Models;

namespace Reviewer2.Services.DTOs.PaperSubmission;


/// <summary>
/// Extension methods for mapping between PaperSubmission DTOs and domain entities.
/// </summary>
public static class PaperSubmissionMapper
{
    /// <summary>
    /// Converts a collection of <see cref="AuthorInputDTO"/> into <see cref="Author"/> entities.
    /// </summary>
    /// <param name="dtos">The author DTOs to map.</param>
    /// <returns>A list of <see cref="Author"/> entities.</returns>
    public static List<Author> ToEntities(this IEnumerable<AuthorInputDTO> dtos)
    {
        if (dtos == null || !dtos.Any())
            throw new ArgumentException("At least one author is required.", nameof(dtos));

        return dtos.Select((dto, index) => new Author
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Institution = dto.Institution,
            IsCorrespondingAuthor = dto.IsCorrespondingAuthor,
            IsPresenter = dto.IsPresenter,
            AuthorOrder = index
        }).ToList();
    }
}
