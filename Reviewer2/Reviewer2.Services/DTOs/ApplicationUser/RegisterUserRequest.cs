namespace Reviewer2.Services.DTOs.ApplicationUser;

/// <summary>
/// Represents the data required to register a new application user.
/// </summary>
/// <remarks>
/// This DTO is used by the application layer to transfer user registration
/// data from the presentation layer to the domain/service layer.
/// </remarks>
public sealed record RegisterUserRequest
{
    /// <summary>
    /// Gets the username chosen by the user.
    /// </summary>
    /// <remarks>
    /// This value is used for authentication and must be unique.
    /// </remarks>
    public required string Username { get; init; }

    /// <summary>
    /// Gets the email address of the user.
    /// </summary>
    /// <remarks>
    /// This value is used for account confirmation and communication
    /// and must be unique.
    /// </remarks>
    public required string Email { get; init; }

    /// <summary>
    /// Gets the password provided by the user.
    /// </summary>
    /// <remarks>
    /// The password must satisfy the configured Identity password requirements.
    /// </remarks>
    public required string Password { get; init; }

    /// <summary>
    /// Gets the first name of the user.
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// Gets the last name of the user.
    /// </summary>
    public required string LastName { get; init; }
}
