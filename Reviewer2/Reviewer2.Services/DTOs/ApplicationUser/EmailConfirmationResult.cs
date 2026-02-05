namespace Reviewer2.Services.DTOs.ApplicationUser;

/// <summary>
/// Represents the result of an email confirmation attempt.
/// </summary>
/// <param name="Succeeded">
/// Indicates whether the email address was successfully confirmed.
/// </param>
/// <param name="ErrorMessage">
/// An optional message describing why the confirmation failed.
/// </param>
public sealed record EmailConfirmationResult(
    bool Succeeded,
    string? ErrorMessage = null
);
