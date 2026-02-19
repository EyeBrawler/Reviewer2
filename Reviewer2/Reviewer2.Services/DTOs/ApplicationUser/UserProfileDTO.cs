using System;

namespace Reviewer2.Services.DTOs.ApplicationUser;

/// <summary>
/// Represents profile information for a user that can be viewed or edited.
/// </summary>
public sealed record UserProfileDTO(
    Guid UserId,
    string UserName,
    string? FirstName,
    string? LastName,
    string? PhoneNumber
);
