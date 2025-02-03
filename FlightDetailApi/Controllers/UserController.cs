using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightDetailApi.Data;
using FlightDetailApi.Models;
using FlightDetailApi.DTO;
using BCrypt.Net;

namespace FlightDetailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext dbContext, ILogger<UserController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // POST: api/user/register
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserRegistrationModel userDetail)
        {
            if (userDetail == null || string.IsNullOrWhiteSpace(userDetail.Username))
            {
                return BadRequest(new { message = "Username is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            try
            {
                // Check if Email already exists
                if (await _dbContext.Users.AnyAsync(u => u.Email == userDetail.Email))
                {
                    _logger.LogWarning($"Email {userDetail.Email} is already registered.");
                    return Conflict(new { message = "Email is already registered." });
                }

                // Check if Username already exists
                if (await _dbContext.Users.AnyAsync(u => u.Username == userDetail.Username))
                {
                    _logger.LogWarning($"Username {userDetail.Username} is already taken.");
                    return Conflict(new { message = "Username is already taken." });
                }

                // Hash password using BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDetail.Password);

                // Map UserRegistrationModel to User entity
                var newUser = new UserRegistrationModel
                {
                    Username = userDetail.Username,
                    Email = userDetail.Email,
                    Password = hashedPassword, // Store hashed password
                    FullName = userDetail.FullName,
                    PhoneNumber = userDetail.PhoneNumber
                };

                // Save the new user
                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"User {userDetail.Username} registered successfully.");
                return Ok(new { message = "Registration Successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return StatusCode(500, new { message = "Something went wrong during registration. Please try again." });
            }
        }

        // POST: api/user/login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserLoginDTO loginDetails)
        {
            if (loginDetails == null || string.IsNullOrEmpty(loginDetails.Username) || string.IsNullOrEmpty(loginDetails.Password))
            {
                return BadRequest(new { message = "Username and password cannot be empty." });
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == loginDetails.Username);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // Verify the password using BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDetails.Password, user.Password);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // Successful login (You can add JWT authentication here)
            return Ok(new { message = "Login successful" });
        }

        // GET: api/user/{username}
        [HttpGet("{username}")]
        public async Task<ActionResult> GetUserByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { message = "Username is required." });
            }

            var user = await _dbContext.Users
                .Where(u => u.Username == username)
                .Select(u => new
                {
                    u.Username,
                    u.Email,
                    u.FullName,
                    u.PhoneNumber
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }
    }
}
