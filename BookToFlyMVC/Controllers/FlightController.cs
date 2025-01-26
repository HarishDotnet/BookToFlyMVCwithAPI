using Microsoft.AspNetCore.Mvc;
using BookToFlyMVC.Data;
using AutoMapper;
using BookToFlyMVC.DTO;
using System.Text.Json;
using FlightDetailApi.Models;
using System.Text;
using System.Net.Http.Headers;

namespace BookToFlyAPI.Controllers
{
    public class FlightController : Controller
    {
        private readonly HttpClient _client;
        private readonly IMapper _mapper;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] FlightDetailsDTO flightDetails)
        {
            if (!ModelState.IsValid)
            {
                return View(flightDetails);
            }

            var jsonContent = JsonSerializer.Serialize(flightDetails);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var token = HttpContext.Session.GetString("JWT_TOKEN");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _client.PostAsync(_client.BaseAddress + "Flight/AddFlight", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["CreateSuccessMessage"] = "Flight added successfully!";
                    return RedirectToAction("Create");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["CreateErrorMessage"] = $"Error: {errorMessage}"; // Save the error message
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["CreateErrorMessage"] = $"HTTP Request Error: {ex.Message}"; // Save error message
            }

            return View(flightDetails);
        }


        public IActionResult SearchFlights()
        {
            return View("SearchFlights");
        }

        [HttpPost("Flight/SearchFlights")]
        public async Task<IActionResult> SearchFlights(FlightSearchInput searchInput)
        {
            List<FlightDetailsDTO> flightList = new List<FlightDetailsDTO>();
            var flightSearchData = new FlightSearchDTO();

            try
            {
                HttpResponseMessage response = null;

                if (!string.IsNullOrEmpty(searchInput.FlightType) && string.IsNullOrEmpty(searchInput.FlightNumber) &&
                    !string.IsNullOrEmpty(searchInput.Source) && !string.IsNullOrEmpty(searchInput.Destination))
                {
                    flightSearchData = _mapper.Map<FlightSearchDTO>(searchInput);
                    var jsonContent = JsonSerializer.Serialize(flightSearchData);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    response = await _client.PostAsync("Flight/DisplayFlightBySourceAndDestination", httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string flightData = await response.Content.ReadAsStringAsync();
                        //properties without considering case between api and mvc models
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
                    response = await _client.GetAsync($"Flight/DisplayFlightByType?FlightType={searchInput.FlightType}&flightNumber={searchInput.FlightNumber}");

                    if (response.IsSuccessStatusCode)
                    {
                        string flightData = await response.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var flight = JsonSerializer.Deserialize<FlightDetailsDTO>(flightData, options);
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

            if (flightList == null || flightList.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "No flights found for the provided search criteria.");
                return View("Error");
            }

            return View("ShowFlights", flightList);
        }

        [HttpGet]
        public async Task<IActionResult> ShowflightCard(string flightNumber)
        {
            var flightdetails = await GetFlightDetailsFromApi(flightNumber); // Use await here
            if (flightdetails != null)
            {
                return View(flightdetails);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Flight details not found.");
                return View();
            }
        }
        public IActionResult Update()
        {
            // This action renders the view to delete the flight
            return View();
        }

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
                return View("Update"); // return a view or redirect as per your logic
            }
        }

        [HttpGet]
        public IActionResult EditPage()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditPage(FlightDetailsDTO flightDetails)
        {
            if (!ModelState.IsValid)
            {
                return View("EditPage", flightDetails);
            }

            var jsonContent = JsonSerializer.Serialize(flightDetails);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var token = HttpContext.Session.GetString("JWT_TOKEN");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _client.PutAsync(_client.BaseAddress + $"Flight/UpdateFlight/{flightDetails.FlightId}", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["FlightDetails"] = JsonSerializer.Serialize(flightDetails);
                    TempData["SuccessMessage"] = "Flight updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "You Can't Add Flight Sorry, because You are not Authorized!";
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error: {errorMessage}");
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"HTTP Request Error: {ex.Message}");
            }

            return RedirectToAction("ShowflightCard", new { flightNumber = flightDetails.FlightId });

        }
        public IActionResult Delete()
        {
            return View();
        }

        public async Task<IActionResult> ConfirmDelete(string flightId)
        {
            var token = HttpContext.Session.GetString("JWT_TOKEN");
            if (string.IsNullOrEmpty(token))
            {
                TempData["DeleteErrorMessage"] = "Token is missing or expired.";
                return RedirectToAction("Delete");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (string.IsNullOrEmpty(flightId))
            {
                TempData["DeleteErrorMessage"] = "Invalid flight ID.";
                return RedirectToAction("Delete");
            }

            try
            {
                // Use the injected _client instead of creating a new HttpClient
                var response = await _client.DeleteAsync($"Flight/DeleteFlight/{flightId}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["DeleteSuccessMessage"] = "Flight deleted successfully.";
                }
                else
                {
                    // Optionally log the response status code or message for debugging
                    TempData["DeleteErrorMessage"] = "Failed to delete the flight. Status code: " + response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                // Log exception using a proper logging framework
                // _logger.LogError(ex, "An error occurred while deleting the flight.");
                Console.WriteLine(ex);
                TempData["DeleteErrorMessage"] = "An error occurred while deleting the flight.";
            }

            return RedirectToAction("Delete"); // Redirect to the same page to show success or error message
        }
        private async Task<FlightDetailsDTO> GetFlightDetailsFromApi(string flightNumber)
        {
            string flightType;
            if(flightNumber.StartsWith("IF"))  flightType="International";
            else flightType=flightNumber.StartsWith("DF")?"Domestic":null;
            // Logic to call an API to fetch flight details
                var response = await _client.GetAsync($"{_client.BaseAddress}Flight/MatchFlightByNumberAndType?FlightType={flightType}&flightNumber={flightNumber}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<FlightDetailsDTO>();
                }
            return null;
        }

        [HttpGet]
        public IActionResult ManageFlights()
        {
            return View();
        }
        //Used to display the flight details with Actions for Admin
        [HttpPost]
        public async Task<IActionResult> ManageFlights([FromForm] string FlightType)
        {
            HttpResponseMessage respose = await _client.GetAsync($"Flight/DisplayFlightByType?FlightType={FlightType}");
            List<FlightDetailsDTO> flightList = new List<FlightDetailsDTO>();
            try
            {
                if (respose.IsSuccessStatusCode)
                {
                    string flightData = await respose.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    flightList = JsonSerializer.Deserialize<List<FlightDetailsDTO>>(flightData, options);
                }
                else
                {
                    return View("Error to get the data from the api");
                }
            }
            catch (Exception ex)
            {
                return View("Error" + ex.Message);
            }

            return View("ManageFlights", flightList);
        }

    }
}
