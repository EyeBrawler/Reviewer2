namespace Reviewer2.Services.DTOs.ApplicationUser;

/// <summary>
/// Represents a password reset request generated for a user account.
/// </summary>
/// <param name="Email">
/// The email address associated with the account requesting a password reset.
/// </param>
/// <param name="EncodedToken">
/// A URL-safe, encoded password reset token used to authorize the reset operation.
/// </param>
public sealed record PasswordResetRequest(
    string Email,
    string EncodedToken
);