using FlightDetailApi.Models;
using FlightDetailApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightDetailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;
        private readonly JWTTokenService _jwtTokenService;
        private readonly ILogger<AdminController> _logger;

        // Constructor to initialize dependencies
        public AdminController(AdminService adminService, JWTTokenService jwtTokenService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginDTO login)
        {
            _logger.LogInformation("Login attempt for user: {Username}", login?.Username);

            // Validate input
            if (login == null || string.IsNullOrEmpty(login.Username))
            {
                _logger.LogWarning("Login failed: Username or password is null or empty.");
                return BadRequest(new { success = false, message = "Username or password should not be null." });
            }

            // Attempt to authenticate the admin user
            var admin = await _adminService.LoginAsync(login.Username, login.Password);
            if (admin == null)
            {
                _logger.LogWarning("Login failed: Invalid username or password.");
                return Unauthorized(new { success = false, message = "Invalid username or password." });
            }

            // Generate JWT token upon successful authentication
            var token = _jwtTokenService.GenerateJWTToken(admin.Username, "Admin");
            _logger.LogInformation("Login successful for user: {Username}. JWT token generated.", login.Username);

            return Ok(token);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AdminModel register)
        {
            _logger.LogInformation("Registration attempt for user: {Username}", register?.Username);

            // Validate model state
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Registration failed: Invalid model state. Errors: {Errors}", string.Join(", ", errors));
                return BadRequest(new { success = false, errors});
            }

            // Attempt to register a new admin
            var success = await _adminService.RegisterAsync(register);
            // if (!success)
            // {
            //     _logger.LogWarning("Registration failed: Username {Username} is already taken.", register.Username);
            //     return Conflict(new { success = false, message = "Username is already taken." });
            // }

            _logger.LogInformation("Registration successful for user: {Username}", register.Username);
            return Ok(new { success = true, message = "Registration successful." });
        }
    }
}