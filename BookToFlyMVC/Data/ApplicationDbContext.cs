using BookToFlyMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BookToFlyMVC.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> Options):base(Options)
        {   }
        public DbSet<UserRegistrationViewModel> User {get; set;}
        public DbSet<AdminRegistrationViewModel> Admin {get; set;}
        public DbSet<InternationalFlightModel> InternationalFlights {get; set;}
        public DbSet<DomesticFlightModel> DomesticFlights  {get; set;}
        public DbSet<BookedTicket> bookedTickets {get; set;}
    }
}