using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookToFlyMVC.Models
{
    public class DomesticFlightModel
    {
        [Key]
        // Unique identifier for the flight
        public int FlightId { get; set; }

        // Airline operating the flight
        public string AirlineName { get; set; }

        // Source airport code (e.g., JFK)
        public string Source { get; set; }

        // Destination airport code (e.g., LHR)
        public string Destination { get; set; }

        // Departure time
        public TimeSpan DepartureTime { get; set; }

        // Arrival time
        public TimeSpan ArrivalTime { get; set; }

        // Total duration of the flight in hours
        public double Duration { get; set; }

        // Price of the flight ticket
        public decimal Price { get; set; }

        // Available seats for the flight
        public int AvailableSeats { get; set; }

        // Additional notes or remarks about the flight
        public string Remarks { get; set; }

        // List of days the flight is available
        public List<string> AvailableDays { get; set; }
    }
}
