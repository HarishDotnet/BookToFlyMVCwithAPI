using FlightDetailApi.DTO;
using FlightDetailApi.Models;
using FlightDetailApi.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FlightDetailApi.Repositories;
using FlightDetailApi.Repositories.IRepository;
using BookToFlyMVC.Validations;

namespace FlightDetailApi.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public FlightRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // Adds a new international flight to the database
        public async Task AddInternationalFlightAsync(InternationalFlightDetails flight)
        {
            if (flight == null)
                throw new ArgumentNullException(nameof(flight));

            _context.InternationalFlightDetails.Add(flight);
            await _context.SaveChangesAsync();
        }

        // Adds a new domestic flight to the database
        public async Task AddDomesticFlightAsync(DomesticFlightDetails flight)
        {
            if (flight == null)
                throw new ArgumentNullException(nameof(flight));

            _context.DomesticFlightDetails.Add(flight);
            await _context.SaveChangesAsync();
        }

        // Checks if a flight exists based on its FlightId
        public async Task<bool> FlightExists(string flightId)
        {
            if (string.IsNullOrEmpty(flightId))
                throw new ArgumentException("Flight ID cannot be null or empty.");

            return await _context.InternationalFlightDetails.AnyAsync(f => f.FlightId == flightId) ||
                   await _context.DomesticFlightDetails.AnyAsync(f => f.FlightId == flightId);
        }

        // Adds a flight based on the given FlightInputDTO, determining if it's international or domestic
        public async Task AddFlightAsync(FlightInputDTO flightInput)
        {
            if (flightInput == null)
                throw new ArgumentNullException(nameof(flightInput));

            if (string.IsNullOrEmpty(flightInput.FlightId))
                throw new ArgumentException("Flight ID cannot be null or empty.");

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
                throw new ArgumentException("Invalid flight type. Flight ID must start with 'IF' or 'DF'.");
            }

            await _context.SaveChangesAsync();
        }

        // Retrieves a flight by its ID, determining if it's international or domestic
        public async Task<object> GetFlightByIdAsync(string flightId)
        {
            if (string.IsNullOrEmpty(flightId))
                throw new ArgumentException("Flight ID cannot be null or empty.");

            if (flightId.StartsWith("IF"))
                return await _context.InternationalFlightDetails.FirstOrDefaultAsync(f => f.FlightId == flightId)
                       ?? throw new FlightNotFoundException($"International flight with ID {flightId} not found.");
            else if (flightId.StartsWith("DF"))
                return await _context.DomesticFlightDetails.FirstOrDefaultAsync(f => f.FlightId == flightId)
                       ?? throw new FlightNotFoundException($"Domestic flight with ID {flightId} not found.");

            throw new ArgumentException("Invalid flight type. Flight ID must start with 'IF' or 'DF'.");
        }

        // Gets a list of flight numbers by type (International or Domestic)
        public async Task<List<string>> GetFlightNumbersByType(string flightType)
        {
            if (string.IsNullOrEmpty(flightType))
                throw new ArgumentException("Flight type cannot be null or empty.");

            if (flightType.Equals("International", StringComparison.OrdinalIgnoreCase))
            {
                return await _context.InternationalFlightDetails.Select(f => f.FlightId).ToListAsync();
            }
            else if (flightType.Equals("Domestic", StringComparison.OrdinalIgnoreCase))
            {
                return await _context.DomesticFlightDetails.Select(f => f.FlightId).ToListAsync();
            }

            throw new ArgumentException("Invalid flight type. Must be 'International' or 'Domestic'.");
        }

        // Retrieves flights based on source and destination with a specified type
        public async Task<List<FlightOutputDTO>> GetFlightsBySourceAndDestination(FlightSearchInput searchInput)
        {
            if (searchInput == null)
                throw new ArgumentNullException(nameof(searchInput));

            if (string.IsNullOrEmpty(searchInput.FlightType) || string.IsNullOrEmpty(searchInput.Source) || string.IsNullOrEmpty(searchInput.Destination))
                throw new ArgumentException("Flight type, source, and destination must be provided.");

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

        // Retrieves all flights based on the specified type (International or Domestic)
        public async Task<List<FlightOutputDTO>> GetFlightsByTypeAsync(string flightType)
        {
            if (string.IsNullOrEmpty(flightType))
                throw new ArgumentException("Flight type cannot be null or empty.");

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

            throw new ArgumentException("Invalid flight type. Must be 'International' or 'Domestic'.");
        }

        // Deletes a flight based on its ID
        public async Task DeleteFlightAsync(string flightId)
        {
            if (string.IsNullOrEmpty(flightId))
                throw new ArgumentException("Flight ID cannot be null or empty.");

            if (flightId.StartsWith("IF"))
            {
                var flight = await _context.InternationalFlightDetails.FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight == null)
                    throw new FlightNotFoundException($"International flight with ID {flightId} not found.");

                _context.InternationalFlightDetails.Remove(flight);
            }
            else if (flightId.StartsWith("DF"))
            {
                var flight = await _context.DomesticFlightDetails.FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight == null)
                    throw new FlightNotFoundException($"Domestic flight with ID {flightId} not found.");

                _context.DomesticFlightDetails.Remove(flight);
            }
            else
            {
                throw new ArgumentException("Invalid flight type. Flight ID must start with 'IF' or 'DF'.");
            }

            await _context.SaveChangesAsync();
        }

        // Updates an existing flight's details
        public async Task UpdateFlightAsync(FlightInputDTO flightInput)
        {
            if (flightInput == null)
                throw new ArgumentNullException(nameof(flightInput));

            var flight = await GetFlightByIdAsync(flightInput.FlightId);
            if (flight == null)
                throw new FlightNotFoundException($"Flight with ID {flightInput.FlightId} not found.");

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
                throw new FlightApiException("Invalid flight type.");
            }

            await _context.SaveChangesAsync();
        }
        // Decrements available seats for a flight
        public async Task<bool> DecrementAvailableSeats(string flightId)
        {
            if (string.IsNullOrEmpty(flightId))
                throw new ArgumentException("Flight ID cannot be null or empty.");

            if (flightId.StartsWith("IF")) // International Flight
            {
                var flight = await _context.InternationalFlightDetails
                                           .FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight == null)
                    throw new FlightNotFoundException($"International flight with ID {flightId} not found.");

                if (flight.AvailableSeats > 0)
                {
                    flight.AvailableSeats--;
                    _context.InternationalFlightDetails.Update(flight);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false; // No seats available
            }
            else if (flightId.StartsWith("DF")) // Domestic Flight
            {
                var flight = await _context.DomesticFlightDetails
                                           .FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight == null)
                    throw new FlightNotFoundException($"Domestic flight with ID {flightId} not found.");

                if (flight.AvailableSeats > 0)
                {
                    flight.AvailableSeats--;
                    _context.DomesticFlightDetails.Update(flight);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false; // No seats available
            }

            throw new ArgumentException("Invalid flight type. Flight ID must start with 'IF' or 'DF'.");
        }
    }
}