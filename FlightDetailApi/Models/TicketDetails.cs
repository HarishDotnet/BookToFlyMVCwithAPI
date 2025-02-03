using System.ComponentModel.DataAnnotations;

namespace FlightDetailApi.Models{
    public class TicketDetails{
        [Key]
        public string Username{get; set;}
        public List<int> BookingId{get; set;}
    }
}