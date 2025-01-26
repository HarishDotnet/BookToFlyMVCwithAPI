using Microsoft.AspNetCore.Mvc;
using FlightDetailApi.DTO;
using FlightDetailApi.Data;
using FlightDetailApi.Helpers;
using AutoMapper;
using FlightDetailApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
namespace FlightDetailsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly FlightHelper _flightHelper;

        public FlightController(ApplicationDbContext context, IMapper mapper)
        {
            _flightHelper = new FlightHelper(context, mapper);
        }

        [HttpPost("AddFlights")]
        public async Task<IActionResult> AddFlights([FromBody] List<FlightInputDTO> flightInputs)
        {
            if (flightInputs == null || !flightInputs.Any())
                return BadRequest("The flight list cannot be empty.");

            var invalidFlights = new List<string>();

            foreach (var flightInput in flightInputs)
            {
                if (!ModelState.IsValid || await _flightHelper.FlightExists(flightInput.FlightId))
                {
                    invalidFlights.Add(flightInput.FlightId ?? "Unknown FlightId");
                    continue;
                }

                // Map and add flights
                if (flightInput.FlightId.StartsWith("IF"))
                {
                    flightInput.FlightType = "International";
                    _flightHelper._context.InternationalFlightDetails.Add(_flightHelper._mapper.Map<InternationalFlightDetails>(flightInput));
                }
                else if (flightInput.FlightId.StartsWith("DF"))
                {
                    flightInput.FlightType = "Domestic";
                    _flightHelper._context.DomesticFlightDetails.Add(_flightHelper._mapper.Map<DomesticFlightDetails>(flightInput));
                }
                else
                {
                    invalidFlights.Add(flightInput.FlightId);
                }
            }

            await _flightHelper._context.SaveChangesAsync();

            if (invalidFlights.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "Some flights could not be added.",
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
            bool flightExists = await _flightHelper.FlightExists(flightInput.FlightId);
            if (flightExists)
            {
                return BadRequest($"Flight with FlightId {flightInput.FlightId} already exists.");
            }

            if (flightInput.FlightId.StartsWith("IF"))
            {
                flightInput.FlightType = "International";
                var flight = _flightHelper._mapper.Map<InternationalFlightDetails>(flightInput);
                _flightHelper._context.InternationalFlightDetails.Add(flight);
            }
            else if (flightInput.FlightId.StartsWith("DF"))
            {
                flightInput.FlightType = "Domestic";
                var flight = _flightHelper._mapper.Map<DomesticFlightDetails>(flightInput);
                _flightHelper._context.DomesticFlightDetails.Add(flight);
            }
            else
            {
                return BadRequest("Invalid flight type.");
            }

            try
            {
                await _flightHelper._context.SaveChangesAsync();
                return Ok("Flight added successfully.");
            }
            catch (DbUpdateException ex)
            {
                // Log the exception here
                return StatusCode(500, $"An error occurred while saving the flight: {ex.Message}");
            }
        }


        [HttpGet("DisplayAllFlightNumbersByType")]
        public async Task<IActionResult> DisplayAllFlightNumbersByType([FromQuery] string FlightType)
        {
            if (string.IsNullOrEmpty(FlightType))
                return BadRequest("FlightType is required.");

            var flightNumbers = await _flightHelper.GetFlightNumbersByType(FlightType);

            if (flightNumbers.Any())
                return Ok(flightNumbers);

            return Ok(new { message = "No flights found for the specified Flight Type." });
        }

        [HttpPost("DisplayFlightBySourceAndDestination")]
        public async Task<IActionResult> GetFlightsBySourceAndDestination(FlightSearchInput searchInput)
        {
            if (string.IsNullOrWhiteSpace(searchInput.Source) || string.IsNullOrWhiteSpace(searchInput.Destination) || string.IsNullOrWhiteSpace(searchInput.FlightType))
                return BadRequest("FlightType, Source, and Destination must be provided.");

            var flights = await _flightHelper.GetFlightsBySourceAndDestination(searchInput);

            if (!flights.Any())
                return NotFound("No flights found for the specified source, destination, and flight type.");

            return Ok(flights);
        }

        [HttpGet("DisplayFlightByType")]
        public async Task<IActionResult> DisplayFlightByType(string FlightType)
        {
            if (string.IsNullOrEmpty(FlightType))
                return BadRequest("FlightType is required.");

            var flights = await _flightHelper.GetFlightsByType(FlightType);

            if (flights == null || !flights.Any())
                return NotFound("No flights found for the specified Flight Type.");

            return Ok(flights);
        }
        [HttpGet("MatchFlightByNumberAndType")]
        public async Task<IActionResult> MatchFlightByNumberAndType([FromQuery] string FlightType, [FromQuery] string flightNumber)
        {


            // Ensure FlightType is provided
            if (string.IsNullOrEmpty(FlightType) && string.IsNullOrEmpty(FlightType))
            {
                return BadRequest("FlightType and FlightNumber both are required.");
            }
            // Declare the list for flight details
            List<FlightOutputDTO> flightLists = await _flightHelper.GetFlightsByType(FlightType);
            var flight = flightLists
                .FirstOrDefault(f => f.FlightId.Equals(flightNumber, StringComparison.OrdinalIgnoreCase));

            if (flight == null)
            {
                return NotFound("Flight not found with the provided Flight Number.");
            }

            // Map to DTO and return the flight details
            var flightOutput = _flightHelper._mapper.Map<FlightOutputDTO>(flight);
            return Ok(flightOutput);
        }
        [HttpPut("UpdateFlight/{flightId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFlight(string flightId, [FromBody] FlightInputDTO flightInput)
        {
            if (string.IsNullOrWhiteSpace(flightId))
                return BadRequest("FlightId is required.");

            if (flightInput == null)
                return BadRequest("FlightInput details are required.");

            Object flight = await _flightHelper._context.InternationalFlightDetails
                .FirstOrDefaultAsync(f => f.FlightId == flightId);

            if (flight == null)
            {
                flight = await _flightHelper._context.DomesticFlightDetails
                    .FirstOrDefaultAsync(f => f.FlightId == flightId);

                if (flight == null)
                    return NotFound("Flight not found.");
            }
            // Now we handle the case where we have the correct flight type
            if (flightInput.FlightType == "International")
                // Map the DTO to the correct international flight
                _flightHelper._mapper.Map<InternationalFlightDetails>(flightInput);
            else if (flightInput.FlightType == "Domestic")
                _flightHelper._mapper.Map<DomesticFlightDetails>(flightInput);
            else
                return BadRequest("Flight type mismatch.");

            await _flightHelper._context.SaveChangesAsync();
            return Ok("Flight updated successfully.");
        }

        [HttpDelete("DeleteFlight/{flightId}")]
        public async Task<IActionResult> DeleteFlight(string flightId)
        {
            if (string.IsNullOrEmpty(flightId))
                return BadRequest(new { success = false, message = "Flight ID cannot be null or empty." });
            try
            {

                // Determine flight type and remove
                if (flightId.StartsWith("IF"))
                {
                    // Ensure the type matches before mapping
                    var internationalFlight = await _flightHelper._context.InternationalFlightDetails.FirstOrDefaultAsync(f => f.FlightId == flightId);

                    if (internationalFlight == null)
                        return BadRequest(new { success = false, message = "Invalid data for InternationalFlightDetails." });

                    _flightHelper._context.InternationalFlightDetails.Remove(internationalFlight);
                }
                else if (flightId.StartsWith("DF"))
                {
                    var domesticFlight = await _flightHelper._context.DomesticFlightDetails.FirstOrDefaultAsync(f => f.FlightId == flightId);

                    if (domesticFlight == null)
                        return BadRequest(new { success = false, message = "Invalid data for DomesticFlightDetails." });

                    _flightHelper._context.DomesticFlightDetails.Remove(domesticFlight);
                }
                else
                {
                    return BadRequest(new { success = false, message = "Invalid flight number." });
                }

                await _flightHelper._context.SaveChangesAsync();
                return Ok(new { success = true, message = "Flight Deleted Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


    }
}
