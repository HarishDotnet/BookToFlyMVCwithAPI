using FlightDetailApi.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightDetailApi.Configurations
{
    public static class DbContextExtensions
{
    public static void AddDatabaseContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        // services.AddDbContext<ApplicationDbContextMVC>(options =>
        //     options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionMVC")));
    }
}

}