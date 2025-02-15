using FlightDetailApi.Data;
using FlightDetailApi.Models;
using FlightDetailApi.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightDetailApi.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        // Specific repository for AdminModel
        public IGenericRepository<AdminModel> Admins => GetRepository<AdminModel>();

        // Generic repository access
        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return (IGenericRepository<T>)_repositories[typeof(T)];
            }

            var repository = new GenericRepository<T>(_context);
            _repositories.Add(typeof(T), repository);
            return repository;
        }

        // Save changes asynchronously
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Dispose the context
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}