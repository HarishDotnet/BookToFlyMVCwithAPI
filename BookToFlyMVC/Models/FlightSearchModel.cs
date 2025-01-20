using System.ComponentModel.DataAnnotations;

namespace BookToFlyMVC.Models
{
    public class FlightSearchModel
    {
        [Required]
        public string FlightType { get; set; }
        public string FlightNumber { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
    }
}
