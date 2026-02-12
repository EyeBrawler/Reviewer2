using System.Security.Claims;

namespace Reviewer2.Services.DTOs.ApplicationUser;

/// <summary>
/// Represents the outcome of a password change attempt.
/// </summary>
public enum ChangePasswordOutcome
{
    /// <summary>
    /// The password was successfully changed.
    /// </summary>
    Success,

    /// <summary>
    /// The operation failed because the user associated with the provided
    /// <see cref="ClaimsPrincipal"/> could not be found.
    /// </summary>
    UserNotFound,

    /// <summary>
    /// The operation failed because the user does not have an existing
    /// password set (for example, if the account was created via
    /// an external login provider).
    /// </summary>
    NoPasswordSet,

    /// <summary>
    /// The operation failed because the provided current password
    /// was incorrect or the new password did not meet policy requirements.
    /// </summary>
    InvalidPassword
}
