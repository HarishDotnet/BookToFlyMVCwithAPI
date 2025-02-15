namespace BookToFlyMVC.Configurations
{
    public static class LoggingConfig
    {
        /// <summary>
        /// Configures logging for the application.
        /// Adds console logging and registers the logger as a singleton service.
        /// </summary>
        /// <param name="services">The IServiceCollection to add logging services to.</param>
        public static void ConfigureLogging(this IServiceCollection services)
        {
            // Creates a logger factory that adds console logging
            var logger = LoggerFactory.Create(builder =>
            {
                builder.AddConsole(); // Enables logging output to the console
            }).CreateLogger<Program>();

            // Registers the logger as a singleton service to be used throughout the application
            services.AddSingleton(logger);
        }
    }
}
