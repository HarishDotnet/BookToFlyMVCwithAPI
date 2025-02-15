using System;
using System.Threading.Tasks;
using FlightDetailApi.Models;

namespace FlightDetailApi.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        // Specific repository for AdminModel
        IGenericRepository<AdminModel> Admins { get; }

        // Generic repository access
        IGenericRepository<T> GetRepository<T>() where T : class;

        // Save changes asynchronously
        Task<int> CompleteAsync();
    }
}