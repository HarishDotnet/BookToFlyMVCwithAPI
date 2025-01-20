using FlightDetailsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightDetailsApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
        }   
        public DbSet<InternationalFlightDetails> InternationalFlightDetails { get; set; }
        public DbSet<DomesticFlightDetails> DomesticFlightDetails { get; set; }

    }
    }
