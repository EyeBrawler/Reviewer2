namespace Reviewer2.Services.DTOs.ApplicationUser;

/// <summary>
/// Represents the result of a two-factor authentication sign-in attempt.
/// </summary>
/// <param name="Outcome">The outcome of the sign-in attempt.</param>
/// <param name="ErrorMessage">
/// An optional error message intended for display to the user when the sign-in fails.
/// </param>
public sealed record TwoFactorSignInResult(
    TwoFactorSignInOutcome Outcome,
    string? ErrorMessage = null
);