using System.ComponentModel.DataAnnotations;

namespace BookToFlyMVC.Models
{
    public class InternationalFlightModel
    {
        [Key]
        [Required]
        [RegularExpression(@"^(IF)[0-9]{1,4}$", ErrorMessage = "Flight number must start with 'IF' (International Flight) followed by 1 to 4 digits.")]
        public string FlightId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Flight name cannot exceed 100 characters.")]
        public string AirlineName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Source location cannot exceed 50 characters.")]
        public string Source { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Destination location cannot exceed 50 characters.")]
        public string Destination { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan ArrivalTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan DepartureTime { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Duration must be a positive number.")]
        public double Duration { get; set; }

        [Required]
        [Range(1, 500, ErrorMessage = "Available seats should be between 1 and 500.")]
        public int AvailableSeats { get; set; }

        [Required]
        [Range(1000, 50000, ErrorMessage = "Ticket price should be between 1000 and 50000.")]
        public decimal TicketPrice { get; set; }

        // List of days the flight is available
        [Required(ErrorMessage = "Available days are required.")]
        [MinLength(1, ErrorMessage = "There must be at least one available day.")]
        public List<string> AvailableDays { get; set; }

        
        // Last updated by admin name
        public string LastModifiedBy { get; set; }

    }
}