using System.ComponentModel.DataAnnotations;

namespace BookToFlyMVC.DTO{
    public class LoginDTO{
        [Key]
       public string Username { get; set; }
        public string Password { get; set; }
    }
}