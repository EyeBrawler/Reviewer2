namespace Reviewer2.Services.DTOs.ApplicationUser;

/// <summary>
/// Represents the result of a user profile update operation.
/// </summary>
/// <param name="Succeeded">
/// Indicates whether the update completed successfully.
/// </param>
/// <param name="ErrorMessage">
/// An optional error message if the update failed.
/// </param>
public sealed record UpdateUserProfileResult(
    bool Succeeded,
    string? ErrorMessage = null
);
