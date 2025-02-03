using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using BookToFlyMVC.Exceptions;

namespace BookToFlyMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _client;
        private readonly IMapper _mapper;

        // Constructor for dependency injection
        public AdminController(ILogger<AdminController> logger, IHttpClientFactory client, IMapper mapper)
        {
            _mapper = mapper;
            _logger = logger;
            _client = client;
        }

        // Dashboard view action
        public IActionResult Dashboard()
        {
            try
            {
                return View("Dashboard");
            }
            catch (Exception exception)
            {
                // Log error and throw a custom exception
                _logger.LogError(exception.Message, "Error loading dashboard.");
                throw new DashboardLoadException("An error occurred while loading the dashboard.\n" + exception.Message);
            }
        }

        // Profile view action
        public IActionResult Profile()
        {
            try
            {
                return View();
            }
            catch (Exception exception)
            {
                // Log error and throw a custom exception
                _logger.LogError(exception.Message, "Error loading profile.");
                throw new ProfileLoadException("An error occurred while loading the profile page.\n" + exception.Message);
            }
        }

        // Logout action - redirects to the login page
        public IActionResult Logout()
        {
            try
            {
                return RedirectToAction("Login", "User");
            }
            catch (Exception exception)
            {
                // Log error and throw a custom exception
                _logger.LogError(exception.Message, "Error during logout.");
                throw new LogoutException("An error occurred during logout.\n" + exception.Message);
            }
        }

        // Action for pages that are yet to be developed
        public IActionResult YetToDevelop()
        {
            try
            {
                // Define a redirect URL
                string redirectUrl = "/Admin/Dashboard";

                // Render a shared view with the redirect URL
                return View("~/Views/Shared/_YetToDevelop.cshtml", redirectUrl);
            }
            catch (Exception exception)
            {
                // Log error and throw a custom exception
                _logger.LogError(exception.Message, "Error in YetToDevelop action.");
                throw new YetToDevelopException("An error occurred while handling the YetToDevelop action.\n" + exception.Message);
            }
        }
    }
}
