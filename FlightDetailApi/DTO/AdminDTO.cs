using System.ComponentModel.DataAnnotations;
using FlightDetailsApi.Validations;


namespace FlightDetailsApi.Models
{
    public class AdminLoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }

    }
}