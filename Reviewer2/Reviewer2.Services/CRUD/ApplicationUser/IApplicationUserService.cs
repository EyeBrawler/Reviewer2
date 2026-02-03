using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

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
    ///<summary>
    /// Retrieves a list of all ApplicationUsers currently registered
    ///</summary>
    ///<returns>List of ApplicationUser objects</returns>
    public Task<List<ApplicationUser>> GetAllAsync();
    
    ///<summary>
    /// Retrieves a ApplicationUser by id number
    ///</summary>
    ///<returns>ApplicationUser object</returns>
    public Task<ApplicationUser?> GetByIdAsync(string id);

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
}