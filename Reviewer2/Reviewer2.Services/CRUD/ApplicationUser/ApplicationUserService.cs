using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
using Reviewer2.Services.DTOs.ApplicationUser;
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
    public async Task<RegisterUserResult> RegisterAsync(
        string email,
        string password)
    {
        try
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return new RegisterUserResult(
                    false,
                    result.Errors,
                    null,
                    null);
            }

            await _userManager.AddToRoleAsync(user, DefaultRole);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            Log.Information("Registered new user {UserId}", user.Id);

            return new RegisterUserResult(
                true,
                null,
                user,
                token);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error registering user");
            return new RegisterUserResult(
                false,
                [new IdentityError { Description = "Registration failed." }],
                null,
                null);
        }
    }
    
    /// <inheritdoc />
    public async Task<LoginResult> PasswordLoginAsync(
        string email,
        string password,
        bool rememberMe)
    {
        var result = await _signInManager.PasswordSignInAsync(
            email,
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


}
