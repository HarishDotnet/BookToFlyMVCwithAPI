namespace BookToFlyMVC.Exceptions
{
    public class AdminException : Exception
    {
        public AdminException(string message) : base(message) { }
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
