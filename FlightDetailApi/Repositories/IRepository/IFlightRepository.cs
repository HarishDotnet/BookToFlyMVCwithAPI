using FlightDetailApi.DTO;
using FlightDetailApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightDetailApi.Repositories.IRepository
{
    public interface IFlightRepository
    {
        Task AddFlightAsync(FlightInputDTO flightInput);
        Task UpdateFlightAsync(FlightInputDTO flightInput);
        Task<bool> FlightExists(string flightId);
        Task<List<string>> GetFlightNumbersByType(string flightType);
        Task<List<FlightOutputDTO>> GetFlightsBySourceAndDestination(FlightSearchInput searchInput);
        Task<List<FlightOutputDTO>> GetFlightsByTypeAsync(string flightType);
        Task<object> GetFlightByIdAsync(string flightId);
        Task DeleteFlightAsync(string flightId);
        Task<bool> DecrementAvailableSeats(string flightId);
    }
}