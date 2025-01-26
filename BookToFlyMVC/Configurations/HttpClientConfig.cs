using BookToFlyMVC.Handlers;

namespace BookToFlyMVC.Configurations
{
    public static class HttpClientConfig
    {
        public static void ConfigureHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient("FlightClient", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5087/api/");
            })
            .AddHttpMessageHandler<TokenHandler>();
        }
    }

}