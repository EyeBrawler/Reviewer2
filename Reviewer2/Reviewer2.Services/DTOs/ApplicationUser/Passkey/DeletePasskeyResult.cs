namespace Reviewer2.Services.DTOs.ApplicationUser.Passkey;

/// <summary>
/// Represents the result of an attempt to delete a registered passkey
/// from a user account.
/// </summary>
/// <param name="Succeeded">
/// Indicates whether the passkey deletion operation completed successfully.
/// </param>
/// <param name="ErrorMessage">
/// An optional error message describing why the deletion failed.
/// This value will be <c>null</c> if the operation succeeded.
/// </param>
public sealed record DeletePasskeyResult(
    bool Succeeded,
    string? ErrorMessage);