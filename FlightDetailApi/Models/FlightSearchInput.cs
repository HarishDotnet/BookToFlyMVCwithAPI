using System.ComponentModel.DataAnnotations;

namespace FlightDetailApi.Models
{
    public class FlightSearchInput
    {
        [Required]
        public string FlightType { get; set; }
        // public string FlightNumber { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
    }
}
