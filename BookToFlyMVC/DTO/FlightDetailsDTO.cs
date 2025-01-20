using System.ComponentModel.DataAnnotations;

namespace BookToFlyMVC.DTO
{
    public class FlightDetailsDTO
    {   
         public string FlightId { get; set; }

        public string AirlineName { get; set; }

        public string Source { get; set; }


        public string Destination { get; set; }


        public TimeSpan ArrivalTime { get; set; }


        public TimeSpan DepartureTime { get; set; }

        public double Duration { get; set; }


        public int AvailableSeats { get; set; }

        public decimal TicketPrice { get; set; }


        public List<string> AvailableDays { get; set; }

        // public string LastModifiedBy { get; set; }

    }
}