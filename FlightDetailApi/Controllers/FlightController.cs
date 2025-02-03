using AutoMapper;
using BookToFlyMVC.Validations;
using FlightDetailApi.DTO;
using FlightDetailApi.Models;
using FlightDetailApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightDetailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IFlightHelper _flightHelper;
        private readonly ILogger<FlightController> _logger;

        public FlightController(
            IFlightHelper flightHelper,
            ILogger<FlightController> logger
        )
        {
            _flightHelper = flightHelper;
            _logger = logger;
        }

        // POST: api/Flight/AddFlights
        // Adds multiple flights at once.
        [HttpPost("AddFlights")]
        public async Task<IActionResult> AddFlights([FromBody] List<FlightInputDTO> flightInputs)
        {
            if (flightInputs == null || !ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid input" });
            }

            var invalidFlights = new List<string>();

            foreach (var flightInput in flightInputs)
            {
                if (flightInput.FlightId == null)
                {
                    invalidFlights.Add("Unknown FlightId");
                    continue;
                }

                // Check if the flight already exists
                if (await _flightHelper.FlightExists(flightInput.FlightId))
                {
                    invalidFlights.Add(flightInput.FlightId);
                    continue;
                }

                try
                {
                    await _flightHelper.AddFlightAsync(flightInput);
                }
                catch (FlightApiException flightApiException) // Custom exception handling
                {
                    _logger.LogError($"Error adding flight {flightInput.FlightId} {flightApiException.Message}");
                    invalidFlights.Add(flightInput.FlightId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding flight {FlightId}", flightInput.FlightId);
                }
            }

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

        // POST: api/Flight/AddFlight
        // Adds a single flight (Admin-only access)
        [HttpPost("AddFlight")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddFlight([FromBody] FlightInputDTO flightInput)
        {
            _logger.LogInformation("AddFlight request received for FlightId: {FlightId}", flightInput?.FlightId);

            if (flightInput == null)
            {
                _logger.LogWarning("Flight input is null.");
                return BadRequest(new { message = "Flight input is required." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid flight input model for FlightId: {FlightId}", flightInput.FlightId);
                return BadRequest(ModelState);
            }

            // Check if flight already exists
            if (await _flightHelper.FlightExists(flightInput.FlightId))
            {
                _logger.LogWarning("Duplicate FlightId detected: {FlightId}", flightInput.FlightId);
                return BadRequest($"Flight with FlightId {flightInput.FlightId} already exists.");
            }

            try
            {
                await _flightHelper.AddFlightAsync(flightInput);
                return Ok("Flight added successfully.");
            }
            catch (FlightApiException flightApiException)
            {
                _logger.LogWarning("Invalid flight type for FlightId: {FlightId} {flightApiException}", flightInput.FlightId, flightApiException.Message);
                return BadRequest(flightApiException.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding flight: {FlightId}", flightInput.FlightId);
                return StatusCode(500, $"An error occurred while saving the flight: {ex.Message}");
            }
        }

        // GET: api/Flight/DisplayAllFlightNumbersByType?FlightType=Domestic
        // Retrieves flight numbers based on flight type.
        [HttpGet("DisplayAllFlightNumbersByType")]
        public async Task<IActionResult> DisplayAllFlightNumbersByType([FromQuery] string FlightType)
        {
            _logger.LogInformation("DisplayAllFlightNumbersByType request received for FlightType: {FlightType}", FlightType);

            if (string.IsNullOrEmpty(FlightType))
            {
                return BadRequest("FlightType is required.");
            }

            var flightNumbers = await _flightHelper.GetFlightNumbersByType(FlightType);

            if (!flightNumbers.Any())
            {
                return Ok(new { message = "No flights found for the specified Flight Type." });
            }

            return Ok(flightNumbers);
        }

        // POST: api/Flight/DisplayFlightBySourceAndDestination
        // Fetches flights based on source and destination.
        [HttpPost("DisplayFlightBySourceAndDestination")]
        public async Task<IActionResult> GetFlightsBySourceAndDestination(FlightSearchInput searchInput)
        {
            _logger.LogInformation("GetFlightsBySourceAndDestination request received.");

            if (string.IsNullOrWhiteSpace(searchInput.Source) || string.IsNullOrWhiteSpace(searchInput.Destination) || string.IsNullOrWhiteSpace(searchInput.FlightType))
            {
                return BadRequest("FlightType, Source, and Destination must be provided.");
            }

            var flights = await _flightHelper.GetFlightsBySourceAndDestination(searchInput);

            if (!flights.Any())
            {
                return NotFound("No flights found for the specified source, destination, and flight type.");
            }

            return Ok(flights);
        }

        // GET: api/Flight/DisplayFlightByType
        // Fetches flights based on flight type.
        [HttpGet("DisplayFlightByType")]
        public async Task<List<FlightOutputDTO>> GetFlightsByType(string flightType)
        {
            return await _flightHelper.GetFlightsByTypeAsync(flightType);
        }

        // GET: api/Flight/MatchFlightByNumberAndType
        // Matches a flight based on its type and number.
        [HttpGet("MatchFlightByNumberAndType")]
        public async Task<IActionResult> MatchFlightByNumberAndType([FromQuery] string FlightType, [FromQuery] string flightNumber)
        {
            _logger.LogInformation("MatchFlightByNumberAndType invoked with FlightType: {FlightType}, FlightNumber: {FlightNumber}", FlightType, flightNumber);

            if (string.IsNullOrEmpty(FlightType) || string.IsNullOrEmpty(flightNumber))
            {
                return BadRequest("FlightType and FlightNumber both are required.");
            }

            try
            {
                var flightLists = await _flightHelper.GetFlightsByTypeAsync(FlightType);
                var flight = flightLists.FirstOrDefault(f => f.FlightId.Equals(flightNumber, StringComparison.OrdinalIgnoreCase));

                if (flight == null)
                {
                    return NotFound("Flight not found with the provided Flight Number.");
                }

                return Ok(flight);
            }
            catch (FlightNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (FlightApiException ex)
            {
                _logger.LogError(ex, "An error occurred while matching the flight.");
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/Flight/UpdateFlight/{flightId}
        // Updates flight details (Admin-only access).
        [HttpPut("UpdateFlight/{flightId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFlight(string flightId, [FromBody] FlightInputDTO flightInput)
        {
            _logger.LogInformation("UpdateFlight request received for FlightId: {FlightId}", flightId);

            if (flightInput == null)
            {
                return BadRequest("Flight input is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (flightId != flightInput.FlightId)
            {
                return BadRequest("Flight ID in route does not match Flight ID in body.");
            }

            try
            {
                await _flightHelper.UpdateFlightAsync(flightInput);
                return Ok("Flight updated successfully.");
            }
            catch (FlightNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (UnauthorizedFlightAccessException ex)
            {
                _logger.LogWarning(ex.Message);
                return Forbid();
            }
            catch (FlightApiException ex)
            {
                _logger.LogError(ex, "Error updating flight {FlightId}", flightId);
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/Flight/DeleteFlight/{flightId}
        // Deletes a flight.
        [HttpDelete("DeleteFlight/{flightId}")]
        public async Task<IActionResult> DeleteFlight(string flightId)
        {
            try
            {
                await _flightHelper.DeleteFlightAsync(flightId);
                return Ok(new { success = true, message = "Flight Deleted Successfully" });
            }
            catch (FlightNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
        }
    }
}
