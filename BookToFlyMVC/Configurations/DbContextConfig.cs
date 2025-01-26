using BookToFlyMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace BookToFlyMVC.Configurations
{
    public static class DbContextConfig
    {
        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
