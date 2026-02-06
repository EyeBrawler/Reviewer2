namespace Reviewer2.Services.DTOs.ApplicationUser;

/// <summary>
/// Represents the outcome of a two-factor authentication sign-in attempt.
/// </summary>
public enum TwoFactorSignInOutcome
{
    /// <summary>
    /// The user successfully completed two-factor authentication and was signed in.
    /// </summary>
    Success,

    /// <summary>
    /// The sign-in attempt failed because the user account is locked out.
    /// </summary>
    LockedOut,

    /// <summary>
    /// The sign-in attempt failed because the provided authenticator code was invalid.
    /// </summary>
    InvalidCode
}