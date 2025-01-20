namespace BookToFlyMVC.Models
{
    public class UserDashboardModel
    {
        public List<InternationalFlightModel> Flights { get; set; } = new List<InternationalFlightModel>();
        public List<BookedTicket> BookedTickets { get; set; } = new List<BookedTicket>();
    }

}
