using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using SchemaStar.Models;
using SchemaStar.Services;
using System.Data.Common;
using Testcontainers.MySql;

namespace SchemaStar.IntegrationTests.Factory
{
    public class CustomizeWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
    {
        /// <summary>
        /// MySql Container Instance Created
        /// </summary>
        private readonly MySqlContainer _dbContainer = new MySqlBuilder("mysql:8.0.44")
                .WithDatabase("schemastar_test")
                .WithUsername("test")
                .WithPassword("test")
                .Build();

        /// <summary>
        /// Starts the DBContainer;
        /// Does the initial seeding of the test user
        /// </summary>
        /// <returns></returns>
        public async ValueTask InitializeAsync() 
        {
            await _dbContainer.StartAsync();

                //create scope for dbcontext and services to prevent memory leaks/zombie data 
                using var scope = Services.CreateScope();
                //Get same instances for services, schemastar context, and user manager used in the application
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<SchemastarContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();
                //create the database tables based on the Docker container
                await context.Database.EnsureCreatedAsync();

                //Seed the user
                var testUserEmail = "test@example.com";
                var user = userManager.FindByEmailAsync(testUserEmail).GetAwaiter().GetResult(); //get result synchronously
                if (user == null)
                {
                    var newUser = new User
                    {
                        UserName = "testuser",
                        Email = testUserEmail,
                        PublicId = Guid.NewGuid().ToMySqlBinary()
                    };
                    var result = userManager.CreateAsync(newUser, "Password123!").GetAwaiter().GetResult(); //get result synchronously
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to seed test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
        }

        /// <summary>
        /// Stops the DBContainer
        /// </summary>
        /// <returns></returns>
        public override async ValueTask DisposeAsync() 
        {
            await _dbContainer.StopAsync();
            await base.DisposeAsync(); //call base dispose async method
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services => 
            {
                //Remove the MySQL dbcontextoptions
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<SchemastarContext>));
                if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);

                //Register the MySql provider with docker container
                services.AddDbContext<SchemastarContext>(options =>
                {
                    options
                        .UseMySQL(_dbContainer.GetConnectionString());
                });
            });

            builder.UseEnvironment("Testing");
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            //create the application based on the configurartion of ConfigureWebHost method
            var host = base.CreateHost(builder);
            return host;
        } 
    }
}
