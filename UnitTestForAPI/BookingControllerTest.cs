using FlightDetailApi;
using FlightDetailApi.Controllers;
using FlightDetailApi.Data;
using FlightDetailApi.Models;
using FlightDetailApi.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;

namespace UnitTestForAPI
{
    public class BookingControllerTest
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IFlightRepository> _mockFlightRepository;
        private readonly BookingController _mockBookingController;

        public BookingControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _mockFlightRepository = new Mock<IFlightRepository>();
            _mockBookingController = new BookingController(_context, _mockFlightRepository.Object);
        }

        [Fact]
        public async Task GetBooking_ValidInput_ReturnsOk()
        {
            //Arrange
            var BookingId = "76544";
            var booking = new BookingDetails
            {
                BookingId = "76544",
                FlightId = "DF909",
                PassengerName = "Malliga",
                Email = "Mallliga@gmail.com",
                PhoneNumber = "8383544311",
                BookingDate = DateTime.Parse("2025-02-02T05:24:40.617"),
                DateOfTravel = DateTime.Parse("2025-02-21T00:00:00"),
                IsCanceled = true
            };

            var flight = new DomesticFlightDetails
            {
                FlightId = "DF909",
                AirlineName = "Sky Connect",
                Source = "Lucknow",
                Destination = "Varanasi",
                Duration = 0.75,
                AvailableSeats = 40,
                TicketPrice = 1000,
                AvailableDays = new List<string> { "Monday", "Thursday", "Saturday" }
            };

            _context.Booking.Add(booking);
            await _context.SaveChangesAsync();

            // _mockFlightRepository.Setup(x => x.GetFlightByIdAsync("IF100")).ReturnsAsync((DomesticFlightDetails)null);

            //Act
            var result = await _mockBookingController.GetBooking(BookingId);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);


        }
        [Fact]
        public async Task GetBooking_ValidInput_CheckFlightNumer()
        {
            //Arrange
            var BookingId = "76544";
            var booking = new BookingDetails
            {
                BookingId = "76544",
                FlightId = "DF909",
                PassengerName = "Malliga",
                Email = "Mallliga@gmail.com",
                PhoneNumber = "8383544311",
                BookingDate = DateTime.Parse("2025-02-02T05:24:40.617"),
                DateOfTravel = DateTime.Parse("2025-02-21T00:00:00"),
                IsCanceled = true
            };

            var flight = new DomesticFlightDetails
            {
                FlightId = "DF909",
                AirlineName = "Sky Connect",
                Source = "Lucknow",
                Destination = "Varanasi",
                Duration = 0.75,
                AvailableSeats = 40,
                TicketPrice = 1000,
                AvailableDays = new List<string> { "Monday", "Thursday", "Saturday" }
            };

            // _mockFlightRepository.Setup(x => x.GetFlightByIdAsync(booking.FlightId)).ReturnsAsync((DomesticFlightDetails)null);
            _mockFlightRepository.Setup(x => x.GetFlightByIdAsync(booking.FlightId)).ReturnsAsync(flight);

            //Act
            var result = await _mockBookingController.GetBooking(BookingId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var  responseJson= JsonConvert.SerializeObject(okResult.Value);
            var response=JsonConvert.DeserializeObject<Dictionary<string,object>>(responseJson);
            var  flightDetailsJson= JsonConvert.SerializeObject(response["flight"]);
            var flightDetails = JsonConvert.DeserializeObject<DomesticFlightDetails>(flightDetailsJson);

            // Console.Write("i am " + flightDetails.FlightId);
            // JsonConvert.SerializeObject(response["flight"])


            //Assert
            Assert.Equal("DF909", flightDetails.FlightId);


        }
        [Fact]
        public async Task GetBooking_ValidInput_ReturnsBadRequest()
        {
            //Arrange           
            //Act
            var result = await _mockBookingController.GetBooking(null);
            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public async Task PutBooking_ValidInput_ReturnOkResult()
        {
            // Arrange
            var booking = new BookingDetails
            {
                BookingId = "12421",
                FlightId = "DF909",
                PassengerName = "Harish",
                Email = "Harish@example.com",
                PhoneNumber = "9876543210",
                BookingDate = DateTime.Parse("2025-02-10T14:30:00"),
                DateOfTravel = DateTime.Parse("2025-03-15T00:00:00"),
                IsCanceled = false
            };

            _context.Booking.Add(booking);
            await _context.SaveChangesAsync();

            // Act 
            var result = await _mockBookingController.PutBooking(booking.BookingId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            // Deserialize to a known type (Dictionary)
            var messageObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(okResult.Value));

            // Assert.NotNull(messageObject);
            // Assert.True(messageObject.ContainsKey("message"));
            Assert.Equal("Ticket Canceled Successfully", messageObject["message"]);
        }




    }
}