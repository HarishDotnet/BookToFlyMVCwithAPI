using System;

namespace BookToFlyMVC.Exceptions
{
    public class AdminException : Exception
    {
        public AdminException() { }
        public AdminException(string message) : base(message) { }
        public AdminException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DashboardLoadException : AdminException
    {
        public DashboardLoadException(string message) : base(message) { }
    }

    public class ProfileLoadException : AdminException
    {
        public ProfileLoadException(string message) : base(message) { }
    }

    public class LogoutException : AdminException
    {
        public LogoutException(string message) : base(message) { }
    }

    public class YetToDevelopException : AdminException
    {
        public YetToDevelopException(string message) : base(message) { }
    }
}
