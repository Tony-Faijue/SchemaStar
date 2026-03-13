using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SchemaStar.ExceptionHandlers;
using SchemaStar.JWT;
using SchemaStar.Models;
using SchemaStar.Services;
using Serilog;

//Bootstrap logger for logging initial project setup errors
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("SchemaStar is starting up...");

    var builder = WebApplication.CreateBuilder(args);

    //Call Logging extension method to register Serilog as logger
    builder.AddLoggingConfiguration();

    // Add services to the container.

    //-------------Controllers & Open API Services-------------

    builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    // Add HttpContextAccessor to access the current HTTP request and response
    builder.Services.AddHttpContextAccessor();

    //-------------Added UserService-------------
    builder.Services.AddScoped<IUserService, UserService>();

    //-------------Database Connection String & Database Context-------------

    //Register the Dbcontext classes in DI Container
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection String not found");
    //Register with MySQL
    builder.Services.AddDbContext<SchemastarContext>(o => o.UseMySQL(connectionString));

    //Register AspNetCore Identity
    builder.Services.AddIdentityServices();

    //-------------Problem Details Service-------------

    //Register Problem Details Service for API Errors
    builder.Services.AddProblemDetails();

    //-------Register the GlobalExceptionHandler

    //Custom Global Exception Handler for HTTP Status Codes
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    //JWT Options & Authentications
    builder.Services.AddJwtAuthentication(builder.Configuration);

    //CORS configuration
    builder.Services.AddCorsConfiguration();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    //Serilog http request logging middleware
    app.UseSerilogRequestLogging();

    //Add exception handler in the middleware
    app.UseExceptionHandler();

    // Add Middleware so that the http status codes that do not return a JSON body will return a JSON body
    app.UseStatusCodePages();

    app.UseHttpsRedirection();

    //Middleware Custom Cors Policy 
    app.UseCors("AllowAngular");

    //Added Authentication
    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "SchemaStar terminated unexpectedly");
}
finally 
{
    Log.CloseAndFlush();
}