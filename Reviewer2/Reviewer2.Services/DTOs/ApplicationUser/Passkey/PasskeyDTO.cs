using System;

namespace Reviewer2.Services.DTOs.ApplicationUser.Passkey;


/// <summary>
/// Represents a read-only data transfer object containing
/// display information about a registered passkey.
/// </summary>
/// <param name="CredentialId">
/// The Base64Url-encoded credential identifier associated with the passkey.
/// </param>
/// <param name="Name">
/// The user-defined friendly name of the passkey, if one was provided.
/// </param>
/// <param name="CreatedAt">
/// The date and time when the passkey was originally registered.
/// </param>
public sealed record PasskeyDTO(
    string CredentialId,
    string? Name,
    DateTimeOffset CreatedAt
);
