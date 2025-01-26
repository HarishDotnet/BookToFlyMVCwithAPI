using BookToFlyMVC.Middleware;

namespace BookToFlyMVC.Configurations
{
    public static class MiddlewareConfig
    {
        public static void ConfigureMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionHandlingMiddleware>();
        }
    }
}