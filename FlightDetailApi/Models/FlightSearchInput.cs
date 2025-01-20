using System.ComponentModel.DataAnnotations;

namespace FlightDetailsApi.Models
{
    public class FlightSearchInput
    {
        [Required]
        public string FlightType{get; set;}
        [Required]
        public string Source { get; set; }
        [Required]
        public string Destination { get; set; }
    }
}
