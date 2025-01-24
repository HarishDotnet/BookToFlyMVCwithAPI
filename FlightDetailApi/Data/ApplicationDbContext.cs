using FlightDetailsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightDetailsApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<InternationalFlightDetails> InternationalFlightDetails { get; set; }
        public DbSet<DomesticFlightDetails> DomesticFlightDetails { get; set; }

        // Override OnModelCreating to specify precision and scale for decimal properties
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // For DomesticFlightDetails entity
            modelBuilder.Entity<DomesticFlightDetails>()
                .Property(f => f.TicketPrice)
                .HasPrecision(18, 2);  // Specify precision 18 and scale 2

            // For InternationalFlightDetails entity
            modelBuilder.Entity<InternationalFlightDetails>()
                .Property(f => f.TicketPrice)
                .HasPrecision(18, 2);  // Specify precision 18 and scale 2
        }
    }
}
