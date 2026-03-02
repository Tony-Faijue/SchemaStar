using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SchemaStar.ExceptionHandlers;
using SchemaStar.JWT;
using SchemaStar.Models;
using SchemaStar.Services;

var builder = WebApplication.CreateBuilder(args);

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
