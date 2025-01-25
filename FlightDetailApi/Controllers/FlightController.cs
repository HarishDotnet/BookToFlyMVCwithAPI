using Microsoft.AspNetCore.Mvc;
using FlightDetailsApi.DTO;
using FlightDetailsApi.Models;
using FlightDetailsApi.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace FlightDetailsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public FlightController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("AddFlights")]
        public async Task<IActionResult> AddFlights([FromBody] List<FlightInputDTO> flightInputs)
        {
            if (flightInputs == null || !flightInputs.Any())
            {
                return BadRequest("The flight list cannot be empty.");
            }

            var invalidFlights = new List<string>();
            foreach (var flightInput in flightInputs)
            {
                if (!ModelState.IsValid)
                {
                    invalidFlights.Add(flightInput.FlightId ?? "Unknown FlightId");
                    continue;
                }

                // Check if the FlightId already exists in the database
                bool flightExists = await _context.InternationalFlightDetails
                    .AnyAsync(f => f.FlightId == flightInput.FlightId) ||
                    await _context.DomesticFlightDetails
                    .AnyAsync(f => f.FlightId == flightInput.FlightId);

                if (flightExists)
                {
                    invalidFlights.Add(flightInput.FlightId);
                    continue;
                }

                // Add the flight based on FlightType
                if (flightInput.FlightId.StartsWith("IF"))
                {
                    flightInput.FlightType = "International";
                    var flight = _mapper.Map<InternationalFlightDetails>(flightInput);
                    _context.InternationalFlightDetails.Add(flight);
                }
                else if (flightInput.FlightId.StartsWith("DF"))
                {
                    flightInput.FlightType = "Domestic";
                    var flight = _mapper.Map<DomesticFlightDetails>(flightInput);
                    _context.DomesticFlightDetails.Add(flight);
                }
                else
                {
                    invalidFlights.Add(flightInput.FlightId);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while saving the flights: {ex.Message}");
            }

            if (invalidFlights.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "Some flights could not be added due to validation errors or duplication.",
                    invalidFlights
                });
            }

            return Ok("All flights added successfully.");
        }


        [HttpPost("AddFlight")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddFlight([FromBody] FlightInputDTO flightInput)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the FlightId already exists in the database
            bool flightExists = await _context.InternationalFlightDetails
                .AnyAsync(f => f.FlightId == flightInput.FlightId);

            if (flightExists)
            {
                return BadRequest($"Flight with FlightId {flightInput.FlightId} already exists.");
            }

            if (flightInput.FlightId.StartsWith("IF"))
            {
                flightInput.FlightType = "International";
                var flight = _mapper.Map<InternationalFlightDetails>(flightInput);
                _context.InternationalFlightDetails.Add(flight);
            }
            else if (flightInput.FlightId.StartsWith("DF"))
            {
                flightInput.FlightType = "Domestic";
                var flight = _mapper.Map<DomesticFlightDetails>(flightInput);
                _context.DomesticFlightDetails.Add(flight);
            }
            else
            {
                return BadRequest("Invalid flight type.");
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Flight added successfully.");
            }
            catch (DbUpdateException ex)
            {
                // Log the exception here
                return StatusCode(500, $"An error occurred while saving the flight: {ex.Message}");
            }
        }

        [HttpDelete("DeleteFlight/{flightId}")]
        public async Task<IActionResult> DeleteFlight(string flightId)
        {
            object flightToDelete = null;
            string successMessage = "Flight deleted successfully.";

            // Determine the flight type and fetch the corresponding flight
            if (flightId.StartsWith("IF"))
            {
                flightToDelete = await _context.InternationalFlightDetails
                    .FirstOrDefaultAsync(f => f.FlightId == flightId);
            }
            else if (flightId.StartsWith("DF"))
            {
                flightToDelete = await _context.DomesticFlightDetails
                    .FirstOrDefaultAsync(f => f.FlightId == flightId);
            }

            // If the flight exists, delete it
            if (flightToDelete != null)
            {
                _context.Remove(flightToDelete);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = successMessage });
            }

            return Ok(new { success = false, message = "Flight Not Deleted." });
        }


        [HttpPut("UpdateFlight/{flightId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFlight(string flightId, [FromBody] FlightInputDTO flightInput)
        {
            Console.WriteLine("Hi " + flightInput.AirlineName);
            Object flight = await _context.InternationalFlightDetails
                .FirstOrDefaultAsync(f => f.FlightId == flightId);

            if (flight == null)
            {
                flight = await _context.DomesticFlightDetails
                    .FirstOrDefaultAsync(f => f.FlightId == flightId);

                if (flight == null)
                    return NotFound("Flight not found.");
            }

            // Now we handle the case where we have the correct flight type
            if (flightInput.FlightType == "International" && flight is InternationalFlightDetails internationalFlight)
            {
                // Map the DTO to the correct international flight
                _mapper.Map(flightInput, internationalFlight);
            }
            else if (flightInput.FlightType == "Domestic" && flight is DomesticFlightDetails domesticFlight)
            {
                // Map the DTO to the correct domestic flight
                _mapper.Map(flightInput, domesticFlight);
            }
            else
            {
                return BadRequest("Flight type mismatch.");
            }

            await _context.SaveChangesAsync();
            return Ok("Flight updated successfully.");
        }
        [HttpGet("DisplayFlightByType")]
        public async Task<IActionResult> DisplayFlightByType([FromQuery] string FlightType, [FromQuery] string flightNumber)
        {
            

            // Ensure FlightType is provided
            if (string.IsNullOrEmpty(FlightType))
            {
                return BadRequest("FlightType is required.");
            }

            // Declare the list for flight details
            List<FlightOutputDTO> flightLists = null;

            // Determine the correct DbSet based on FlightType
            if (FlightType.Equals("International", StringComparison.OrdinalIgnoreCase))
            {
                var flightList = await _context.InternationalFlightDetails.ToListAsync(); // Fetch all International flights
                flightLists = _mapper.Map<List<FlightOutputDTO>>(flightList);
            }
            else if (FlightType.Equals("Domestic", StringComparison.OrdinalIgnoreCase))
            {
                var flightList = await _context.DomesticFlightDetails.ToListAsync(); // Fetch all Domestic flights
                flightLists = _mapper.Map<List<FlightOutputDTO>>(flightList);
            }
            else
            {
                return BadRequest("Invalid FlightType. Please specify 'International' or 'Domestic'.");
            }

            // If flightNumber is provided, search for that specific flight number in the fetched list
            if (!string.IsNullOrEmpty(flightNumber))
            {
                var flight = flightLists
                    .FirstOrDefault(f => f.FlightId.Equals(flightNumber, StringComparison.OrdinalIgnoreCase));

                if (flight == null)
                {
                    return NotFound("Flight not found with the provided Flight Number.");
                }

                // Map to DTO and return the flight details
                var flightOutput = _mapper.Map<FlightOutputDTO>(flight);
                return Ok(flightOutput);
            }

            // If flightNumber is not provided, return all flights of the specified type
            if (flightLists != null)
            {
                // Map the list of flights to DTOs
                var flightOutput = _mapper.Map<List<FlightOutputDTO>>(flightLists);
                return Ok(flightOutput);
            }
            else
            {
                return NotFound("No flights found for the specified Flight Type.");
            }
        }

        [HttpGet("DisplayFlightByType/{FlightType}")]
        public async Task<IActionResult> DisplayFlightByType(string FlightType)
        {

            // Ensure FlightType is provided
            if (string.IsNullOrEmpty(FlightType))
            {
                return BadRequest("FlightType is required.");
            }

            // Declare the list for flight details
            List<FlightOutputDTO> flightLists = null;

            // Determine the correct DbSet based on FlightType
            if (FlightType.Equals("International", StringComparison.OrdinalIgnoreCase))
            {
                var flightList = await _context.InternationalFlightDetails.ToListAsync(); // Fetch all International flights
                flightLists = _mapper.Map<List<FlightOutputDTO>>(flightList);
                if (flightLists != null)
                    return Ok(flightLists);
                else
                    return NoContent();
            }
            else if (FlightType.Equals("Domestic", StringComparison.OrdinalIgnoreCase))
            {
                var flightList = await _context.DomesticFlightDetails.ToListAsync(); // Fetch all Domestic flights
                flightLists = _mapper.Map<List<FlightOutputDTO>>(flightList);
                if (flightLists != null)
                    return Ok(flightLists);
                else
                    return NoContent();
            }
            else
            {
                return BadRequest("Invalid FlightType. Please specify 'International' or 'Domestic'.");
            }
        }


        [HttpPost("DisplayFlightBySourceAndDestination")]
        public async Task<IActionResult> GetFlightsBySourceAndDestination(FlightSearchInput searchInput)
        {
            if (string.IsNullOrWhiteSpace(searchInput.Source) || string.IsNullOrWhiteSpace(searchInput.Destination) || string.IsNullOrWhiteSpace(searchInput.FlightType))
            {
                return BadRequest("FlightType, Source, and Destination must be provided.");
            }

            List<FlightOutputDTO> allFlights = new List<FlightOutputDTO>();

            if (searchInput.FlightType.Equals("International", StringComparison.OrdinalIgnoreCase))
            {
                // Fetch international flights matching the criteria
                var internationalFlights = await _context.InternationalFlightDetails
                    .Where(f => f.Source.ToLower() == searchInput.Source.ToLower()
                             && f.Destination.ToLower() == searchInput.Destination.ToLower())
                    .ToListAsync();
                allFlights.AddRange(_mapper.Map<List<FlightOutputDTO>>(internationalFlights));
            }
            else if (searchInput.FlightType.Equals("Domestic", StringComparison.OrdinalIgnoreCase))
            {
                // Fetch domestic flights matching the criteria
                var domesticFlights = await _context.DomesticFlightDetails
                    .Where(f => f.Source.ToLower() == searchInput.Source.ToLower()
                             && f.Destination.ToLower() == searchInput.Destination.ToLower())
                    .ToListAsync();
                allFlights.AddRange(_mapper.Map<List<FlightOutputDTO>>(domesticFlights));
            }
            else
            {
                return BadRequest("Invalid FlightType. It must be either 'International' or 'Domestic'.");
            }

            if (!allFlights.Any())
            {
                return NotFound("No flights found for the specified source, destination, and flight type.");
            }

            return Ok(allFlights);
        }

        [HttpGet("DisplayAllFlightNumbersByType")]
        public async Task<IActionResult> DisplayAllFlightNumbersByType([FromQuery] string FlightType)
        {
            Console.WriteLine("hi");
            // Ensure FlightType is provided
            if (string.IsNullOrEmpty(FlightType))
            {
                return BadRequest("FlightType is required.");
            }

            List<string> flightNumbers = new List<string>();

            // Fetch flight numbers based on FlightType
            if (FlightType.Equals("International", StringComparison.OrdinalIgnoreCase))
            {
                var flightList = await _context.InternationalFlightDetails
                    .Select(f => f.FlightId) // Get only the FlightId (Flight Number)
                    .ToListAsync();

                flightNumbers.AddRange(flightList);
            }
            else if (FlightType.Equals("Domestic", StringComparison.OrdinalIgnoreCase))
            {
                var flightList = await _context.DomesticFlightDetails
                    .Select(f => f.FlightId) // Get only the FlightId (Flight Number)
                    .ToListAsync();

                flightNumbers.AddRange(flightList);
            }
            else
            {
                return BadRequest("Invalid FlightType. Please specify 'International' or 'Domestic'.");
            }

            if (flightNumbers.Any())
            {
                return Ok(flightNumbers);
            }
            else
            {
                return Ok(new { message = "No flights found for the specified Flight Type." });
            }
        }


    }
}
