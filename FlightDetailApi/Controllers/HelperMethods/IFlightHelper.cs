using FlightDetailApi.DTO;
using FlightDetailApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightDetailApi.Controllers.HelperMethods
{
    public interface IFlightHelper
    {
         Task UpdateFlightAsync(FlightInputDTO flightInput);
        Task AddFlightAsync(FlightInputDTO flightInput);
    Task<bool> FlightExists(string flightId);
    Task AddInternationalFlightAsync(InternationalFlightDetails flight);
    Task AddDomesticFlightAsync(DomesticFlightDetails flight);
        Task<object> GetFlightByIdAsync(string flightId);
        Task<List<string>> GetFlightNumbersByType(string flightType);
        Task<List<FlightOutputDTO>> GetFlightsBySourceAndDestination(FlightSearchInput searchInput);
        Task<List<FlightOutputDTO>> GetFlightsByTypeAsync(string flightType);
        Task DeleteFlightAsync(string flightId);
    }
}