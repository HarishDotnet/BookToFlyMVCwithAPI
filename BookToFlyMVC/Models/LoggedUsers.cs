using System.ComponentModel.DataAnnotations;
namespace BookToFlyMVC.Models
{
    public class LoggedUsers
    {
        [Key]
        public string Name {get;set;}
    }
}