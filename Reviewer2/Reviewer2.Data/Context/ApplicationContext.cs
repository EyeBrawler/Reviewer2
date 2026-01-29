using Microsoft.EntityFrameworkCore;

namespace Reviewer2.Data.Context;

public class ApplicationContext : DbContext
{
    /// <inheritdoc />
    public ApplicationContext()
    {
        
    }
    /// <inheritdoc />
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        
    }
}