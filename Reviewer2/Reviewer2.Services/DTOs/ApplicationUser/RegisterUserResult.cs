using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Reviewer2.Services.DTOs.ApplicationUser;

/// <summary>
/// Represents the result of a user registration attempt.
/// </summary>
/// <param name="Succeeded">
/// Indicates whether the user was successfully created.
/// </param>
/// <param name="Errors">
/// A collection of identity errors describing why registration failed,
/// or <c>null</c> if the operation succeeded.
/// </param>
/// <param name="User">
/// The newly created <see cref="ApplicationUser"/> when registration succeeds;
/// otherwise <c>null</c>.
/// </param>
/// <param name="EmailConfirmationToken">
/// An email confirmation token generated for the user, used to verify their email address.
/// This value is <c>null</c> if registration fails.
/// </param>
public record RegisterUserResult(
    bool Succeeded,
    IEnumerable<IdentityError>? Errors,
    Data.Models.ApplicationUser? User,
    string? EmailConfirmationToken
);