using System.Text.Json;
using FlightDetailApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

public class BookingController : Controller
{
    private readonly HttpClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookingController"/> class.
    /// </summary>
    /// <param name="httpClientFactory">Injected HttpClientFactory for creating named HttpClient instances.</param>
    public BookingController(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient("FlightClient"); // Use the named client
    }
    /// <summary>
    /// Retrieves all booking tickets from the API and displays them in the view.
    /// </summary>
    /// <returns>An IActionResult containing the list of tickets or an empty view if the request fails.</returns>
    [HttpGet("Booking/ShowBooking")]
    public async Task<IActionResult> ShowBooking()
    {
        // Call the API to fetch all tickets
        var response = await _client.GetAsync("Ticket/GetAllTickets"); // BaseAddress is already set

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var tickets = JsonSerializer.Deserialize<List<TicketDetails>>(content, options);
            return View(tickets);
        }
        else
        {
            return View(); // Return empty view on failure
        }
    }
}
