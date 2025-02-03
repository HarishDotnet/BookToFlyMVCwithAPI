using FlightDetailApi.DTO;
using FlightDetailApi.Models;
using FlightDetailApi.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FlightDetailApi.Repositories;

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

        // Adds a new international flight to the database
        public async Task AddInternationalFlightAsync(InternationalFlightDetails flight)
        {
            _context.InternationalFlightDetails.Add(flight);
            await _context.SaveChangesAsync();
        }

        // Adds a new domestic flight to the database
        public async Task AddDomesticFlightAsync(DomesticFlightDetails flight)
        {
            _context.DomesticFlightDetails.Add(flight);
            await _context.SaveChangesAsync();
        }

        // Checks if a flight exists based on its FlightId
        public async Task<bool> FlightExists(string flightId)
        {
            return await _context.InternationalFlightDetails.AnyAsync(f => f.FlightId == flightId) ||
                   await _context.DomesticFlightDetails.AnyAsync(f => f.FlightId == flightId);
        }

        // Adds a flight based on the given FlightInputDTO, determining if it's international or domestic
        public async Task AddFlightAsync(FlightInputDTO flightInput)
        {
            if (flightInput.FlightId.StartsWith("IF")) // International Flight
            {
                flightInput.FlightType = "International";
                var flight = _mapper.Map<InternationalFlightDetails>(flightInput);
                _context.InternationalFlightDetails.Add(flight);
            }
            else if (flightInput.FlightId.StartsWith("DF")) // Domestic Flight
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

        // Retrieves a flight by its ID, determining if it's international or domestic
        public async Task<object> GetFlightByIdAsync(string flightId)
        {
            if (flightId.StartsWith("IF"))
                return await _context.InternationalFlightDetails.FirstOrDefaultAsync(f => f.FlightId == flightId);
            else if (flightId.StartsWith("DF"))
                return await _context.DomesticFlightDetails.FirstOrDefaultAsync(f => f.FlightId == flightId);
            return null;
        }

        // Gets a list of flight numbers by type (International or Domestic)
        public async Task<List<string>> GetFlightNumbersByType(string flightType)
        {
            if (flightType.Equals("International", StringComparison.OrdinalIgnoreCase))
            {
                // Select only FlightId from InternationalFlightDetails
                return await _context.InternationalFlightDetails.Select(f => f.FlightId).ToListAsync();
            }
            else if (flightType.Equals("Domestic", StringComparison.OrdinalIgnoreCase))
            {
                // Select only FlightId from DomesticFlightDetails
                return await _context.DomesticFlightDetails.Select(f => f.FlightId).ToListAsync();
            }
            return new List<string>();
        }

        // Retrieves flights based on source and destination with a specified type
        public async Task<List<FlightOutputDTO>> GetFlightsBySourceAndDestination(FlightSearchInput searchInput)
        {
            var allFlights = new List<FlightOutputDTO>();

            if (searchInput.FlightType.Equals("International", StringComparison.OrdinalIgnoreCase))
            {
                // Filter international flights by source and destination
                var flights = await _context.InternationalFlightDetails
                    .Where(f => f.Source.ToLower() == searchInput.Source.ToLower() &&
                                f.Destination.ToLower() == searchInput.Destination.ToLower())
                    .ToListAsync();
                allFlights.AddRange(_mapper.Map<List<FlightOutputDTO>>(flights));
            }
            else if (searchInput.FlightType.Equals("Domestic", StringComparison.OrdinalIgnoreCase))
            {
                // Filter domestic flights by source and destination
                var flights = await _context.DomesticFlightDetails
                    .Where(f => f.Source.ToLower() == searchInput.Source.ToLower() &&
                                f.Destination.ToLower() == searchInput.Destination.ToLower())
                    .ToListAsync();
                allFlights.AddRange(_mapper.Map<List<FlightOutputDTO>>(flights));
            }

            return allFlights;
        }

        // Retrieves all flights based on the specified type (International or Domestic)
        public async Task<List<FlightOutputDTO>> GetFlightsByTypeAsync(string flightType)
        {
            if (flightType.Equals("International", StringComparison.OrdinalIgnoreCase))
            {
                // Retrieve all international flights
                var flights = await _context.InternationalFlightDetails.ToListAsync();
                return _mapper.Map<List<FlightOutputDTO>>(flights);
            }
            else if (flightType.Equals("Domestic", StringComparison.OrdinalIgnoreCase))
            {
                // Retrieve all domestic flights
                var flights = await _context.DomesticFlightDetails.ToListAsync();
                return _mapper.Map<List<FlightOutputDTO>>(flights);
            }
            return new List<FlightOutputDTO>();
        }

        // Deletes a flight based on its ID
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

        // Updates an existing flight's details
        public async Task UpdateFlightAsync(FlightInputDTO flightInput)
        {
            var flight = await GetFlightByIdAsync(flightInput.FlightId);
            if (flight == null)
            {
                throw new KeyNotFoundException($"Flight with ID {flightInput.FlightId} not found.");
            }

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

            await _context.SaveChangesAsync();
        }
        public async Task<bool> DecrementAvailableSeats(string flightId)
        {
            if (flightId.StartsWith("IF"))  // International Flight
            {
                var flight = await _context.InternationalFlightDetails
                                           .FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight == null)
                {
                    return false;  // Flight not found
                }

                if (flight.AvailableSeats > 0)
                {
                    flight.AvailableSeats--;  // Decrement the available seats
                    _context.InternationalFlightDetails.Update(flight);
                    return true;
                }
                return false;  // No seats available
            }
            else if (flightId.StartsWith("DF"))  // Domestic Flight
            {
                var flight = await _context.DomesticFlightDetails
                                           .FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight == null)
                {
                    return false;  // Flight not found
                }

                if (flight.AvailableSeats > 0)
                {
                    flight.AvailableSeats--;  // Decrement the available seats
                    _context.DomesticFlightDetails.Update(flight);
                    return true;
                }
                return false;  // No seats available
            }

            return false;  // Invalid flight type
        }

    }
}
