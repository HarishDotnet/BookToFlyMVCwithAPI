using System;
using System.Net;

namespace BookToFlyMVC.Validations
{
    public class FlightException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public FlightException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) 
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class FlightNotFoundException : FlightException
    {
        public FlightNotFoundException(string flightId) 
            : base($"Flight with ID {flightId} not found.", HttpStatusCode.NotFound)
        {
        }
    }

    public class UnauthorizedFlightAccessException : FlightException
    {
        public UnauthorizedFlightAccessException() 
            : base("You are not authorized to perform this action.", HttpStatusCode.Unauthorized)
        {
        }
    }

    public class FlightApiException : FlightException
    {
        public FlightApiException(string message) 
            : base($"Flight API Error: {message}", HttpStatusCode.InternalServerError)
        {
        }
    }
}
