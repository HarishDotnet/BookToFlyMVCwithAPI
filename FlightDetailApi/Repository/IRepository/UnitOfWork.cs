using FlightDetailApi.Data;
using FlightDetailApi.Models;

namespace FlightDetailApi.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContextMVC _context;
        private IGenericRepository<AdminModel> _admins;

        public UnitOfWork(ApplicationDbContextMVC context)
        {
            _context = context;
        }

        public IGenericRepository<AdminModel> Admins => _admins ??= new GenericRepository<AdminModel>(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
