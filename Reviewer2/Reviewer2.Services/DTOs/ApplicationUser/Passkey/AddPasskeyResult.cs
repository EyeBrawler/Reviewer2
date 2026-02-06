namespace Reviewer2.Services.DTOs.ApplicationUser.Passkey;


/// <summary>
/// Represents the result of an attempt to register a new passkey
/// for a user account.
/// </summary>
/// <param name="Succeeded">
/// Indicates whether the passkey registration operation completed successfully.
/// </param>
/// <param name="ErrorMessage">
/// An optional error message describing why the registration failed.
/// This value will be <c>null</c> if the operation succeeded.
/// </param>
/// <param name="CredentialId">
/// The Base64Url-encoded credential identifier for the newly registered passkey.
/// This value will be <c>null</c> if the operation did not succeed.
/// </param>
public sealed record AddPasskeyResult(
    bool Succeeded,
    string? ErrorMessage,
    string? CredentialId);
