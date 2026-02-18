using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Reviewer2.Data.Context
{
    /// <summary>
    /// Used to create a design time DbContext to run migrations via shell
    /// </summary>
    public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
    {
        /// <summary>
        /// Creates the design time DbContext
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ApplicationContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<ApplicationContextFactory>() // <-- This line adds user secrets
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            var connectionString = configuration.GetConnectionString("Reviewer2Connection");

            optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
            {
                // Optional but recommended
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });

            return new ApplicationContext(optionsBuilder.Options);
        }
        
        /// <summary>
        /// Provides a design-time factory for creating instances of <see cref="UserContext"/>.
        /// 
        /// This factory is used by Entity Framework Core tooling (e.g., migrations)
        /// when the application’s normal dependency injection pipeline is not available.
        /// 
        /// It enables commands such as:
        ///     dotnet ef migrations add
        ///     dotnet ef database update
        /// 
        /// The factory builds configuration manually and configures the DbContext
        /// with the appropriate connection string.
        /// </summary>
        public class UserContextFactory
            : IDesignTimeDbContextFactory<UserContext>
        {
            /// <summary>
            /// Creates a new instance of <see cref="UserContext"/> for design-time operations.
            /// 
            /// This method is invoked by EF Core tools at migration-time. It:
            /// - Loads configuration from appsettings.json
            /// - Includes user secrets (for local development)
            /// - Configures the DbContext to use PostgresSQL via Npgsql
            /// 
            /// The connection string must exist under the name "Reviewer2Connection".
            /// </summary>
            /// <param name="args">
            /// Command-line arguments supplied by the EF Core tooling.
            /// </param>
            /// <returns>
            /// A configured instance of <see cref="UserContext"/>.
            /// </returns>
            public UserContext CreateDbContext(string[] args)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddUserSecrets<UserContextFactory>()
                    .Build();

                var options = new DbContextOptionsBuilder<UserContext>()
                    .UseNpgsql(config.GetConnectionString("Reviewer2Connection"))
                    .Options;

                return new UserContext(options);
            }
        }
    }
}