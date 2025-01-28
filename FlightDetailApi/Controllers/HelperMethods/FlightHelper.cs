using FlightDetailApi.DTO;
using FlightDetailApi.Models;
using FlightDetailApi.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace FlightDetailApi.Controllers.HelperMethods
{
    public class FlightHelper : IFlightHelper
    {
        public readonly ApplicationDbContext _context;
        public readonly IMapper _mapper;

        public FlightHelper(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

    public async Task AddInternationalFlightAsync(InternationalFlightDetails flight)
    {
        _context.InternationalFlightDetails.Add(flight);
        await _context.SaveChangesAsync();
    }

    public async Task AddDomesticFlightAsync(DomesticFlightDetails flight)
    {
        _context.DomesticFlightDetails.Add(flight);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> FlightExists(string flightId)
    {
        return await _context.InternationalFlightDetails.AnyAsync(f => f.FlightId == flightId) ||
               await _context.DomesticFlightDetails.AnyAsync(f => f.FlightId == flightId);
    }


        public async Task AddFlightAsync(FlightInputDTO flightInput)
        {
            if (flightInput.FlightId.StartsWith("IF"))
            {
                flightInput.FlightType = "International";
                var flight = _mapper.Map<InternationalFlightDetails>(flightInput);
                _context.InternationalFlightDetails.Add(flight);
            }
            else if (flightInput.FlightId.StartsWith("DF"))
            {
                flightInput.FlightType = "Domestic";
                var flight = _mapper.Map<DomesticFlightDetails>(flightInput);
                _context.DomesticFlightDetails.Add(flight);
            }
            else
            {
                throw new ArgumentException("Invalid flight type.");
            }

            await _context.SaveChangesAsync();
        }

        public async Task<object> GetFlightByIdAsync(string flightId)
        {
            if (flightId.StartsWith("IF"))
                return await _context.InternationalFlightDetails.FirstOrDefaultAsync(f => f.FlightId == flightId);
            else if (flightId.StartsWith("DF"))
                return await _context.DomesticFlightDetails.FirstOrDefaultAsync(f => f.FlightId == flightId);
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

        public async Task<List<FlightOutputDTO>> GetFlightsByTypeAsync(string flightType)
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
            return new List<FlightOutputDTO>();
        }

        public async Task DeleteFlightAsync(string flightId)
        {
            if (flightId.StartsWith("IF"))
            {
                var flight = await _context.InternationalFlightDetails.FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight != null) _context.InternationalFlightDetails.Remove(flight);
            }
            else if (flightId.StartsWith("DF"))
            {
                var flight = await _context.DomesticFlightDetails.FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight != null) _context.DomesticFlightDetails.Remove(flight);
            }
            await _context.SaveChangesAsync();
        }

    public async Task UpdateFlightAsync(FlightInputDTO flightInput)
    {
        // Retrieve the flight from the database
        var flight = await GetFlightByIdAsync(flightInput.FlightId);
        if (flight == null)
        {
            throw new KeyNotFoundException($"Flight with ID {flightInput.FlightId} not found.");
        }

        // Map DTO to the retrieved entity
        if (flight is InternationalFlightDetails internationalFlight)
        {
            _mapper.Map(flightInput, internationalFlight);
            _context.InternationalFlightDetails.Update(internationalFlight);
        }
        else if (flight is DomesticFlightDetails domesticFlight)
        {
            _mapper.Map(flightInput, domesticFlight);
            _context.DomesticFlightDetails.Update(domesticFlight);
        }
        else
        {
            throw new ArgumentException("Invalid flight type.");
        }

        // Save changes
        await _context.SaveChangesAsync();
    }
    }
}