using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
using Reviewer2.Services.DTOs.ApplicationUser;
using Reviewer2.Services.DTOs.ApplicationUser.Passkey;
using Serilog;

namespace Reviewer2.Services.CRUD.ApplicationUser;

using Data.Models;

/// <inheritdoc cref="IApplicationUserService"/>
public class ApplicationUserService : IApplicationUserService
{
    private readonly UserContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private const string DefaultRole = "Attendee";

    /// <inheritdoc cref="IApplicationUserService"/>
    public ApplicationUserService(UserContext context, UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <inheritdoc />
    public async Task SignOutAsync()
    {
        try
        {
            await _signInManager.SignOutAsync();
        }
        catch (Exception e)
        {
            Log.Warning("Unable to SignOutAsync in ApplicationUserService: Exception thrown: {Exception}", e.ToString());
        }
    }
    
    /// <inheritdoc />
    public async Task<bool> SignInAsync(string username)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null) await _signInManager.SignInAsync(user, isPersistent: false);
            return true;
        }
        catch (Exception e)
        {
            Log.Warning("Unable to SignInAsync with UserName: {username} in ApplicationUserService: Exception thrown: {Exception}", username, e.ToString());
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<List<ApplicationUser>> GetUsersByRoleAsync(string roleName)
    {
        try
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            return usersInRole.ToList();
        }
        catch (Exception e)
        {
            Log.Warning("Unable to GetUsersByRoleAsync with RoleName: {roleName} in ApplicationUserService: Exception thrown: {Exception}", roleName, e.ToString());
            return [];
        }
    }
    
    /// <inheritdoc />
    public async Task<IList<string>?> GetRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return null;

        return await _userManager.GetRolesAsync(user);
    }

    /// <inheritdoc />
    public async Task<List<ApplicationUser>> GetAllAsync()
    {
        try
        {
            return await _context.Users.ToListAsync();
        }
        catch (Exception e)
        {
            Log.Warning("Unable to GetAllAsync in ApplicationUserService: Exception thrown: {Exception}", e.ToString());
            return [];
        }
    }

    /// <inheritdoc />
    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        var applicationUser = await _context.Users.FindAsync(id);
        if (applicationUser != null)
        {
            Log.Information("Retrieved Application User: {ApplicationUser}", applicationUser.ToString());
        }
        else
        {
            Log.Information("Unable to retrieve Application User with ID: {userId}", id);
        }
        return applicationUser;
    }

    /// <inheritdoc />
    public async Task<bool?> IsNewUser(ApplicationUser user)
    {
        try
        {
            if (string.IsNullOrEmpty(user.UserName) && string.IsNullOrEmpty(user.Email))
            {
                return null;
            }
            
            _context.ChangeTracker.Clear();

            var existingUser = await _context.Users.FindAsync(user.Id);

            return existingUser == null;
        }
        catch (Exception e)
        {
            Log.Warning("Unable to determine if user is new: {Exception}", e.ToString());
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<ApplicationUser?> GetByLastNameAsync(string lastName)
    {
        try
        {
            var applicationUser = await _context.Users.FirstOrDefaultAsync(n => n.LastName == lastName);
            return applicationUser;
        }
        catch (Exception e)
        {
            Log.Warning("Unable to retrieve user by last name: {LastName}. Exception Thrown: {Exception}", lastName, e.ToString());
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<ApplicationUser?> GetByUserNameAsync(string userName)
    {
        try
        {
            var applicationUser = await _context.Users.FirstOrDefaultAsync(n => n.UserName == userName);
            return applicationUser;
        }
        catch (Exception e)
        {
            Log.Warning("Unable to retrieve user with UserName: {Username}. Exception Thrown: {Exception}", userName, e);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        try
        {
            var applicationUser = await _context.Users.FirstOrDefaultAsync(n => n.Email == email);
            return applicationUser;
        }
        catch (Exception e)
        {
            Log.Warning("Unable to find user with Email: {Email}. Exception Thrown: {Exception}", email, e.ToString());
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<IdentityResult> AddApplicationUser(ApplicationUser applicationUser)
    {
        try
        {
            var result = await _userManager.CreateAsync(applicationUser);
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(applicationUser, DefaultRole);
                Log.Information("Added ApplicationUser with ID {id}", applicationUser.Id);
                return IdentityResult.Success;
            }
            
            Log.Warning("Unable to create ApplicationUser with ID {id}", applicationUser.Id);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Could not add ApplicationUser");
            return IdentityResult.Failed();
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateApplicationUser(ApplicationUser applicationUser)
    {
        try
        {
            var strategy = _context.Database.CreateExecutionStrategy();
        
            return await strategy.ExecuteAsync(async () =>
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            
                var existingUser = await _context.Users.FirstOrDefaultAsync(
                    u => u.Id == applicationUser.Id);

                if (existingUser == null)
                {
                    Log.Error("Could not find user with ID {id}", applicationUser.Id);
                    return false;
                }

                applicationUser.NormalizedEmail = applicationUser.Email?.ToUpper();
                applicationUser.NormalizedUserName = applicationUser.UserName?.ToUpper();

                _context.Entry(existingUser).CurrentValues.SetValues(applicationUser);

                await _context.SaveChangesAsync();
                scope.Complete();

                Log.Information("Updated user with ID {id}", applicationUser.Id);
                return true;
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating user with ID {id}", applicationUser.Id);
            return false;
        }
    }
    
    /// <inheritdoc />
    public async Task<RegisterUserResult> RegisterAsync(RegisterUserRequest request)
    {
        try
        {
            var user = new ApplicationUser
            {
                UserName = request.Username.Trim(),
                Email = request.Email.Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim()
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)
            {
                return new RegisterUserResult(
                    Succeeded: false,
                    Errors: createResult.Errors,
                    UserId: null,
                    EmailConfirmationToken: null);
            }

            await _userManager.AddToRoleAsync(user, DefaultRole);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            Log.Information("Registered new user {UserId}", user.Id);

            return new RegisterUserResult(
                Succeeded: true,
                Errors: null,
                UserId: user.Id,
                EmailConfirmationToken: token);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error registering user");

            return new RegisterUserResult(
                Succeeded: false,
                Errors:
                [
                    new IdentityError { Description = "Registration failed." }
                ],
                UserId: null,
                EmailConfirmationToken: null);
        }
    }

    
    /// <inheritdoc />
    public async Task<LoginResult> PasswordLoginAsync(
        string login,
        string password,
        bool rememberMe)
    {
        // Try to find by username first
        // If not found, try email
        var user = await _userManager.FindByNameAsync(login) ?? await _userManager.FindByEmailAsync(login);

        // If still not found, fail early
        if (user == null)
        {
            return new LoginResult(LoginOutcome.Failed);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            password,
            rememberMe,
            lockoutOnFailure: false);

        return MapSignInResult(result);
    }
    
    private static LoginResult MapSignInResult(SignInResult result)
    {
        if (result.Succeeded)
            return new(LoginOutcome.Success);

        if (result.RequiresTwoFactor)
            return new(LoginOutcome.RequiresTwoFactor);

        if (result.IsLockedOut)
            return new(LoginOutcome.LockedOut);

        return new(LoginOutcome.Failed);
    }
    
    /// <inheritdoc />
    public async Task<PasswordResetRequest?> RequestPasswordResetAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            return null;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(token));

        return new PasswordResetRequest(email, encodedToken);
    }

    /// <inheritdoc />
    public async Task<EmailChangeConfirmationResult> ConfirmEmailChangeAsync(
        string userId,
        string newEmail,
        string encodedToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new(false, "Invalid email change request.");

        string token;
        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken));
        }
        catch
        {
            return new EmailChangeConfirmationResult(false, "Invalid confirmation token.");
        }

        var changeResult = await _userManager.ChangeEmailAsync(user, newEmail, token);
        if (!changeResult.Succeeded)
            return new EmailChangeConfirmationResult(false, "Error changing email.");

        var usernameResult = await _userManager.SetUserNameAsync(user, newEmail);
        if (!usernameResult.Succeeded)
            return new EmailChangeConfirmationResult(false, "Error updating username.");

        await _signInManager.RefreshSignInAsync(user);

        return new(true);
    }
    
    /// <inheritdoc />
    public async Task<EmailConfirmationResult> ConfirmEmailAsync(
        string userId,
        string encodedToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new EmailConfirmationResult(false, "User not found.");
        }

        var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken));
        var result = await _userManager.ConfirmEmailAsync(user, token);

        return result.Succeeded
            ? new EmailConfirmationResult(true)
            : new EmailConfirmationResult(false, "Error confirming your email.");
    }

    /// <inheritdoc />
    public async Task<ApplicationUser?> GetPendingTwoFactorUserAsync()
    {
        return await _signInManager.GetTwoFactorAuthenticationUserAsync();
    }
    
    /// <inheritdoc />
    public async Task<TwoFactorSignInResult> SignInWithAuthenticatorAsync(
        string twoFactorCode,
        bool rememberMe,
        bool rememberMachine)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user is null)
        {
            throw new InvalidOperationException(
                "No user is currently pending two-factor authentication.");
        }

        var normalizedCode = twoFactorCode
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty);

        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
            normalizedCode,
            rememberMe,
            rememberMachine);

        var userId = await _userManager.GetUserIdAsync(user);

        if (result.Succeeded)
        {
            Log.Information(
                "User with ID '{UserId}' logged in with 2FA.", userId);

            return new TwoFactorSignInResult(TwoFactorSignInOutcome.Success);
        }

        if (result.IsLockedOut)
        {
            Log.Warning(
                "User with ID '{UserId}' account locked out during 2FA.", userId);

            return new TwoFactorSignInResult(TwoFactorSignInOutcome.LockedOut);
        }

        Log.Warning(
            "Invalid authenticator code entered for user with ID '{UserId}'.", userId);

        return new TwoFactorSignInResult(
            TwoFactorSignInOutcome.InvalidCode,
            "Error: Invalid authenticator code.");
    }
    
    /// <inheritdoc />
    public async Task<IReadOnlyList<PasskeyDTO>?> GetPasskeysAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return null;

        var passkeys = await _userManager.GetPasskeysAsync(user);

        return passkeys
            .Select(p => new PasskeyDTO(
                WebEncoders.Base64UrlEncode(p.CredentialId),
                p.Name,
                p.CreatedAt))
            .ToList();
    }
    
    /// <inheritdoc />
    public async Task<AddPasskeyResult> AddPasskeyAsync(
        string userId,
        string credentialJson)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new AddPasskeyResult(false, "Invalid user.", null);

        var attestationResult =
            await _signInManager.PerformPasskeyAttestationAsync(credentialJson);

        if (!attestationResult.Succeeded)
            return new AddPasskeyResult(false, attestationResult.Failure.Message, null);

        var addResult =
            await _userManager.AddOrUpdatePasskeyAsync(user, attestationResult.Passkey);

        if (!addResult.Succeeded)
            return new AddPasskeyResult(false, "Could not store passkey.", null);

        var credentialId =
            WebEncoders.Base64UrlEncode(attestationResult.Passkey.CredentialId);

        return new AddPasskeyResult(true, null, credentialId);
    }

    
    /// <inheritdoc />
    public async Task<DeletePasskeyResult> DeletePasskeyAsync(
        string userId,
        string credentialIdBase64Url)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new(false, "Invalid user.");

        byte[] credentialId;

        try
        {
            credentialId = WebEncoders.Base64UrlDecode(credentialIdBase64Url);
        }
        catch
        {
            return new(false, "Invalid credential ID format.");
        }

        var result =
            await _userManager.RemovePasskeyAsync(user, credentialId);

        return result.Succeeded
            ? new DeletePasskeyResult(true, null)
            : new DeletePasskeyResult(false, "Failed to delete passkey.");
    }


    /// <inheritdoc />
    public async Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal principal)
    {
        return await _userManager.GetUserAsync(principal);
    }
    
    /// <inheritdoc />
    public async Task<UserProfileDTO?> GetProfileAsync(ClaimsPrincipal principal)
    {
        var user = await _userManager.GetUserAsync(principal);
        if (user is null)
            return null;

        return new UserProfileDTO(
            user.Id,
            user.UserName!,
            user.FirstName,
            user.LastName,
            user.PhoneNumber
        );
    }
    
    /// <inheritdoc />
    public async Task<UpdateUserProfileResult> UpdateProfileAsync(
        UpdateUserProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
            return new UpdateUserProfileResult(false, "User not found.");

        user.UserName = request.UserName;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var error = string.Join("; ", result.Errors.Select(e => e.Description));
            return new UpdateUserProfileResult(false, error);
        }

        await _signInManager.RefreshSignInAsync(user);

        return new UpdateUserProfileResult(true);
    }


    
}