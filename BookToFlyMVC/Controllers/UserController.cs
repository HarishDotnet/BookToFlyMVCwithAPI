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

        public IActionResult Register(){
            return View();
        }
        // Registration Action
        [HttpPost]
        public async Task<ActionResult<UserRegistrationViewModel>> Register([FromForm] UserRegistrationViewModel userDetail)
        {
            if (userDetail.Username == null)
            {
                return View();
            }
            if (!ModelState.IsValid)
            {
                // If there are validation errors, add them to ViewBag
                ViewBag.UsernameError = ModelState["Username"]?.Errors?.FirstOrDefault()?.ErrorMessage;
                ViewBag.EmailError = ModelState["Email"]?.Errors?.FirstOrDefault()?.ErrorMessage;
                ViewBag.PasswordError = ModelState["Password"]?.Errors?.FirstOrDefault()?.ErrorMessage;
                ViewBag.FullNameError = ModelState["FullName"]?.Errors?.FirstOrDefault()?.ErrorMessage;
                ViewBag.PhoneNumberError = ModelState["PhoneNumber"]?.Errors?.FirstOrDefault()?.ErrorMessage;

                return View(userDetail); // Return the view with errors in ViewBag
            }

            try
            {
                // Check if the Email already exists
                if (await _dbContext.User.AnyAsync(u => u.Email == userDetail.Email))
                {
                    _logger.LogWarning($"Email {userDetail.Email} is already registered.");
                    ViewBag.EmailError = "Email is already registered.";
                    return View("~/Views/User/Register.cshtml", userDetail);
                }

                // Check if the Username already exists
                if (await _dbContext.User.AnyAsync(u => u.Username == userDetail.Username))
                {
                    _logger.LogWarning($"Username {userDetail.Username} is already taken.");
                    ViewBag.UsernameError = "Username is already taken.";
                    return View("~/Views/User/Register.cshtml", userDetail);
                }

                // Map UserRegistrationViewModel to User entity
                var user = new UserRegistrationViewModel
                {
                    Username = userDetail.Username,
                    Email = userDetail.Email,
                    Password = userDetail.Password, // You should hash the password before saving it
                    FullName = userDetail.FullName,
                    PhoneNumber = userDetail.PhoneNumber
                };

                // Save the new user
                _dbContext.User.Add(user);
                await _dbContext.SaveChangesAsync();
                ViewBag.Registration = "Registration Succsessfull";
                _logger.LogInformation($"User {userDetail.Username} registered successfully.");
                return RedirectToAction("Login", "Home"); // Redirect to login after successful registration
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                ViewBag.Error = "Something went wrong during registration. Please try again.";
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
