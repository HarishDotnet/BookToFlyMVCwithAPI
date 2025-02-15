using FlightDetailApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightDetailApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<InternationalFlightDetails> InternationalFlightDetails { get; set; }
        public DbSet<DomesticFlightDetails> DomesticFlightDetails { get; set; }
        public DbSet<UserRegistrationModel> Users { get; set; }
        public DbSet<TicketDetails> Tickets { get; set; }
        public DbSet<BookingDetails> Booking { get; set; }
        public DbSet<AdminModel> Admin { get; set; }

        // Override OnModelCreating to specify precision and scale for decimal properties
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            base.OnModelCreating(modelBuilder);
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
