namespace BookToFlyMVC.Configurations
{
    public static class LoggingConfig
    {
        public static void ConfigureLogging(this IServiceCollection services)
        {
            var logger = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }).CreateLogger<Program>();

            services.AddSingleton(logger);
        }
    }
}