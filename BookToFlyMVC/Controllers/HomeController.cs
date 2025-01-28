using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using BookToFlyMVC.Models;
using BookToFlyMVC.Data;
using Microsoft.EntityFrameworkCore;
using BookToFlyMVC.DTO;
using AutoMapper;
using System.Text.Json;
using System.Text;

namespace BookToFlyMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly HttpClient _client;
    private readonly IMapper _mapper;
    public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, IHttpClientFactory client, IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
        _client = client.CreateClient("FlightClient");
    }

    public IActionResult Error(int? code = null)
    {
        ViewData["ErrorCode"] = code ?? 500; // Default to 500 if no code is provided
        return View("Error"); // Use a single view for all errors
    }

    public IActionResult Login()
    {
        return View();
    }

    // Login Action
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginDetails)
    {
        if (!ModelState.IsValid)
        {
            return View(loginDetails);
        }

        try
        {
            if (loginDetails.Role == "Admin")
            {
                var admin = _mapper.Map<LoginDTO>(loginDetails);
                var jsonContent = JsonSerializer.Serialize(admin);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(_client.BaseAddress + "Admin/Login", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response to get the token
                    var token = await response.Content.ReadAsStringAsync();

                    // Save the token in the session
                    HttpContext.Session.SetString("JWT_TOKEN", token);

                    // Redirect to Admin Dashboard
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login credentials.");
                }
            }
            else if (loginDetails.Role == "User")
            {
                // Query the User table
                var user = await _dbContext.User
                    .FirstOrDefaultAsync(u => u.Username == loginDetails.Username && u.Password == loginDetails.Password);

                if (user != null)
                {
                    ViewBag.ViewData["RenderHeader"]=true;
                    return RedirectToAction("Index", "User");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid username, password, or role.");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while processing your request from API.");
            Console.WriteLine(ex.Message);
        }

        return View(loginDetails);
    }


    // Logout Action
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Remove("JWT_TOKEN"); // Clear login status
        return RedirectToAction("Login");
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult SearchFlight()
    {
        return View();
    }
}
