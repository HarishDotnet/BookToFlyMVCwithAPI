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
            var admin = await _unitOfWork.Admins.FindAsync(a => a.Username == username);

            if (admin == null || admin.Password != password)
                return null;

            return admin;
        }

        public async Task<bool> RegisterAsync(AdminModel admin)
        {
            var existingAdmin = await _unitOfWork.Admins.FindAsync(a => a.Username == admin.Username);
            if (existingAdmin != null)
                return false;
           
            await _unitOfWork.Admins.AddAsync(admin);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
