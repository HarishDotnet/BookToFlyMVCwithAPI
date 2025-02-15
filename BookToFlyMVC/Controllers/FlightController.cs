using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using BookToFlyMVC.DTO;
using System.Text.Json;
using FlightDetailApi.Models;
using System.Text;
using System.Net.Http.Headers;
using BookToFlyMVC.Exceptions;
namespace BookToFlyMVC.Controllers
{
    public class FlightController : Controller
    {
        private readonly HttpClient _client;
        private readonly IMapper _mapper;
        // Constructor: Initialize HttpClient and AutoMapper
        public FlightController(IMapper mapper, IHttpClientFactory clientFactory)
        {
            _mapper = mapper;
            _client = clientFactory.CreateClient("FlightClient");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        // Handles the flight creation process
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] FlightDetailsDTO flightDetails)
        {
            // Modify Flight ID based on flight type (International/ Domestic)
            flightDetails.FlightId = flightDetails.FlightType.Equals("International")
                ? "IF" + flightDetails.FlightId : "DF" + flightDetails.FlightId;

            // Validate the model before sending it to the API
            if (!ModelState.IsValid)
            {
                return View(flightDetails);
            }

            // Convert flightDetails object to JSON for API request
            var jsonContent = JsonSerializer.Serialize(flightDetails);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                // Retrieve JWT token from session and attach it for authorization
                var token = HttpContext.Session.GetString("JWT_TOKEN");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Send the POST request to add the flight to the API
                var response = await _client.PostAsync(_client.BaseAddress + "Flight/AddFlight", httpContent);

                // Check if the request was successful, otherwise throw an exception
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["CreateFailedMessage"] = "Failed to add flight! Flight number already exist";
                    return View(flightDetails);
                    throw new FlightCreationException($"Failed to add flight: {errorMessage}");
                }

                // Display success message and redirect
                TempData["CreateSuccessMessage"] = "Flight added successfully!";
                return RedirectToAction("ShowflightCard", new { flightNumber = flightDetails.FlightId });
            }
            catch (HttpRequestException ex)
            {
                // Log and throw error if the HTTP request fails
                throw new FlightCreationException($"HTTP Request Error: {ex.Message}");
            }
        }

        public IActionResult SearchFlights()
        {
            return View("SearchFlights");
        }

        // Handles flight search based on search criteria
        [HttpPost("Flight/SearchFlights")]
        public async Task<IActionResult> SearchFlights(FlightSearchInput searchInput)
        {
            List<FlightDetailsDTO> flightList = new List<FlightDetailsDTO>();
            try
            {
                // Validate input data
                if (string.IsNullOrEmpty(searchInput.FlightType) ||
                    string.IsNullOrEmpty(searchInput.Source) || string.IsNullOrEmpty(searchInput.Destination))
                {
                    throw new FlightSearchException("Invalid search parameters.");
                }

                // Map search input to a DTO and send to API
                var flightSearchData = _mapper.Map<FlightSearchDTO>(searchInput);
                var jsonContent = JsonSerializer.Serialize(flightSearchData);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Call the API to search for flights
                var response = await _client.PostAsync("Flight/DisplayFlightBySourceAndDestination", httpContent);

                // Handle failure to fetch flight data
                if (!response.IsSuccessStatusCode)
                {
                    throw new FlightSearchException("Failed to fetch flight data.");
                }

                string flightData = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                flightList = JsonSerializer.Deserialize<List<FlightDetailsDTO>>(flightData, options);
                // If no flights are found, throw an exception
                if (flightList == null || flightList.Count == 0)
                {
                    throw new FlightSearchException("No flights found for the provided search criteria.");
                }

                // Return the flight list to the view
                return View("ShowFlights", flightList);
            }
            catch (Exception ex)
            {
                // If any exception occurs, throw a search exception
                throw new FlightSearchException($"Exception: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ShowflightCard(string flightNumber)
        {
            var flightdetails = await GetFlightDetailsFromApi(flightNumber);

            // Assign flight type based on the flight ID
            flightdetails.FlightType = flightdetails.FlightId.StartsWith("IF") ? "International" : "Domestic";

            // Check if flight details were found and display accordingly
            if (flightdetails != null)
            {
                TempData["UpdateSuccessMessage"] = "Flight updated successfully!";
                return View(flightdetails);
            }
            else
            {
                // Handle flight not found scenario
                ModelState.AddModelError(string.Empty, "Flight details not found.");
                return View("Edit");
            }
        }

        public IActionResult Update()
        {
            return View();
        }

        // Fetch flight details for editing
        [HttpGet("Flight/EditGetDetail/{flightId}")]
        public async Task<IActionResult> EditGetDetail(string flightId)
        {
            if (flightId != null)
            {
                var flight = await GetFlightDetailsFromApi(flightId);
                TempData["FlightDetails"] = JsonSerializer.Serialize(flight);
                return View("EditPage", flight);
            }
            else
            {
                return View("Update");
            }
        }

        [HttpGet]
        public IActionResult EditPage()
        {
            return View();
        }

        // Handles the update flight functionality
        [HttpPost]
        public async Task<IActionResult> EditPage(FlightDetailsDTO flightDetails)
        {
            // Validate flight details before sending update request
            if (!ModelState.IsValid)
            {
                return View("EditPage", flightDetails);
            }

            var jsonContent = JsonSerializer.Serialize(flightDetails);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                // Attach authorization token
                var token = HttpContext.Session.GetString("JWT_TOKEN");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Send update request to the API
                var response = await _client.PutAsync(_client.BaseAddress + $"Flight/UpdateFlight/{flightDetails.FlightId}", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["FlightDetails"] = JsonSerializer.Serialize(flightDetails);
                    TempData["UpdateSuccessMessage"] = "Flight updated successfully!";
                }
                else
                {
                    // Handle error if unable to update
                    TempData["UpdateErrorMessage"] = "You Can't Add Flight Sorry, because You are not Authorized!";
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error: {errorMessage}");
                }
            }
            catch (HttpRequestException ex)
            {
                // Log error if request fails
                ModelState.AddModelError("", $"HTTP Request Error: {ex.Message}");
            }

            // Redirect to show the updated flight details
            return RedirectToAction("ShowflightCard", new { flightNumber = flightDetails.FlightId });
        }

        public IActionResult Delete()
        {
            return View();
        }

        // Confirms the deletion of the flight
        public async Task<IActionResult> ConfirmDelete(string flightId)
        {
            var token = HttpContext.Session.GetString("JWT_TOKEN");
            if (string.IsNullOrEmpty(token))
            {
                throw new FlightDeletionException("Token is missing or expired.");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (string.IsNullOrEmpty(flightId))
            {
                throw new FlightDeletionException("Invalid flight ID.");
            }

            try
            {
                var response = await _client.DeleteAsync($"Flight/DeleteFlight/{flightId}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new FlightDeletionException("Failed to delete the flight.");
                }

                TempData["DeleteSuccessMessage"] = "Flight deleted successfully.";
            }
            catch (Exception ex)
            {
                // Log error if deletion fails
                throw new FlightDeletionException($"Error deleting flight: {ex.Message}");
            }
            return RedirectToAction("Delete");
        }

        public async Task<FlightDetailsDTO> GetFlightDetailsFromApi(string flightNumber)
        {
            string flightType = flightNumber.StartsWith("IF") ? "International" :
                                flightNumber.StartsWith("DF") ? "Domestic" : null;

            try
            {
                // Fetch flight details based on flight number and type
                var response = await _client.GetAsync($"{_client.BaseAddress}Flight/MatchFlightByNumberAndType?FlightType={flightType}&flightNumber={flightNumber}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new FlightRetrievalException("Failed to retrieve flight details.");
                }

                return await response.Content.ReadFromJsonAsync<FlightDetailsDTO>();
            }
            catch (Exception ex)
            {
                // Handle error while fetching flight details
                throw new FlightRetrievalException($"Error retrieving flight details: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> ManageFlights(string FlightType, int page = 1, int pageSize = 10)
        {
            HttpResponseMessage response = await _client.GetAsync($"Flight/DisplayFlightByType?FlightType={FlightType}");
            List<FlightDetailsDTO> flightList = new List<FlightDetailsDTO>();

            try
            {
                if (response.IsSuccessStatusCode)
                {
                    string flightData = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    flightList = JsonSerializer.Deserialize<List<FlightDetailsDTO>>(flightData, options);
                }
            }
            catch (Exception ex)
            {
                return View("Error" + ex.Message);
            }

            // Paginate the flight list
            var paginatedFlights = flightList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Pass the paginated list and pagination info to the view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalFlights = flightList.Count;
            ViewBag.TotalPages = (int)Math.Ceiling((double)flightList.Count / pageSize);
            ViewBag.FlightType = FlightType; // Pass FlightType to the view

            return View(paginatedFlights);
        }
    }
}
