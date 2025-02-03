using System.ComponentModel.DataAnnotations;
using FlightDetailApi.Data;

namespace FlightDetailApi.Validations
{
    public class CheckUserNameAttribute : ValidationAttribute
    {
        private readonly string _errorMessage;

        public CheckUserNameAttribute(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        // Override the IsValid method to check the username existence in the DB
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Username cannot be null."); // If value is null, fail validation
            }

            string username = value.ToString();

            // Get the ApplicationDbContext from the ValidationContext's service provider
            var dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));
            if (dbContext == null)
            {
                return new ValidationResult("Database context is unavailable.");
            }

            // Check if the username exists in the Users table
            bool userExists = dbContext.Admin.Any(u => u.Username == username);  // Corrected the logic
            if (userExists)
            {
                return new ValidationResult(_errorMessage); // If username exists, return the error message
            }

            // If username doesn't exist, validation passes
            return ValidationResult.Success;
        }
    }
}
