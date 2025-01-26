using BookToFlyMVC.DTO;
using BookToFlyMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BookToFlyMVC.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> Options):base(Options)
        {   }
        public DbSet<UserRegistrationViewModel> User {get; set;}
        public DbSet<BookedTicket> bookedTickets {get; set;}
        public DbSet<LoggedUsers> LoggedUsers {get ; set;} 
        public DbSet<LoginDTO> admin {get ; set;} 
    }
}