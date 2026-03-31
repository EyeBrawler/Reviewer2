using System;

namespace Reviewer2.Services.DTOs.ApplicationUser;

/// <summary>
/// Represents a lightweight view of a user for searching or listing purposes.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
/// <param name="Email">The user's email address.</param>
public record UserLookupDTO(
    Guid Id,
    string FirstName,
    string LastName,
    string Email
);