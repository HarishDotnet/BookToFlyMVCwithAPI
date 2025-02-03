using System.ComponentModel.DataAnnotations;

namespace FlightDetailApi.Models
{
    public class UserRegistrationModel
    {
        [Key]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 50 characters.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Username must contain at least one character.")]// Regex to ensure Username contains at least 1 character (not empty)
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [DataType(DataType.Password)]
        // Regex to ensure Password contains at least 1 uppercase, 1 lowercase, and 1 special character
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{6,15}$", ErrorMessage = "Password must be between 6 and 15 characters and contain at least one uppercase letter, one lowercase letter, and one special character.")]
        public string Password { get; set; }

        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        // Regex to ensure FullName contains only letters and spaces
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Full Name should only contain letters and spaces.")]
        public string FullName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
