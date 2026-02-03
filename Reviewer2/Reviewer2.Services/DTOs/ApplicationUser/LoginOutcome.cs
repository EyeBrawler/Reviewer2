namespace Reviewer2.Services.DTOs.ApplicationUser;

/// <summary>
/// Defines the possible outcomes of a user login attempt.
/// </summary>
public enum LoginOutcome
{
    /// <summary>
    /// The user successfully authenticated.
    /// </summary>
    Success,

    /// <summary>
    /// The user must complete a second authentication factor before signing in.
    /// </summary>
    RequiresTwoFactor,

    /// <summary>
    /// The user account is locked and cannot be accessed.
    /// </summary>
    LockedOut,

    /// <summary>
    /// The login attempt failed due to invalid credentials or other errors.
    /// </summary>
    Failed
}


/// <summary>
/// Represents the result of a user login attempt.
/// </summary>
/// <param name="Outcome">
/// The outcome of the login operation, indicating whether authentication
/// succeeded or requires additional action.
/// </param>
public record LoginResult(LoginOutcome Outcome);