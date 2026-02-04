using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Reviewer2.Services.CRUD.ApplicationUser;

/// <summary>
/// Creates the associated roles within the Database via Microsoft Identity
/// </summary>
public class RoleInitializer
{
    private readonly RoleManager<IdentityRole> _roleManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleInitializer"/> class.
    /// </summary>
    /// <param name="roleManager">
    /// The <see cref="RoleManager{IdentityRole}"/> used to manage roles in the database.
    /// </param>
    public RoleInitializer(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    /// <summary>
    /// Initializes roles in the database if they do not already exist.
    /// </summary>
    /// <remarks>
    /// This method iterates through a predefined list of roles and checks if each role exists in the database.
    /// If a role does not exist, it creates the role using the RoleManager.
    /// </remarks>
    public async Task InitializeAsync()
    {
        string[] roles = ["Attendee", "Paper Chair", "Conference Chair"];

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}