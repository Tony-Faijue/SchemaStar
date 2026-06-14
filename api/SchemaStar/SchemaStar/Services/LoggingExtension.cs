using Serilog;

namespace SchemaStar.Services
{
    public static class LoggingExtension
    {
        public static void AddLoggingConfiguration(this WebApplicationBuilder builder) 
        {
            // Use the UseSerilog extension method to configure Serilog as the logging provider for the application
            builder.Host.UseSerilog((context, services, lc) => lc
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
            );
        }
    }
}
