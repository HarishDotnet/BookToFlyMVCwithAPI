using AutoMapper;
using BookToFlyMVC.Data;

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

        // Default Show Flights

        public IActionResult Dashboard()
        {
            return View("Dashboard");
        }

        public IActionResult ManageFlights()
        {
            return View("ManageFlights");
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Login", "User");
        }
        public IActionResult YetToDevelop()
        {
            // Define the URL or string that will be passed to the view (you can modify this to whatever you want)
            string redirectUrl = "/Admin/Dashboard"; // Example URL to redirect to the Dashboard

            // Return the _YetToDevelop.cshtml view with the string model (redirect URL)
            return View("~/Views/Shared/_YetToDevelop.cshtml", redirectUrl);
        }

    }
}
