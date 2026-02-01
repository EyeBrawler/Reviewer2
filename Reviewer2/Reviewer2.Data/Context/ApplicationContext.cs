using Microsoft.EntityFrameworkCore;

namespace Reviewer2.Data.Context;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    // DbSet<Paper>, DbSet<Review>, etc.
}