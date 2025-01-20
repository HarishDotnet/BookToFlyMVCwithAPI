using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookToFlyMVC.Data;
using BookToFlyMVC.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BookToFlyMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext dbContext, ILogger<UserController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        // Registration Action
        [HttpPost]
        public async Task<ActionResult<UserRegistrationViewModel>> Register([FromForm] UserRegistrationViewModel userDetail)
        {
            if (!ModelState.IsValid)
            {
                return View(userDetail); // Return the view with errors if validation fails
            }

            try
            {
                // Check if the Email already exists
                if (await _dbContext.User.AnyAsync(u => u.Email == userDetail.Email))
                {
                    _logger.LogWarning($"Email {userDetail.Email} is already registered.");
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View("~/Views/User/Register.cshtml", userDetail);
                }

                // Add the user if email and username are unique
                _dbContext.User.Add(userDetail);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"User {userDetail.Username} registered successfully.");
                return RedirectToAction("Login", "User"); // Redirect to login after successful registration
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return View("~/Views/User/Register.cshtml", userDetail);
            }
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
                return RedirectToAction("Dashboard","Admin");
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
                return RedirectToAction("UserDashboard","User");
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
            return RedirectToAction("Index", "Home"); // Redirect to home after logout
        }

        // User Dashboard
        public IActionResult UserDashboard()
        {
            return View("~/Views/UserFunctionality/UserDashboard.cshtml"); // Return the user dashboard view
        }
    }
}
