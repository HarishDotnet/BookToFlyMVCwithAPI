using Newtonsoft.Json;      // For JsonConvert
using Newtonsoft.Json.Linq; // For JObject
using FlightDetailApi.DTO;
using FlightDetailApi.Models;
using FlightDetailApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FlightDetailApi.Repositories;
using FlightDetailApi.Repositories.IRepository;

namespace FlightDetailApi.Tests
{
    public class FlightControllerTests
    {
        private readonly Mock<IFlightRepository> _mockflightRepository;
        private readonly Mock<ILogger<FlightController>> _mockLogger;
        private readonly FlightController _controller;

        public FlightControllerTests()
        {
            _mockflightRepository = new Mock<IFlightRepository>();
            _mockLogger = new Mock<ILogger<FlightController>>();
            _controller = new FlightController(_mockflightRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AddFlight_ValidInput_ReturnsOk()
        {
            // Arrange
            var flightInput = new FlightInputDTO { FlightId = "IF123" };

            _mockflightRepository.Setup(x => x.FlightExists(flightInput.FlightId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.AddFlight(flightInput);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Flight added successfully.", okResult.Value);
            _mockflightRepository.Verify(x => x.AddFlightAsync(flightInput), Times.Once);
        }
        [Fact]
        public async Task AddFlight_InvalidModel_ReturnsBadRequestWithErrors()
        {
            // Arrange
            var invalidFlight = new FlightInputDTO(); // Invalid object
            _controller.ModelState.AddModelError("FlightId", "FlightId is required.");

            // Act
            var result = await _controller.AddFlight(invalidFlight);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result); 
            var json = JsonConvert.SerializeObject(badRequestResult.Value); // Fix variable name
            var response = JObject.Parse(json);

            // Check for ModelState error structure
            Assert.Equal("FlightId is required.", response["FlightId"][0].Value<string>());
        }
        [Fact]
        public async Task UpdateFlight_ValidInput_ReturnsOk()
        {
            // Arrange
            var flightId = "IF123";
            var flightInput = new FlightInputDTO { FlightId = flightId };

            _mockflightRepository.Setup(x => x.UpdateFlightAsync(flightInput))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateFlight(flightId, flightInput);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Flight updated successfully.", okResult.Value);
            _mockflightRepository.Verify(x => x.UpdateFlightAsync(flightInput), Times.Once);
        }
        [Fact]
        public async Task AddFlight_NullInput_ReturnsBadRequestWithMessage()
        {
            // Act
            var result = await _controller.AddFlight(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            // Serialize the result to check the "message" property
            var json = JsonConvert.SerializeObject(badRequestResult.Value);
            var response = JObject.Parse(json);

            Assert.Equal("Flight input is required.", response["message"].Value<string>());
        }

        [Fact]
        public async Task UpdateFlight_MismatchedIds_ReturnsBadRequest()
        {
            // Arrange
            var flightInput = new FlightInputDTO { FlightId = "IF123" };

            // Act
            var result = await _controller.UpdateFlight("DF456", flightInput);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Flight ID in route does not match Flight ID in body.", badRequestResult.Value);
        }


        [Fact]
        public async Task DeleteFlight_ValidInput_ReturnsOk()
        {
            // Arrange
            var flightId = "IF123";

            _mockflightRepository.Setup(x => x.DeleteFlightAsync(flightId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteFlight(flightId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Serialize the result to a JSON string and parse it as a JObject
            var json = JsonConvert.SerializeObject(okResult.Value);
            var response = JObject.Parse(json);

            Assert.True(response["success"].Value<bool>());
            Assert.Equal("Flight Deleted Successfully", response["message"].Value<string>());
            _mockflightRepository.Verify(x => x.DeleteFlightAsync(flightId), Times.Once);
        }
        [Fact]
        public async Task GetFlightsBySourceAndDestination_ValidInput_ReturnsFlights()
        {
            // Arrange
            var searchInput = new FlightSearchInput
            {
                Source = "NYC",
                Destination = "LON",
                FlightType = "International"
            };

            var expectedFlights = new List<FlightOutputDTO>
            {
                new FlightOutputDTO { FlightId = "IF123" }
            };

            _mockflightRepository.Setup(x => x.GetFlightsBySourceAndDestination(searchInput))
                .ReturnsAsync(expectedFlights);

            // Act
            var result = await _controller.GetFlightsBySourceAndDestination(searchInput);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedFlights, okResult.Value);
        }

        [Fact]
        public async Task MatchFlightByNumberAndType_FoundFlight_ReturnsFlight()
        {
            // Arrange
            var flightType = "International";
            var flightNumber = "IF123";
            var expectedFlight = new FlightOutputDTO { FlightId = flightNumber };

            _mockflightRepository.Setup(x => x.GetFlightsByTypeAsync(flightType))
                .ReturnsAsync(new List<FlightOutputDTO> { expectedFlight });

            // Act
            var result = await _controller.MatchFlightByNumberAndType(flightType, flightNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedFlight, okResult.Value);
        }

        [Fact]
        public async Task MatchFlightByNumberAndType_NotFound_ReturnsNotFound()
        {
            // Arrange
            var flightType = "International";
            var flightNumber = "IF123";

            _mockflightRepository.Setup(x => x.GetFlightsByTypeAsync(flightType))
                .ReturnsAsync(new List<FlightOutputDTO>());

            // Act
            var result = await _controller.MatchFlightByNumberAndType(flightType, flightNumber);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result); // Check for NotFoundObjectResult
            Assert.Equal("Flight not found with the provided Flight Number.", notFoundResult.Value); // Verify message
        }
        
    }
}