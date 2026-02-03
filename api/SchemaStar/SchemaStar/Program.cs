using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SchemaStar.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Register the Dbcontext classes in DI Container
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection String not found");
//Register with MySQL
builder.Services.AddDbContext<SchemastarContext>(o => o.UseMySQL(connectionString));

//Register Problem Details Service for API Errors
builder.Services.AddProblemDetails();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Add Middleware so that http status codes that do not return a JSON body will return a JSON body
app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
