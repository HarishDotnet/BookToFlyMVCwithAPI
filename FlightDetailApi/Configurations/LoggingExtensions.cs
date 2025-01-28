using Microsoft.Extensions.Hosting;
using Serilog;

namespace FlightDetailApi.Configurations
{
    public static class LoggingExtensions
    {
        public static void ConfigureLogging(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration) // Read settings from appsettings.json
                    .Enrich.FromLogContext(); // Enrich logs with context information
            });
        }
    }
}
