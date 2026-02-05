namespace Reviewer2.Services.DTOs.ApplicationUser;

/// <summary>
/// Represents the outcome of an email change confirmation attempt.
/// </summary>
/// <param name="Succeeded">
/// Indicates whether the email change was successfully completed.
/// </param>
/// <param name="ErrorMessage">
/// An optional error message describing why the confirmation failed.
/// </param>
public sealed record EmailChangeConfirmationResult(
    bool Succeeded,
    string? ErrorMessage = null
);