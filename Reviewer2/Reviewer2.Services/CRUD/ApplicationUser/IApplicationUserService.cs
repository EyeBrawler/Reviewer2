using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Reviewer2.Services.DTOs.ApplicationUser;
using Reviewer2.Services.DTOs.ApplicationUser.Passkey;

namespace Reviewer2.Services.CRUD.ApplicationUser;

using Data.Models;

/// <summary>
/// Interface for managing application user-related operations.
/// Provides methods for user authentication, retrieval, and management.
/// </summary>
public interface IApplicationUserService
{
    /// <summary>
    /// Signs out the current user.
    /// </summary>
    public Task SignOutAsync();
    /// <summary>
    /// Signs in a user by username.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <returns>True if sign-in was successful, otherwise false.</returns>
    public Task<bool> SignInAsync(string username);
    
    /// <summary>
    /// Gets a list of users by role.
    /// </summary>
    /// <param name="roleName">The name of the role.</param>
    /// <returns>A list of users in the specified role.</returns>
    public Task<List<ApplicationUser>> GetUsersByRoleAsync(string roleName);
    
    /// <summary>
    /// Retrieves the names of all roles assigned to the specified user.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user whose roles should be retrieved.
    /// </param>
    /// <returns>
    /// A collection of role names associated with the user.
    /// Returns <c>null</c> if no user with the specified identifier exists.
    /// </returns>
    /// <remarks>
    /// This method queries the underlying identity store to determine
    /// the roles currently assigned to the user. The returned collection
    /// reflects role membership at the time of invocation.
    /// </remarks>
    Task<IList<string>?> GetRolesAsync(string userId);

    
    ///<summary>
    /// Retrieves a list of all ApplicationUsers currently registered
    ///</summary>
    ///<returns>List of ApplicationUser objects</returns>
    public Task<List<ApplicationUser>> GetAllAsync();
    
    ///<summary>
    /// Retrieves a ApplicationUser by id guid
    ///</summary>
    ///<returns>ApplicationUser object</returns>
    public Task<ApplicationUser?> GetByIdAsync(Guid id);

    /// <summary>
    /// Determines if the given user is currently persisted within the database.
    /// </summary>
    /// <param name="user">An <see cref="ApplicationUser"/> object. </param>
    /// <returns></returns>
    public Task<bool?> IsNewUser(ApplicationUser user);
    
    /// <summary>
    /// Retrieves an ApplicationUser by last name.
    /// </summary>
    /// <param name="lastName">The last name of the user.</param>
    /// <returns>An ApplicationUser object if found, otherwise null.</returns>
    public Task<ApplicationUser?> GetByLastNameAsync(string lastName);

    /// <summary>
    /// Retrieves an ApplicationUser by username.
    /// </summary>
    /// <param name="userName">The username of the user.</param>
    /// <returns>An ApplicationUser object if found, otherwise null.</returns>
    public Task<ApplicationUser?> GetByUserNameAsync(string userName);

    /// <summary>
    /// Retrieves an ApplicationUser by email.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <returns>An ApplicationUser object if found, otherwise null.</returns>
    public Task<ApplicationUser?> GetByEmailAsync(string email);
    
    ///<summary>
    /// Creates a ApplicationUser object and saves it to the database
    ///</summary>
    ///<returns>ApplicationUser object</returns>
    public Task<IdentityResult> AddApplicationUser(ApplicationUser applicationUser);
    
    ///<summary>
    /// Updates a ApplicationUser currently in the database
    ///</summary>
    ///<returns>Updated ApplicationUser object</returns>
    public Task<bool> UpdateApplicationUser(ApplicationUser applicationUser);

    /// <summary>
    /// Registers a new application user in the system.
    /// </summary>
    /// <param name="request">
    /// A <see cref="RegisterUserRequest"/> containing the information required
    /// to create the user account.
    /// </param>
    /// <returns>
    /// A <see cref="RegisterUserResult"/> indicating whether the registration
    /// succeeded, along with any errors, the created user, and an email
    /// confirmation token if successful.
    /// </returns>
    /// <remarks>
    /// This method creates the user, assigns the default role, and generates
    /// an email confirmation token. It does not sign the user in.
    /// </remarks>
    public Task<RegisterUserResult> RegisterAsync(RegisterUserRequest request);
    
    /// <summary>
    /// Attempts to sign in a user using email and password credentials.
    /// </summary>
    /// <returns>
    /// A <see cref="LoginResult"/> describing the outcome of the login attempt.
    /// </returns>
    Task<LoginResult> PasswordLoginAsync(
        string email,
        string password,
        bool rememberMe);
    
    /// <summary>
    /// Initiates a password reset request for the specified email address.
    /// </summary>
    /// <param name="email">
    /// The email address associated with the account requesting a password reset.
    /// </param>
    /// <returns>
    /// A <see cref="PasswordResetRequest"/> containing the encoded reset token and email
    /// if the request can be fulfilled; otherwise, <c>null</c>.
    /// This method does not disclose whether the email corresponds to an existing account.
    /// </returns>
    Task<PasswordResetRequest?> RequestPasswordResetAsync(string email);
    
    /// <summary>
    /// Confirms a pending email address change for a user account.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user whose email address is being changed.
    /// </param>
    /// <param name="newEmail">
    /// The new email address to associate with the user account.
    /// </param>
    /// <param name="encodedToken">
    /// A URL-safe, encoded confirmation token generated when the email change was requested.
    /// </param>
    /// <returns>
    /// A result indicating whether the email change was successfully confirmed,
    /// along with an optional error message if the operation failed.
    /// </returns>
    Task<EmailChangeConfirmationResult> ConfirmEmailChangeAsync(
        Guid userId,
        string newEmail,
        string encodedToken);
    
    /// <summary>
    /// Confirms a user's email address using a confirmation token.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user whose email address is being confirmed.
    /// </param>
    /// <param name="encodedToken">
    /// A URL-safe, encoded email confirmation token.
    /// </param>
    /// <returns>
    /// A result indicating whether the email confirmation succeeded,
    /// including an optional error message if the operation failed.
    /// </returns>
    Task<EmailConfirmationResult> ConfirmEmailAsync(
        Guid userId,
        string encodedToken);
    
    /// <summary>
    /// Retrieves the user currently undergoing two-factor authentication.
    /// </summary>
    /// <returns>
    /// The pending two-factor authentication user, or <c>null</c> if no such user exists.
    /// </returns>
    Task<ApplicationUser?> GetPendingTwoFactorUserAsync();


    /// <summary>
    /// Attempts to complete a two-factor authentication sign-in using an authenticator code.
    /// </summary>
    /// <param name="twoFactorCode">The authenticator code entered by the user.</param>
    /// <param name="rememberMe">Whether the sign-in should persist across browser sessions.</param>
    /// <param name="rememberMachine">
    /// Whether the current machine should be remembered and bypass two-factor authentication in the future.
    /// </param>
    /// <returns>
    /// A <see cref="TwoFactorSignInResult"/> describing the outcome of the sign-in attempt.
    /// </returns>
    Task<TwoFactorSignInResult> SignInWithAuthenticatorAsync(
        string twoFactorCode,
        bool rememberMe,
        bool rememberMachine);
    
    /// <summary>
    /// Retrieves all registered passkeys for the specified user.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user whose passkeys should be retrieved.
    /// </param>
    /// <returns>
    /// A read-only list of <see cref="PasskeyDTO"/> objects representing the user's
    /// registered passkeys, or <c>null</c> if no user with the specified ID exists.
    /// </returns>
    /// <remarks>
    /// The returned credential identifiers are Base64Url-encoded and safe for
    /// transport and display in client applications.
    /// </remarks>
    Task<IReadOnlyList<PasskeyDTO>?> GetPasskeysAsync(Guid userId);
    
    /// <summary>
    /// Registers a new passkey for the specified user using the provided
    /// WebAuthn attestation payload.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user who is registering the passkey.
    /// </param>
    /// <param name="credentialJson">
    /// The JSON-encoded WebAuthn credential attestation response
    /// returned from the client.
    /// </param>
    /// <returns>
    /// An <see cref="AddPasskeyResult"/> describing whether the operation succeeded.
    /// If successful, the result contains the Base64Url-encoded credential identifier.
    /// If unsuccessful, the result contains an error message describing the failure.
    /// </returns>
    /// <remarks>
    /// This method validates the attestation response before storing the passkey.
    /// The credential identifier returned in the result can be used for future
    /// passkey management operations.
    /// </remarks>
    Task<AddPasskeyResult> AddPasskeyAsync(
        Guid userId,
        string credentialJson);

    /// <summary>
    /// Deletes a previously registered passkey from the specified user account.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user whose passkey should be removed.
    /// </param>
    /// <param name="credentialIdBase64Url">
    /// The Base64Url-encoded credential identifier of the passkey to delete.
    /// </param>
    /// <returns>
    /// A <see cref="DeletePasskeyResult"/> indicating whether the passkey
    /// was successfully removed. If unsuccessful, an error message is provided.
    /// </returns>
    /// <remarks>
    /// The credential identifier must be Base64Url-encoded. If decoding fails,
    /// the operation will return a failure result.
    /// </remarks>
    Task<DeletePasskeyResult> DeletePasskeyAsync(
        Guid userId,
        string credentialIdBase64Url);

    /// <summary>
    /// Retrieves the <see cref="ApplicationUser"/> associated with the specified
    /// <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <param name="principal">
    /// The claims principal representing the currently authenticated user.
    /// </param>
    /// <returns>
    /// The corresponding <see cref="ApplicationUser"/> if one exists;
    /// otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method resolves the user through the underlying identity system
    /// and does not guarantee that the principal is authenticated.
    /// </remarks>
    Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal principal);
    
    /// <summary>
    /// Retrieves profile information for the currently authenticated user.
    /// </summary>
    /// <param name="principal">
    /// The claims principal representing the current user.
    /// </param>
    /// <returns>
    /// A <see cref="UserProfileDTO"/> if the user exists; otherwise, <c>null</c>.
    /// </returns>
    Task<UserProfileDTO?> GetProfileAsync(ClaimsPrincipal principal);


    /// <summary>
    /// Updates editable profile information for an existing user.
    /// </summary>
    /// <param name="request">
    /// A request containing the updated profile values.
    /// </param>
    /// <returns>
    /// A <see cref="UpdateUserProfileResult"/> describing the outcome.
    /// </returns>
    Task<UpdateUserProfileResult> UpdateProfileAsync(UpdateUserProfileRequest request);
    
    /// <summary>
    /// Attempts to complete a two-factor authentication sign-in using a recovery code.
    /// </summary>
    /// <param name="recoveryCode">The recovery code entered by the user.</param>
    /// <returns>
    /// A <see cref="TwoFactorSignInResult"/> describing the outcome.
    /// </returns>
    Task<TwoFactorSignInResult> SignInWithRecoveryCodeAsync(string recoveryCode);
    
    /// <summary>
    /// Attempts to change the password for the currently authenticated user.
    /// </summary>
    /// <param name="principal">
    /// The <see cref="ClaimsPrincipal"/> representing the currently authenticated user.
    /// </param>
    /// <param name="currentPassword">
    /// The user's current password.
    /// </param>
    /// <param name="newPassword">
    /// The new password the user wishes to set.
    /// </param>
    /// <returns>
    /// A <see cref="ChangePasswordResult"/> describing the outcome of the operation,
    /// including whether the password was successfully changed or if an error occurred.
    /// </returns>
    /// <remarks>
    /// This method validates that the user exists and has an existing password
    /// before attempting to change it. If the operation succeeds, the user's
    /// sign-in session is refreshed to ensure security consistency.
    /// </remarks>
    Task<ChangePasswordResult> ChangePasswordAsync(
        ClaimsPrincipal principal,
        string currentPassword,
        string newPassword);
}