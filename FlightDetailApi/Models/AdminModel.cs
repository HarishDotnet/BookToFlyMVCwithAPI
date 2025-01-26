using System.ComponentModel.DataAnnotations;
using FlightDetailApi.Validations;


namespace FlightDetailApi.Models
{
    public class AdminModel
    {
        [Key]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        [CheckUserName("User Name Already Exist")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}