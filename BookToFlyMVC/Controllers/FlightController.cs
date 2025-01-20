using Microsoft.AspNetCore.Mvc;
using BookToFlyMVC.Data;
using AutoMapper;
using BookToFlyMVC.DTO;
using System.Text.Json;
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
         // GET: FlightAdmin/Create
        public IActionResult Create()
        {
            return View("Create");
        }

        // POST: FlightAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlightDetailsDTO flightDetails)
        {
            if (!ModelState.IsValid)
            {
                return View(flightDetails);
            }

            var jsonContent = JsonSerializer.Serialize(flightDetails);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _client.PostAsync(_client.BaseAddress+"Flight/AddFlight", httpContent);

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

    }
}
