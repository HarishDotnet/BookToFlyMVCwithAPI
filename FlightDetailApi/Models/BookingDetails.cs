using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace FlightDetailApi
{
    public class BookingDetails
    {
        [Key]
        [Required]
        public string BookingId { get; set; }  // Unique ID for the booking

        [Required(ErrorMessage = "Flight ID is required.")]
        public string FlightId { get; set; }   // Associated Flight ID

        [Required(ErrorMessage = "Passenger name is required.")]
        [StringLength(100, ErrorMessage = "Passenger name cannot exceed 100 characters.")]
        public string PassengerName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Phone number must start with 6, 7, 8, or 9 and be exactly 10 digits.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Booking date is required.")]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Date of travel is required.")]
        [CustomValidation(typeof(BookingDetails), nameof(ValidateTravelDate))]
        public DateTime DateOfTravel { get; set; } // Date of travel

        public bool IsCanceled { get; set; }  // True if the ticket is canceled

        // Custom validation method for DateOfTravel
        public static ValidationResult ValidateTravelDate(DateTime date, ValidationContext context)
        {
            if (date < DateTime.Today)
            {
                return new ValidationResult("Date of travel cannot be in the past.");
            }
            return ValidationResult.Success;
        }
    }
}
