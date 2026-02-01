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
        
        public class UserContextFactory
            : IDesignTimeDbContextFactory<UserContext>
        {
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