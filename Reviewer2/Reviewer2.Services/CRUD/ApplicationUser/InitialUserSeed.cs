namespace Reviewer2.Services.CRUD.ApplicationUser;

using System;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides methods for seeding initial user data into the database.
/// </summary>
public static class InitialUserSeed
{
    /// <summary>
    /// Seeds a default user with Technician permissions if the user does not already exist.
    /// NOTE: The password of this user should be changed as soon as possible once the application is running.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SeedDefaultUserAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        const string email = "admin@reviewer2";

        // Check if user already exists
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}