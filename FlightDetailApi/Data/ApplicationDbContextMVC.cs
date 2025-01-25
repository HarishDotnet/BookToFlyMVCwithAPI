using FlightDetailsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightDetailsApi.Data
{
    public class ApplicationDbContextMVC : DbContext
    {
        public ApplicationDbContextMVC(DbContextOptions<ApplicationDbContextMVC> options) : base(options)
        {
        }
        public DbSet<AdminModel> admin {get; set;}
    }
}
