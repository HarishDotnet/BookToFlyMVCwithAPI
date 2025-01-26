using FlightDetailApi.DTO;
using FlightDetailApi.Models;
using FlightDetailApi.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace FlightDetailApi.Helpers
{
    public class FlightHelper
    {
        public readonly ApplicationDbContext _context;
        public readonly IMapper _mapper;

        public FlightHelper(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> FlightExists(string flightId)
        {
            return await _context.InternationalFlightDetails.AnyAsync(f => f.FlightId == flightId) ||
                   await _context.DomesticFlightDetails.AnyAsync(f => f.FlightId == flightId);
        }

        public async Task<object> GetFlightById(string flightId)
        {
            if (flightId.StartsWith("IF"))
            {
                return await _context.InternationalFlightDetails.FirstOrDefaultAsync(f => f.FlightId == flightId);
            }
            else if (flightId.StartsWith("DF"))
            {
                return await _context.DomesticFlightDetails.FirstOrDefaultAsync(f => f.FlightId == flightId);
            }
            return null;
        }


        public async Task<List<string>> GetFlightNumbersByType(string flightType)
        {
            if (flightType.Equals("International", StringComparison.OrdinalIgnoreCase))
            {
                return await _context.InternationalFlightDetails.Select(f => f.FlightId).ToListAsync();
            }
            else if (flightType.Equals("Domestic", StringComparison.OrdinalIgnoreCase))
            {
                return await _context.DomesticFlightDetails.Select(f => f.FlightId).ToListAsync();
            }
            return new List<string>();
        }

        public async Task<List<FlightOutputDTO>> GetFlightsByType(string flightType)
        {
            if (flightType.Equals("International", StringComparison.OrdinalIgnoreCase))
            {
                var flights = await _context.InternationalFlightDetails.ToListAsync();
                return _mapper.Map<List<FlightOutputDTO>>(flights);
            }
            else if (flightType.Equals("Domestic", StringComparison.OrdinalIgnoreCase))
            {
                var flights = await _context.DomesticFlightDetails.ToListAsync();
                return _mapper.Map<List<FlightOutputDTO>>(flights);
            }
            return null;
        }

        public async Task<List<FlightOutputDTO>> GetFlightsBySourceAndDestination(FlightSearchInput searchInput)
        {
            var allFlights = new List<FlightOutputDTO>();

            if (searchInput.FlightType.Equals("International", StringComparison.OrdinalIgnoreCase))
            {
                var flights = await _context.InternationalFlightDetails
                    .Where(f => f.Source.ToLower() == searchInput.Source.ToLower() &&
                                f.Destination.ToLower() == searchInput.Destination.ToLower())
                    .ToListAsync();
                allFlights.AddRange(_mapper.Map<List<FlightOutputDTO>>(flights));
            }
            else if (searchInput.FlightType.Equals("Domestic", StringComparison.OrdinalIgnoreCase))
            {
                var flights = await _context.DomesticFlightDetails
                    .Where(f => f.Source.ToLower() == searchInput.Source.ToLower() &&
                                f.Destination.ToLower() == searchInput.Destination.ToLower())
                    .ToListAsync();
                allFlights.AddRange(_mapper.Map<List<FlightOutputDTO>>(flights));
            }

            return allFlights;
        }
    }
}
