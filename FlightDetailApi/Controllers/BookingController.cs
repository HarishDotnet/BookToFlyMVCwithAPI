using FlightDetailApi.Data;
using FlightDetailApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightDetailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlightHelper _flightHelper;

        public BookingController(ApplicationDbContext context, IFlightHelper flightHelper)
        {
            _context = context;
            _flightHelper = flightHelper;
        }

        // GET: api/Booking
        // Retrieves all booking records from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDetails>>> GetBookings()
        {
            return await _context.Booking.ToListAsync();
        }

        // GET: api/Booking/{id}
        // Retrieves a specific booking by ID along with its flight details.
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDetails>> GetBooking(string id)
        {
            var booking = await _context.Booking.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            // Determine flight type based on ID prefix
            string flightType = id.StartsWith("IF") ? "International" : "Domestic";

            // Fetch flight details using FlightHelper
            var flight = await _flightHelper.GetFlightByIdAsync(booking.FlightId);

            return Ok(new { booking, flight, FlightType = flightType });
        }

        // POST: api/Booking
        // Creates a new booking entry.
        [HttpPost]
        public async Task<ActionResult<BookingDetails>> PostBooking(BookingDetails booking)
        {
            // Validate model state before proceeding
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }

            try
            {
                // Add the booking object to the database
                _context.Booking.Add(booking);

                // Decrement available seats for the flight
                bool seatsUpdated = await _flightHelper.DecrementAvailableSeats(booking.FlightId);

                if (!seatsUpdated)
                {
                    return BadRequest("No seats available for this flight or flight not found.");
                }

                // Save changes to both booking and flight data
                await _context.SaveChangesAsync();
                return Ok(booking);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  // Return error message in case of failure
            }
        }

        // PUT: api/Booking/{id}
        // Cancels a booking by updating its status.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(string id)
        {
            var booking = await _context.Booking.FindAsync(id);
            try
            {
                if (booking != null)
                {
                    booking.IsCanceled = true;  // Mark the booking as canceled
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Ticket Canceled Successfully" });
                }
                else
                {
                    return NotFound();  // Return NotFound if booking doesn't exist
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex);
            }

            return NoContent(); // Return NoContent if an exception occurs
        }

        // DELETE: api/Booking/{id}
        // Deletes a booking from the database.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Booking.Remove(booking); // Remove the booking from the database
            await _context.SaveChangesAsync();

            return NoContent(); // Indicate successful deletion
        }
    }
}
