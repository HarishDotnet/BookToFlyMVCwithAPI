// using Microsoft.AspNetCore.Mvc;
// using BookToFlyMVC.Data;
// using AutoMapper;
// using BookToFlyMVC.DTO;
// using System.Text.Json;
// using FlightDetailsApi.Models;
// using System.Text;
// namespace BookToFlyAPI.Controllers
// {
//     public class FlightController : Controller
//     {
//         private readonly HttpClient _client;
//         private readonly IMapper _mapper;
//         private readonly ApplicationDbContext _dbContext;

//         public FlightController(ApplicationDbContext dbContext, IMapper mapper, IHttpClientFactory clientFactory)
//         {
//             _dbContext = dbContext;
//             _mapper = mapper;
//             _client = clientFactory.CreateClient("FlightClient");
//         }

//         [HttpGet]
//         // GET: FlightAdmin/Create
//         public IActionResult Create()
//         {
//             return View("Create");
//         }

//         // POST: FlightAdmin/Create
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Create([FromForm] FlightDetailsDTO flightDetails)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return View(flightDetails);
//             }

//             var jsonContent = JsonSerializer.Serialize(flightDetails);
//             var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

//             try
//             {
//                 var response = await _client.PostAsync(_client.BaseAddress + "Flight/AddFlight", httpContent);

//                 if (response.IsSuccessStatusCode)
//                 {
//                     TempData["SuccessMessage"] = "Flight added successfully!";
//                     return RedirectToAction("Create");
//                 }
//                 else
//                 {
//                     var errorMessage = await response.Content.ReadAsStringAsync();
//                     ModelState.AddModelError("", $"Error: {errorMessage}");
//                 }
//             }
//             catch (HttpRequestException ex)
//             {
//                 ModelState.AddModelError("", $"HTTP Request Error: {ex.Message}");
//             }

//             return View(flightDetails);
//         }

//         public IActionResult SearchFlights()
//         {
//             return View("SearchFlights");
//         }

//         //Used to display the flight details by Searching for User
//         [HttpPost("Flight/SearchFlights")]
//         public async Task<IActionResult> SearchFlights(FlightSearchInput searchInput)
//         {
//             Console.WriteLine($"Fetching flight details with Flight Type: {searchInput.FlightType}, Source: {searchInput.Source}, Destination: {searchInput.Destination}, Flight Number: {searchInput.FlightNumber}");

//             // List to hold flight data
//             List<FlightDetailsDTO> flightList = new List<FlightDetailsDTO>();
//             var flightSearchData = new FlightSearchDTO();
//             try
//             {
//                 // Determine which API endpoint to call based on user input
//                 HttpResponseMessage response = null;

//                 if (!string.IsNullOrEmpty(searchInput.FlightType) && string.IsNullOrEmpty(searchInput.FlightNumber) && !string.IsNullOrEmpty(searchInput.Source) && !string.IsNullOrEmpty(searchInput.Destination))
//                 {
//                     // Call API to get flights by Source and Destination
//                     flightSearchData = _mapper.Map<FlightSearchDTO>(searchInput);
//                     var jsonContent = JsonSerializer.Serialize(flightSearchData);
//                     var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
//                     response = await _client.PostAsync("Flight/DisplayFlightBySourceAndDestination", httpContent);
//                     if (response.IsSuccessStatusCode)
//                     {
//                         string flightData = await response.Content.ReadAsStringAsync();

//                         // Deserialize the JSON into a list of FlightOutputDTO
//                         var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
//                         flightList = JsonSerializer.Deserialize<List<FlightDetailsDTO>>(flightData, options);
//                     }
//                     else
//                     {
//                         ModelState.AddModelError(string.Empty, "Failed to fetch flight data.");
//                     }

//                 }
//                 else if (!string.IsNullOrEmpty(searchInput.FlightType) && !string.IsNullOrEmpty(searchInput.FlightNumber))
//                 {
//                     // Call API to get flights by Flight Type (and Flight Number)
//                     response = await _client.GetAsync($"Flight/DisplayFlightByType?FlightType={searchInput.FlightType}&flightNumber={searchInput.FlightNumber}");
//                     if (response.IsSuccessStatusCode)
//                     {
//                         string flightData = await response.Content.ReadAsStringAsync();

//                         // Deserialize the JSON into a list of FlightOutputDTO
//                         var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
//                         FlightDetailsDTO flight = JsonSerializer.Deserialize<FlightDetailsDTO>(flightData, options);
//                         flightList.Add(flight);
//                     }
//                     else
//                     {
//                         ModelState.AddModelError(string.Empty, "Failed to fetch flight data.");
//                     }
//                 }
//                 else
//                 {
//                     ModelState.AddModelError(string.Empty, "Invalid search parameters.");
//                     return View("Error");
//                 }
//             }
//             catch (Exception ex)
//             {
//                 ModelState.AddModelError(string.Empty, $"Exception: {ex.Message}");
//             }

//             // Check if flight data is available
//             if (flightList == null)
//             {
//                 ModelState.AddModelError(string.Empty, "No flights found for the provided search criteria.");
//                 return View("Error");
//             }

//             // Return the view with the list of flights
//             return View("ShowFlights", flightList); // Ensure a view exists to display the list of flights
//         }


//         //Used to display the flight details with Actions for Admin
//         [HttpPost("GetFlightsByType")]
//         public async Task<IActionResult> GetFlightsByType([FromForm] string FlightType)
//         {
//             HttpResponseMessage respose = await _client.GetAsync($"Flight/DisplayFlightByType/{FlightType}");
//             List<FlightDetailsDTO> flightList = new List<FlightDetailsDTO>();
//             try
//             {
//                 if (respose.IsSuccessStatusCode)
//                 {
//                     string flightData = await respose.Content.ReadAsStringAsync();
//                     var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
//                     flightList = JsonSerializer.Deserialize<List<FlightDetailsDTO>>(flightData, options);
//                     Console.WriteLine("Hi");

//                 }
//                 else
//                 {
//                     return View("Error to get the data from the api");
//                 }
//             }
//             catch (Exception ex)
//             {
//                 return View("Error" + ex.Message);
//             }

//             return View("ShowFlightWithOptions", flightList);
//         }
//         [HttpGet]
//         public IActionResult Edit()
//         {
//             return View();
//         }
//         [HttpPost]
//         public async Task<IActionResult> Edit([FromForm] string FlightType, [FromForm] string flightNumber)
//         {
//             Console.WriteLine("Fetching flights for FlightType: " + FlightType);
//             try
//             {
//                 FlightDetailsDTO flightList;
//                 if (!string.IsNullOrEmpty(flightNumber))
//                 {
//                     // Call the GetFlightDetails API for the specific flightNumber
//                     HttpResponseMessage flightResponse = await _client.GetAsync($"Flight/DisplayFlightByType?FlightType={FlightType}&flightNumber={flightNumber}");
//                     if (flightResponse.IsSuccessStatusCode)
//                     {
//                         string flight = await flightResponse.Content.ReadAsStringAsync();
//                         var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
//                         flightList = JsonSerializer.Deserialize<FlightDetailsDTO>(flight, options);
//                         flightList.FlightType = FlightType;
//                         return View("UpdateFlight", flightList); // Return the single flight detail
//                     }
//                     else
//                     {
//                         return NotFound(new { message = "Error while fetching data." });
//                     }
//                 }
//                 else
//                 {
//                     return BadRequest(new { message = "Flight not found." });

//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Error: {ex.Message}");
//                 return StatusCode(500, "An internal error occurred while fetching flight details.");
//             }
//         }



//         [HttpGet("ShowflightCard/Details/{flightNumber}")]
//         public IActionResult ShowflightCard(string flightNumber)
//         {
//             var flightdetails=GetFlightDetailsFromApi(flightNumber);
//             if(flightdetails!=null)
//                 return View(flightdetails);
//             else 
//                 return View();
//         }
//         [HttpPost]
//         public async Task<IActionResult> UpdateFlight(FlightDetailsDTO flightDetails)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return View(flightDetails);
//             }
//             // var flight=_mapper.Map<flightde>
//             var jsonContent = JsonSerializer.Serialize(flightDetails);
//             var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

//             try
//             {
//                 var response = await _client.PutAsync(_client.BaseAddress + $"Flight/UpdateFlight/{flightDetails.FlightId}", httpContent);

//                 if (response.IsSuccessStatusCode)
//                 {
//                     TempData["FlightDetails"] = JsonSerializer.Serialize(flightDetails);
//                     TempData["SuccessMessage"] = "Flight updated successfully!";
//                     return RedirectToAction("ShowflightCard");
//                 }
//                 else
//                 {
//                     var errorMessage = await response.Content.ReadAsStringAsync();
//                     ModelState.AddModelError("", $"Error: {errorMessage}");
//                 }
//             }
//             catch (HttpRequestException ex)
//             {
//                 ModelState.AddModelError("", $"HTTP Request Error: {ex.Message}");
//             }

//             return View(flightDetails);
//         }
//         [HttpGet]
//         public IActionResult Delete()
//         {
//             // This action renders the view to delete the flight
//             return View();
//         }

//         [HttpGet]
//         public async Task<IActionResult> GetFlightDetails(string flightNumber)
//         {
//             // Replace with actual logic to fetch flight details
//             var flightDetails = await GetFlightDetailsFromApi(flightNumber);
//             if (flightDetails != null)
//             {
//                 return Json(flightDetails);
//             }
//             return NotFound();
//         }

//         [HttpPost]
//         public async Task<IActionResult> Delete(string flightId)
//         {
//             try
//             {
//                 using (var httpClient = new HttpClient())
//                 {
//                     var response = await _client.DeleteAsync($"{_client.BaseAddress}Flight/DeleteFlight/{flightId}");
//                     if (response.IsSuccessStatusCode)
//                     {
//                         return Ok();
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 // Log the exception
//                 Console.WriteLine(ex.Message);
//             }

//             return BadRequest();
//         }

//         private async Task<FlightDetailsDTO> GetFlightDetailsFromApi(string flightNumber)
//         {
//             string flightType;
//             if(flightNumber.StartsWith("IF"))  flightType="International";
//             else flightType=flightNumber.StartsWith("DF")?"Domestic":null;
//             // Logic to call an API to fetch flight details
//                 var response = await _client.GetAsync($"{_client.BaseAddress}Flight/DisplayFlightByType?FlightType={flightType}&flightNumber={flightNumber}");

//                 if (response.IsSuccessStatusCode)
//                 {
//                     return await response.Content.ReadFromJsonAsync<FlightDetailsDTO>();
//                 }
//             return null;
//         }
//     }
// }

using Microsoft.AspNetCore.Mvc;
using BookToFlyMVC.Data;
using AutoMapper;
using BookToFlyMVC.DTO;
using System.Text.Json;
using FlightDetailsApi.Models;
using System.Text;

namespace BookToFlyAPI.Controllers
{
    public class FlightController : Controller
    {
        private readonly HttpClient _client;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public FlightController(ApplicationDbContext dbContext, IMapper mapper, IHttpClientFactory clientFactory)
        {
            _dbContext = dbContext;
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
                var response = await _client.PostAsync(_client.BaseAddress + "Flight/AddFlight", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Flight added successfully!";
                    return RedirectToAction("Create");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error: {errorMessage}");
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"HTTP Request Error: {ex.Message}");
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

        [HttpGet("ShowflightCard/Details/{flightNumber}")]
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

        [HttpPost]
        public async Task<IActionResult> UpdateFlight(FlightDetailsDTO flightDetails)
        {
            if (!ModelState.IsValid)
            {
                return View(flightDetails);
            }

            var jsonContent = JsonSerializer.Serialize(flightDetails);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _client.PutAsync(_client.BaseAddress + $"Flight/UpdateFlight/{flightDetails.FlightId}", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["FlightDetails"] = JsonSerializer.Serialize(flightDetails);
                    TempData["SuccessMessage"] = "Flight updated successfully!";
                    return RedirectToAction("ShowflightCard", new { flightNumber = flightDetails.FlightId });
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error: {errorMessage}");
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"HTTP Request Error: {ex.Message}");
            }

            return View(flightDetails);
        }
        public IActionResult Delete()
        {
            // This action renders the view to delete the flight
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(string flightId)
        {
            if (string.IsNullOrEmpty(flightId))
            {
                return BadRequest("Invalid flight ID.");
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await _client.DeleteAsync($"{_client.BaseAddress}Flight/DeleteFlight/{flightId}");
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Flight deleted successfully.";
                        return RedirectToAction("Delete"); // Redirect to the same page
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "An error occurred while deleting the flight.";
            }

            return RedirectToAction("Delete"); // Redirect to the same page
        }


        private async Task<FlightDetailsDTO> GetFlightDetailsFromApi(string flightNumber)
        {
            string flightType = flightNumber.StartsWith("IF") ? "International" :
                flightNumber.StartsWith("DF") ? "Domestic" : null;

            if (flightType == null)
                return null;

            var response = await _client.GetAsync($"{_client.BaseAddress}Flight/DisplayFlightByType?FlightType={flightType}&flightNumber={flightNumber}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<FlightDetailsDTO>();
            }

            return null;
        }
    }
}
