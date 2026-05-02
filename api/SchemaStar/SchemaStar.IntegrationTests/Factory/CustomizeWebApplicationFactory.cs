using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SchemaStar.Models;
using SchemaStar.Services;
using System.Data.Common;

namespace SchemaStar.IntegrationTests.Factory
{
    public class CustomizeWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services => 
            {
                //Remove the MySQL options
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<SchemastarContext>));

                if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);

                //Create the SQLite connection
                services.AddSingleton<DbConnection>(container => 
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();
                    return connection;
                });

                //Register the SQlite connection
                services.AddDbContext<SchemastarContext>((container, options) => 
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                });

                //Initialize the SQlite database schema based on SchemaStarContext
                var sp = services.BuildServiceProvider();

                Task.Run(async () =>
                {
                    using var scope = sp.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<SchemastarContext>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>(); //for user factory for identity

                    //Create the tables
                   await context.Database.EnsureCreatedAsync();

                    //Seed the Test User
                    var testUserEmail = "test@example.com";
                    var user = await userManager.FindByEmailAsync(testUserEmail);

                    if (user == null)
                    {
                        var newUser = new User
                        {
                            UserName = "testuser",
                            Email = testUserEmail,
                            PublicId = Guid.NewGuid().ToMySqlBinary()
                        };

                        var result = userManager.CreateAsync(newUser, "Password123!").Result;

                        if (!result.Succeeded)
                        {
                            throw new Exception($"Failed to seed test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        }
                    }

                }).GetAwaiter().GetResult(); //Wait for the async to finish seeding
            });

            builder.UseEnvironment("Development");
        }
    }
}
