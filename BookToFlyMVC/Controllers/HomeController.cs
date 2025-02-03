using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using BookToFlyMVC.Models;
using BookToFlyMVC.DTO;
using BookToFlyMVC.Exceptions;
using AutoMapper;
using System.Text.Json;
using System.Text;

namespace BookToFlyMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _client;
    private readonly IMapper _mapper;
    public HomeController(ILogger<HomeController> logger, IHttpClientFactory client, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
        _client = client.CreateClient("FlightClient");
    }

    public IActionResult Error(int? code = null)
    {
        ViewData["ErrorCode"] = code ?? 500;
        return View("Error");
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginDetails)
    {
        if (!ModelState.IsValid)
        {
            return View(loginDetails);
        }

        try
        {
            var admin = _mapper.Map<LoginDTO>(loginDetails);
            var jsonContent = JsonSerializer.Serialize(admin);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(_client.BaseAddress + "Admin/Login", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                HttpContext.Session.SetString("JWT_TOKEN", token);
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                throw new LoginFailedException();
            }
        }
        catch (LoginFailedException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, new ApiRequestException(ex.Message, ex).Message);
        }

        return View(loginDetails);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Remove("JWT_TOKEN");
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
