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
            flightDetails.FlightId = flightDetails.FlightType.Equals("International")
                ? "IF" + flightDetails.FlightId : "DF" + flightDetails.FlightId;
            
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

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new FlightCreationException($"Failed to add flight: {errorMessage}");
                }

                TempData["CreateSuccessMessage"] = "Flight added successfully!";
                return RedirectToAction("Create");
            }
            catch (HttpRequestException ex)
            {
                throw new FlightCreationException($"HTTP Request Error: {ex.Message}");
            }
        }


        public IActionResult SearchFlights()
        {
            return View("SearchFlights");
        }

       [HttpPost("Flight/SearchFlights")]
        public async Task<IActionResult> SearchFlights(FlightSearchInput searchInput)
        {
            List<FlightDetailsDTO> flightList = new List<FlightDetailsDTO>();

            try
            {
                if (string.IsNullOrEmpty(searchInput.FlightType) ||
                    string.IsNullOrEmpty(searchInput.Source) || string.IsNullOrEmpty(searchInput.Destination))
                {
                    throw new FlightSearchException("Invalid search parameters.");
                }

                var flightSearchData = _mapper.Map<FlightSearchDTO>(searchInput);
                var jsonContent = JsonSerializer.Serialize(flightSearchData);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("Flight/DisplayFlightBySourceAndDestination", httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    throw new FlightSearchException("Failed to fetch flight data.");
                }

                string flightData = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                flightList = JsonSerializer.Deserialize<List<FlightDetailsDTO>>(flightData, options);

                if (flightList == null || flightList.Count == 0)
                {
                    throw new FlightSearchException("No flights found for the provided search criteria.");
                }

                return View("ShowFlights", flightList);
            }
            catch (Exception ex)
            {
                throw new FlightSearchException($"Exception: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ShowflightCard(string flightNumber)
        {
            var flightdetails = await GetFlightDetailsFromApi(flightNumber); 
            flightdetails.FlightType=flightdetails.FlightId.StartsWith("IF")?"International":"Domestic";
            if (flightdetails != null)
            {
                TempData["UpdateSuccessMessage"] = "Flight updated successfully!";
                return View(flightdetails);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Flight details not found.");
                return View("Edit");
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
                    TempData["UpdateSuccessMessage"] = "Flight updated successfully!";
                    // return RedirectToAction("ShowflightCard", new { flightNumber = flightDetails.FlightId });
                }
                else
                {
                    TempData["UpdateErrorMessage"] = "You Can't Add Flight Sorry, because You are not Authorized!";
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
                throw new FlightDeletionException($"Error deleting flight: {ex.Message}");
            }

            return RedirectToAction("Delete");
        }
        private async Task<FlightDetailsDTO> GetFlightDetailsFromApi(string flightNumber)
        {
            string flightType = flightNumber.StartsWith("IF") ? "International" :
                                flightNumber.StartsWith("DF") ? "Domestic" : null;

            try
            {
                var response = await _client.GetAsync($"{_client.BaseAddress}Flight/MatchFlightByNumberAndType?FlightType={flightType}&flightNumber={flightNumber}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new FlightRetrievalException("Failed to retrieve flight details.");
                }

                return await response.Content.ReadFromJsonAsync<FlightDetailsDTO>();
            }
            catch (Exception ex)
            {
                throw new FlightRetrievalException($"Error retrieving flight details: {ex.Message}");
            }
        }

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

            return View(flightList);
        }

    }
}
