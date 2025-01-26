using System.ComponentModel.DataAnnotations;
using FlightDetailApi.Validations;


namespace FlightDetailApi.Models
{
    public class AdminLoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }

    }
}