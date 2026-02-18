using System;

namespace Reviewer2.Data.Models;
public class Author
{
    public Guid Id { get; set; }

    public Guid PaperId { get; set; }

    // Optional link to a real user account
    public string? UserId { get; set; }
    
    public ApplicationUser? User { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;

    public int AuthorOrder { get; set; }

    public bool IsCorrespondingAuthor { get; set; }
    public bool IsPresenter { get; set; }

    public Paper Paper { get; set; } = default!;
}
 