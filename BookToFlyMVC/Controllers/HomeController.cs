using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using BookToFlyMVC.Models;
using BookToFlyMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace BookToFlyMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet]
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
            // If model validation fails, return the view with errors
            return View(loginDetails);
        }

        try
        {
            // Check the Role and query the corresponding table
            if (loginDetails.Role == "Admin")
            {
                // Query the Admin table
                var admin = await _dbContext.Admin
                    .FirstOrDefaultAsync(a => a.Username == loginDetails.Username && a.Password == loginDetails.Password);

                if (admin != null)
                {
                    // Login successful - Redirect to Admin Dashboard
                    return RedirectToAction("Index", "Admin");
                }
            }
            else if (loginDetails.Role == "User")
            {
                // Query the User table
                var user = await _dbContext.User
                    .FirstOrDefaultAsync(u => u.Username == loginDetails.Username && u.Password == loginDetails.Password);

                if (user != null)
                {
                    // Login successful - Redirect to User Home Page
                    return RedirectToAction("Index", "User");
                }
            }

            // If no match is found, add a model error
            ModelState.AddModelError(string.Empty, "Invalid username, password, or role.");
        }
        catch (Exception ex)
        {
            // Log the exception and return a generic error message
            ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
            Console.WriteLine(ex.Message);
        }

        // If login fails, return the view with the login details and errors
        return View(loginDetails);
    }

    // Logout Action
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Home"); // Redirect to home after logout
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
