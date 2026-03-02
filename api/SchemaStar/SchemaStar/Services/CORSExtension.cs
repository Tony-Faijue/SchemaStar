namespace SchemaStar.Services
{
    public static class CORSExtension
    {
        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services) 
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
