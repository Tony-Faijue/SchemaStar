using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SchemaStar.JWT;

namespace SchemaStar.Services
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            //Add and Validate JWTOptions
            // Validate JWTOptions
            services.AddOptions<JWTOptions>()
                .Bind(configuration.GetSection(JWTOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            // Get JWT Settings and the key for Authentication
            var jwtSettings = configuration.GetSection(JWTOptions.SectionName).Get<JWTOptions>()
                ?? throw new InvalidOperationException("JWT Settings are missing");
            var key = System.Text.Encoding.UTF8.GetBytes(jwtSettings.Key);

            //--- Register Authentication Services ----
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options => 
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    };

                    //Tell JWT to read from the Cookie for HTTP Only Cookie
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.ContainsKey("X-Access-Token"))
                            {
                                context.Token = context.Request.Cookies["X-Access-Token"];
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }
    }
}
