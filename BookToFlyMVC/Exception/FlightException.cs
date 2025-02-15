using System;

namespace BookToFlyMVC.Exceptions
{
    // Base exception class for flight-related errors
    public class FlightException : Exception
    {
        public FlightException(string message) : base(message) { }
    }

    public class FlightCreationException : FlightException
    {
        public FlightCreationException(string message) : base(message) { }
    }

    public class FlightSearchException : FlightException
    {
        public FlightSearchException(string message) : base(message) { }
    }

    public class FlightUpdateException : FlightException
    {
        public FlightUpdateException(string message) : base(message) { }
    }

    public class FlightDeletionException : FlightException
    {
        public FlightDeletionException(string message) : base(message) { }
    }

    public class FlightRetrievalException : FlightException
    {
        public FlightRetrievalException(string message) : base(message) { }
    }
    public class LoginFailedException : Exception
    {
        public LoginFailedException() : base("Invalid login credentials.") { }
    }

    public class ApiRequestException : Exception
    {
        public ApiRequestException(string message, Exception inner) : base(message, inner) { }
    }
     public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }

}
