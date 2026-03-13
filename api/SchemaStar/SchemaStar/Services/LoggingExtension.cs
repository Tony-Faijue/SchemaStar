using Serilog;

namespace SchemaStar.Services
{
    public static class LoggingExtension
    {
        public static void AddLoggingConfiguration(this WebApplicationBuilder builder) 
        {
            builder.Services.AddSerilog((services, lc) => lc
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
            );
        }
    }
}
