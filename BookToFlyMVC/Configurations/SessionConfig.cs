namespace BookToFlyMVC.Configurations
{
    public static class SessionConfig
    {
        /// <summary>
        /// Configures session management for the application.
        /// Sets session timeout, ensures cookies are HTTP-only, and marks them as essential.
        /// </summary>
        /// <param name="services">The IServiceCollection to add session services to.</param>
        public static void ConfigureSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                // Sets the session timeout duration to 30 minutes
                options.IdleTimeout = TimeSpan.FromMinutes(30);

                // Ensures that the session cookie is accessible only via HTTP (not JavaScript)
                options.Cookie.HttpOnly = true;

                // Marks the session cookie as essential, ensuring it's always stored
                options.Cookie.IsEssential = true;
            });
        }
    }
}
