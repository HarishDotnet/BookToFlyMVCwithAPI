using AutoMapper;
using FlightDetailsApi.Data;
using FlightDetailsApi.Models;
using FlightDetailsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContextMVC _mvcContext; // MVC database context
    private readonly JWTTokenService _jwtTokenService; // JWT Service
    private readonly IMapper _mapper;
    public AdminController(ApplicationDbContextMVC mvcContext, JWTTokenService jwtTokenService,IMapper mapper)
    {
        _mvcContext = mvcContext;
        _mapper=mapper;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] AdminLoginDTO login)
    {
        if (login == null || string.IsNullOrEmpty(login.Username))
        {
            return BadRequest(new { success = false, message = "username or password should not be null" });
        }
        // Check if the user exists
        var user = await _mvcContext.admin.FirstOrDefaultAsync(u => u.Username == login.Username);
        if (user == null)
        {
            return Unauthorized( "Invalid username or password." );
        }
        // Verify the password
        if (!login.Password.Equals( user.Password))
        {
            return Unauthorized( "Invalid username or password." );
        }
        // Generate JWT Token
        var token = _jwtTokenService.GenerateJWTToken(user.Username, "Admin");

        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] AdminModel register)
    {
        // Validate the model
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        // Check if the username already exists
        var existingUser = await _mvcContext.admin.FirstOrDefaultAsync(u => u.Username == register.Username);
        if (existingUser != null)
        {
            return Conflict(new { success = false, message = "Username is already taken." });
        }

        _mvcContext.admin.Add(register);
        await _mvcContext.SaveChangesAsync();

        return Ok(new { success = true, message = "Registration successful" });
    }
}
