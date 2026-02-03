using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Context;
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
}
