using Microsoft.AspNetCore.Identity;
using SchemaStar.Models;

namespace SchemaStar.Services
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services) 
        { 
            services.AddIdentity<User, IdentityRole<ulong>>(options => 
            {
                //Password requirements
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                
                //Require unique email
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<SchemastarContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
