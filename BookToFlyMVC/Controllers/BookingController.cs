using System.Text.Json;
using FlightDetailApi.Models;
using Microsoft.AspNetCore.Mvc;

public class BookingController : Controller
{
    private readonly HttpClient _httpClient;

    public BookingController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // GET: Ticket/AllTickets
    [HttpGet("Booking/ShowBooking")]
    public async Task<IActionResult> ShowBooking()
    {
        // Define the API endpoint
        var apiUrl = "http://localhost:5087/api/Ticket/GetAllTickets";

        // Call the API
        var response = await _httpClient.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            // Console.WriteLine("inside");
            // Read and deserialize the response content
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var tickets = JsonSerializer.Deserialize<List<TicketDetails>>(content,options);;
            // Pass the data to the view
            // Console.WriteLine(tickets[0].Username);
            
            return View(tickets);
        }
        else
        {
            // Console.WriteLine("outside");
            // Handle the case where the API call fails
            return View();
        }
    }
}
