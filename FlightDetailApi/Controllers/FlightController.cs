using AutoMapper;
using FlightDetailApi.Controllers.HelperMethods;
using FlightDetailApi.DTO;
using FlightDetailApi.Models;
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

                if (await _flightHelper.FlightExists(flightInput.FlightId))
                {
                    invalidFlights.Add(flightInput.FlightId);
                    continue;
                }

                try
                {
                    await _flightHelper.AddFlightAsync(flightInput);
                }
                catch (ArgumentException)
                {
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
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid flight type for FlightId: {FlightId}", flightInput.FlightId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding flight: {FlightId}", flightInput.FlightId);
                return StatusCode(500, $"An error occurred while saving the flight: {ex.Message}");
            }
        }

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

        [HttpGet("DisplayFlightByType")]
        public async Task<List<FlightOutputDTO>> GetFlightsByType(string flightType)
        {
            return await _flightHelper.GetFlightsByTypeAsync(flightType);
        }

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while matching the flight.");
                return StatusCode(500, "An internal error occurred.");
            }
        }

        [HttpPut("UpdateFlight/{flightId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFlight(string flightId, [FromBody] FlightInputDTO flightInput)
        {
            _logger.LogInformation("UpdateFlight request received for FlightId: {FlightId}", flightId);

            // Validate input
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
                // Delegate update logic to the helper
                await _flightHelper.UpdateFlightAsync(flightInput);
                return Ok("Flight updated successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating flight {FlightId}", flightId);
                return StatusCode(500, "An error occurred while updating the flight.");
            }
        }

        [HttpDelete("DeleteFlight/{flightId}")]
        public async Task<IActionResult> DeleteFlight(string flightId)
        {
            try
            {
                await _flightHelper.DeleteFlightAsync(flightId);
                return Ok(new { success = true, message = "Flight Deleted Successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting flight.");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}