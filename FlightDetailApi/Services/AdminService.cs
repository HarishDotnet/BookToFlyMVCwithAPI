using FlightDetailApi.Models;
using FlightDetailApi.Repositories;
namespace FlightDetailApi.Services
{
    public class AdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminModel> LoginAsync(string username, string password)
        {
            try
            {
                var admin = (await _unitOfWork.Admins.FindAsync(a => a.Username == username)).FirstOrDefault();

                if (admin == null || !VerifyPassword(password, admin.Password))
                    return null;

                return admin;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred during login.", ex);
            }
        }

        public async Task<bool> RegisterAsync(AdminModel admin)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));

            try
            {
                var existingAdmin = (await _unitOfWork.Admins.FindAsync(a => a.Username == admin.Username)).FirstOrDefault();
                if (existingAdmin != null)
                    return false;

                admin.Password = HashPassword(admin.Password);
                await _unitOfWork.Admins.AddAsync(admin);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred during registration.", ex);
            }
        }

        private bool VerifyPassword(string inputPassword, string storedPasswordHash)
        {
            // Implement secure password verification
            return inputPassword == storedPasswordHash; // Replace with secure comparison
        }

        private string HashPassword(string password)
        {
            // Implement secure password hashing
            return password; // Replace with secure hashing
        }
    }
}