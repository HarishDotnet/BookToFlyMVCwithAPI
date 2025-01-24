using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookToFlyMVC.Data;
using BookToFlyMVC.Models;

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

        // User Dashboard
        public IActionResult Index()
        {
            return View(); // Return the user dashboard view
        }
    }
}
