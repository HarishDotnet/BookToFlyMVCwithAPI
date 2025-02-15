using BookToFlyMVC.Handlers;

namespace BookToFlyMVC.Configurations
{
    public static class HttpClientConfig
    {
        /// <summary>
        /// Configures an HTTP client for making API requests.
        /// Sets the base address and attaches a custom message handler for token management.
        /// </summary>
        /// <param name="services">The IServiceCollection to add the HTTP client to.</param>
        public static void ConfigureHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient("FlightClient", client =>
            {
                // Sets the base address for the HTTP client to communicate with the flight API
                client.BaseAddress = new Uri("http://localhost:5087/api/");
            })
            // Attaches a custom message handler for handling token-related operations
            .AddHttpMessageHandler<TokenHandler>();
        }
    }
}
