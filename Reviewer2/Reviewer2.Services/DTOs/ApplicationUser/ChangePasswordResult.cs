namespace Reviewer2.Services.DTOs.ApplicationUser;

/// <summary>
/// Represents the result of a password change attempt.
/// </summary>
/// <param name="Outcome">
/// The <see cref="ChangePasswordOutcome"/> indicating the result of the operation.
/// </param>
/// <param name="ErrorMessage">
/// An optional error message describing why the operation failed.
/// This value is intended for display to the user when appropriate.
/// </param>
public sealed record ChangePasswordResult(
    ChangePasswordOutcome Outcome,
    string? ErrorMessage = null);