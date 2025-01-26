using FlightDetailApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightDetailApi.Data
{
    public class ApplicationDbContextMVC : DbContext
    {
        public ApplicationDbContextMVC(DbContextOptions<ApplicationDbContextMVC> options) : base(options)
        {
        }
        public DbSet<AdminModel> admin {get; set;}
    }
}
