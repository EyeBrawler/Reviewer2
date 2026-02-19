using System;
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
/// <param name="UserId">
/// The unique identifier of the newly created user when registration succeeds;
/// otherwise <c>null</c>.
/// </param>
/// <param name="EmailConfirmationToken">
/// An email confirmation token generated for the user, used to verify their email address.
/// This value is <c>null</c> if registration fails.
/// </param>
public sealed record RegisterUserResult(
    bool Succeeded,
    IEnumerable<IdentityError>? Errors,
    Guid? UserId,
    string? EmailConfirmationToken
);