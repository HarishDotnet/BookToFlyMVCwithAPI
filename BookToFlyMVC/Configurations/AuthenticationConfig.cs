using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BookToFlyMVC.Configurations
{
    public static class AuthenticationConfig
    {
        /// <summary>
        /// Configures authentication services for the application.
        /// Supports both Cookie-based authentication and JWT authentication.
        /// </summary>
        /// <param name="services">The IServiceCollection to add authentication services to.</param>
        public static void ConfigureAuthentication(this IServiceCollection services)
        {
            // Configures Cookie-based authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    // Sets the login path to the User/Login page
                    options.LoginPath = "/User/Login";
                    // Sets the logout path to the User/Logout page
                    options.LogoutPath = "/User/Logout";
                });

            // Configures JWT-based authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // Specifies the authority that issues JWT tokens
                    options.Authority = "http://localhost:5087";
                    // Specifies the audience that the JWT token is intended for
                    options.Audience = "https://localhost:7202";
                    // Disables HTTPS metadata requirement (should be enabled in production)
                    options.RequireHttpsMetadata = false;
                });
        }
    }
}
