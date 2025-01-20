using System.ComponentModel.DataAnnotations;
namespace BookToFlyMVC.Models
{
    public class LoginViewModel
{
    [Required]
    [StringLength(100, ErrorMessage = "Username must be between 1 and 100 characters.", MinimumLength = 1)]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    public string Role { get; set; }  // 'User' or 'Admin'
}

}
