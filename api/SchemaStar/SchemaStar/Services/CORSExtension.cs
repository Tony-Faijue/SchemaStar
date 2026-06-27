namespace SchemaStar.Services
{
    public static class CORSExtension
    {
        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration) 
        {
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
