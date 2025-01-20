using Microsoft.AspNetCore.Mvc;
using FlightDetailsApi.DTO;
using FlightDetailsApi.Models;
using FlightDetailsApi.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost("AddFlight")]
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

        // Delete a flight by flight number
        [HttpDelete("DeleteFlight/{flightId}")]
        public async Task<IActionResult> DeleteFlight(string flightId)
        {
            var Iflight = await _context.InternationalFlightDetails
                .FirstOrDefaultAsync(f => f.FlightId == flightId);

            if (Iflight != null)
            {
                _context.InternationalFlightDetails.Remove(Iflight);
                await _context.SaveChangesAsync();
                return Ok("International flight deleted successfully.");
            }

            var Dflight = await _context.DomesticFlightDetails
                .FirstOrDefaultAsync(f => f.FlightId == flightId);

            if (Dflight != null)
            {
                _context.DomesticFlightDetails.Remove(Dflight);
                await _context.SaveChangesAsync();
                return Ok("Domestic flight deleted successfully.");
            }

            return NotFound("Flight not found.");
        }

        [HttpPut("UpdateFlight/{flightId}")]
        public async Task<IActionResult> UpdateFlight(string flightId, [FromBody] FlightInputDTO flightInput)
        {
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
    Console.WriteLine("Hi I am FlightType and/or FlightNumber");

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
        flightLists=_mapper.Map<List<FlightOutputDTO>>(flightList);
    }
    else if (FlightType.Equals("Domestic", StringComparison.OrdinalIgnoreCase))
    {
        var flightList = await _context.DomesticFlightDetails.ToListAsync(); // Fetch all Domestic flights
        flightLists=_mapper.Map<List<FlightOutputDTO>>(flightList);
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


    }
}
