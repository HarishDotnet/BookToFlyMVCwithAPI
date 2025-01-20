using System.Text;
using System.Text.Json;
using AutoMapper;
using BookToFlyMVC.Data;
using BookToFlyMVC.DTO;
using BookToFlyMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookToFlyMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _client;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public AdminController(ApplicationDbContext dbContext, IMapper mapper, IHttpClientFactory clientFactory)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _client = clientFactory.CreateClient("FlightClient");
        }

        // GET: Add Flight Page
        public IActionResult AddFlight()
        {
            return View(); // Returns the AddFlight view
        }

        // Default Show Flights
        [HttpGet]
        public IActionResult ShowFlights()
        {
            return View("ShowFlights");
        }

        // Show Flights with FlightType Parameter
        [HttpGet("Admin/ShowFlights/{FlightType}")]
        public async Task<IActionResult> ShowFlights(string FlightType)
        {
            Console.WriteLine("Hi iam here");
            if (!FlightType.Equals("International", StringComparison.OrdinalIgnoreCase) &&
                !FlightType.Equals("Domestic", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "Invalid Flight Type");
                return View("Error");
            }

            object flights = FlightType.Equals("International", StringComparison.OrdinalIgnoreCase)
                ? new List<InternationalFlightModel>()
                : new List<DomesticFlightModel>();

            try
            {
                HttpResponseMessage response = await _client.GetAsync($"Flight/DisplayFlightByType/{FlightType}");
                if (response.IsSuccessStatusCode)
                {
                    string flightData = await response.Content.ReadAsStringAsync();

                    // Create JsonSerializerOptions to allow case-insensitive matching of property names.
                    // For example, if the JSON has a property "flightId" != model file:"FlightId" will give u null value
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    flights = FlightType.Equals("International", StringComparison.OrdinalIgnoreCase)
                        ? JsonSerializer.Deserialize<List<InternationalFlightModel>>(flightData, options)
                        : JsonSerializer.Deserialize<List<DomesticFlightModel>>(flightData, options);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to fetch flight data.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Exception: {ex.Message}");
            }

            var flightDto = _mapper.Map<List<FlightDetailsDTO>>(flights);
            return View("ShowFlights", flightDto); // Ensure the view matches the DTO
        }
        [HttpPost("Admin/SearchFlights")]
        public async Task<IActionResult> SearchFlights(FlightSearchModel searchInput)
        {
            Console.WriteLine($"Fetching flight details with Flight Type: {searchInput.FlightType}, Source: {searchInput.Source}, Destination: {searchInput.Destination}, Flight Number: {searchInput.FlightNumber}");

            // List to hold flight data
            List<FlightDetailsDTO> flightList = new List<FlightDetailsDTO>();
            var flightSearchData = new FlightSearchDTO();
            try
            {
                // Determine which API endpoint to call based on user input
                HttpResponseMessage response = null;

                if (!string.IsNullOrEmpty(searchInput.FlightType) && string.IsNullOrEmpty(searchInput.FlightNumber) && !string.IsNullOrEmpty(searchInput.Source) && !string.IsNullOrEmpty(searchInput.Destination))
                {
                    // Call API to get flights by Source and Destination
                    flightSearchData = _mapper.Map<FlightSearchDTO>(searchInput);
                    var jsonContent = JsonSerializer.Serialize(flightSearchData);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    response = await _client.PostAsync("Flight/DisplayFlightBySourceAndDestination", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        string flightData = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON into a list of FlightOutputDTO
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        flightList = JsonSerializer.Deserialize<List<FlightDetailsDTO>>(flightData, options);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Failed to fetch flight data.");
                    }

                }
                else if (!string.IsNullOrEmpty(searchInput.FlightType) && !string.IsNullOrEmpty(searchInput.FlightNumber))
                {
                    // Call API to get flights by Flight Type (and Flight Number)
                    response = await _client.GetAsync($"Flight/DisplayFlightByType?FlightType={searchInput.FlightType}&flightNumber={searchInput.FlightNumber}");
                    if (response.IsSuccessStatusCode)
                    {
                        string flightData = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON into a list of FlightOutputDTO
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        FlightDetailsDTO flight = JsonSerializer.Deserialize<FlightDetailsDTO>(flightData, options);
                        flightList.Add(flight);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Failed to fetch flight data.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid search parameters.");
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Exception: {ex.Message}");
            }

            // Check if flight data is available
            if (flightList == null)
            {
                ModelState.AddModelError(string.Empty, "No flights found for the provided search criteria.");
                return View("Error");
            }

            // Return the view with the list of flights
            return View("ShowFlights", flightList); // Ensure a view exists to display the list of flights
        }



        [HttpPost]
        public async Task<IActionResult> AddFlight(InternationalFlightModel flightModel)
        {
            if (ModelState.IsValid)
            {
                _dbContext.InternationalFlights.Add(flightModel);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("ManageFlights");
            }
            return View(flightModel);
        }

        public IActionResult Dashboard()
        {
            return View("Dashboard");
        }

        public IActionResult ManageFlights()
        {
            var flights = _dbContext.InternationalFlights.ToList();
            return View(flights);
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Login", "User");
        }
    }
}
