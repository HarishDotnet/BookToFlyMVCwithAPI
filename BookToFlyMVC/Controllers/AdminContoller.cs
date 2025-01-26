using AutoMapper;
using BookToFlyMVC.Data;

using Microsoft.AspNetCore.Mvc;

namespace BookToFlyMVC.Controllers
{
     public class AdminController : Controller{
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly  IHttpClientFactory _client;
        private readonly IMapper _mapper;
        // Default Show Flights
        public AdminController(ILogger<UserController> logger,ApplicationDbContext dbContext, IHttpClientFactory client, IMapper mapper){
            _mapper=mapper;
            _logger=logger;
            _client=client;
            _dbContext=dbContext;
        }

        public IActionResult Dashboard()
        {
            return View("Dashboard");
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
