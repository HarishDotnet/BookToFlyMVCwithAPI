using FlightDetailApi.Data;
using FlightDetailApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FlightDetailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Ticket/{username}
        // Retrieves a user's ticket booking IDs.
        [HttpGet("{username}")]
        public async Task<IActionResult> GetTicket(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username is required.");
            }

            var ticket = await _context.Tickets.FindAsync(username);
            if (ticket == null)
            {
                return NotFound("No tickets found for this user.");
            }

            return Ok(new { TicketList = ticket.BookingId });
        }

        // POST: api/Ticket
        // Adds a new ticket booking for a user.
        [HttpPost]
        public async Task<IActionResult> AddTicket([FromQuery] string username, [FromQuery] int bookingId)
        {
            if (string.IsNullOrEmpty(username) || bookingId == 0)
            {
                return BadRequest("Username and booking ID are required.");
            }

            // Check if the user exists
            var isUser = await _context.Users.AnyAsync(u => u.Username == username);
            if (!isUser)
            {
                return BadRequest("Error: Username not found.");
            }

            // Find existing ticket entry for the user
            var ticketEntry = await _context.Tickets.FindAsync(username);

            if (ticketEntry != null)
            {
                // Add the booking ID to the user's existing list
                if (!ticketEntry.BookingId.Contains(bookingId))
                {
                    ticketEntry.BookingId.Add(bookingId);
                }
                else
                {
                    return BadRequest("This booking ID is already associated with the user.");
                }
            }
            else
            {
                // Create a new entry if none exists
                var newTicket = new TicketDetails { Username = username, BookingId = new List<int> { bookingId } };
                _context.Tickets.Add(newTicket);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully added ticket." });
        }
        // GET: api/Ticket/GetAllTickets
        // Retrieves all username-booking ID pairs.
        [HttpGet("GetAllTickets")]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _context.Tickets
                .Select(t => new { t.Username, BookingId = t.BookingId })
                .ToListAsync();

            if (tickets == null || !tickets.Any())
            {
                return NotFound("No ticket data available.");
            }

            return Ok(tickets);
        }

    }
}
