using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using BookToFlyMVC.Models;
using BookToFlyMVC.DTO;
using BookToFlyMVC.Exceptions;
using AutoMapper;
using System.Text.Json;
using System.Text;

namespace BookToFlyMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _client;
        private readonly IMapper _mapper;

        // Constructor: Initializes logger, HttpClient, and AutoMapper
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory client, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _client = client.CreateClient("FlightClient");
        }

        // Returns the login view
        public IActionResult Login()
        {
            return View();
        }

        // Handles the login post request
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginDetails)
        {
            // Validates the model before processing login request
            if (!ModelState.IsValid)
            {
                return View(loginDetails); // Returns the view with validation errors
            }

            try
            {
                // Map LoginViewModel to LoginDTO for API request
                var admin = _mapper.Map<LoginDTO>(loginDetails);
                var jsonContent = JsonSerializer.Serialize(admin); // Serialize login details
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json"); // Create HTTP content for request

                // Sends POST request to the API to authenticate the user
                var response = await _client.PostAsync(_client.BaseAddress + "Admin/Login", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    // Read the JSON response as a string
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    // Parse the JSON and extract only the token
                    var jsonDoc = JsonDocument.Parse(jsonResponse);
                    var token = jsonDoc.RootElement.GetProperty("token").GetString();
                    // Store only the token in session
                    HttpContext.Session.SetString("JWT_TOKEN", token);

                    return RedirectToAction("Dashboard", "Admin"); // Redirect to Admin dashboard on successful login
                }
                else
                {
                    throw new LoginFailedException(); // Throw custom exception if login fails
                }
            }
            catch (LoginFailedException ex)
            {
                // Handle login failure
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                // Handle any other exceptions (e.g., API request issues)
                ModelState.AddModelError(string.Empty, new ApiRequestException(ex.Message, ex).Message);
            }

            // Returns the login view with error messages if login failed
            return View(loginDetails);
        }

        // Handles logout functionality
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Signs out from the authentication scheme
            HttpContext.Session.Remove("JWT_TOKEN"); // Remove the JWT token from the session
            return RedirectToAction("Login"); // Redirect to login page after logging out
        }

        // Returns the home page (index view)
        public IActionResult Index()
        {
            return View();
        }

        // Returns the privacy page view
        public IActionResult Privacy()
        {
            return View();
        }

        // This action method is for the search flight view
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult SearchFlight()
        {
            return View(); // Return the search flight view with no caching
        }
        public IActionResult Error(int? code = null, string errorMessage = null, string additionalDetails = null)
        {
            // Set the error details for the view
            ViewData["ErrorCode"] = code ?? 500;
            ViewData["ErrorMessage"] = errorMessage ?? "No additional information is available.";
            ViewData["AdditionalDetails"] = additionalDetails ?? "No further details available.";

            return View("Error");
        }



    }
}
