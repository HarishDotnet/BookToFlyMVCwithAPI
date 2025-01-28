using System.Threading.Tasks;
using FlightDetailApi.Models;

namespace FlightDetailApi.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<AdminModel> Admins { get; }
        Task<int> CompleteAsync();
    }
}
