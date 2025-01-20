using System;
using System.ComponentModel.DataAnnotations;

namespace BookToFlyMVC.Models
{
    public class BookedTicket
    {
        public int Id { get; set; }  // Unique identifier for the booking

        [Required]
        public string FlightNumber { get; set; }  // Flight Number for the booked flight

        [Required]
        public string FromPlace { get; set; }  // Departure location

        [Required]
        public string ToPlace { get; set; }  // Destination location

        public DateTime DepartureTime { get; set; }  // Flight departure time

        [Required]
        public string UserId { get; set; }  // The user who booked the ticket (foreign key to User)

        public string UserName { get; set; }  // User's name (optional, for easy display)

        public DateTime BookingDate { get; set; }  // Date when the ticket was booked

        public bool IsCancelled { get; set; }  // Flag to check if the booking is cancelled

        public string Status
        {
            get
            {
                return IsCancelled ? "Cancelled" : "Booked";
            }
        }
    }
}
