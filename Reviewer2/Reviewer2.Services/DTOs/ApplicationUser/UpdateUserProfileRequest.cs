namespace Reviewer2.Services.DTOs.ApplicationUser;

/// <summary>
/// Represents a request to update editable profile information
/// for an existing user.
/// </summary>
public sealed record UpdateUserProfileRequest(
    string UserId,
    string UserName,
    string? FirstName,
    string? LastName,
    string? PhoneNumber
);