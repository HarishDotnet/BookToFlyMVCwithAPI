using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookToFlyMVC.Configurations
{
    public static class SessionConfig
    {
        public static void ConfigureSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }
    }

}