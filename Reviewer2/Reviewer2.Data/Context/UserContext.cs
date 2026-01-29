using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Reviewer2.Data.Models;

namespace Reviewer2.Data.Context;

// <inheritdoc />
public class UserContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    /// <inheritdoc />
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }
}