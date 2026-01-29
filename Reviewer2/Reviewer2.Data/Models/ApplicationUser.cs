using Microsoft.AspNetCore.Identity;

namespace Reviewer2.Data.Models;

/// <summary>
/// Represents an application user with additional properties.
/// Inherits from IdentityUser.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the user.
    /// </summary>
    public string? LastName { get; set; }
    
    /// <summary>
    /// Gets the full name of the user by combining the first and last names.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
}