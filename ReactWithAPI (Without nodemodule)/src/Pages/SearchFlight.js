import React, { useState, useEffect } from 'react';
import FlightFilters from '../Reusable Components/FlightFilters';
import FlightCard from '../Reusable Components/FlightCard';
import Pagination from './Pagination';
import BookingForm from './BookingForm';
import LoginPage from './LoginPage';

const SearchFlight = () => {
  const [flights, setFlights] = useState([]);
  const [filteredFlights, setFilteredFlights] = useState([]);
  const [loading, setLoading] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [flightType, setFlightType] = useState('International');
  const [filters, setFilters] = useState({
    airline: '',
    maxPrice: 0,
    sortBy: 'price',
    source: '',
    destination: '',
  });
  const [airlines, setAirlines] = useState([]);
  const [showLoginModal, setShowLoginModal] = useState(false);
  const [selectedFlight, setSelectedFlight] = useState(null);
  const [sourceCities, setSourceCities] = useState([]); // List of source cities
  const [destinationCities, setDestinationCities] = useState([]); // List of destination cities based on selected source

  const handleFlightTypeChange = (event) => {
    const selectedFlightType = event.target.value;
    setFlightType(selectedFlightType);
    setFilters({ ...filters, source: '', destination: '' }); // Reset filters when flight type changes
  };

  useEffect(() => {
    const fetchFlightsAndAirlines = async () => {
      setLoading(true);
      try {
        const flightUrl = `http://localhost:5087/api/Flight/DisplayFlightByType?flightType=${flightType}`;
        const flightResponse = await fetch(flightUrl);
        const flightData = await flightResponse.json();
        setFlights(flightData);

        const airlineNames = [...new Set(flightData.map(flight => flight.airlineName))];
        setAirlines(airlineNames);

        // Extract unique source cities
        const uniqueSourceCities = [...new Set(flightData.map(flight => flight.source))];
        setSourceCities(uniqueSourceCities);

        setTotalPages(Math.ceil(flightData.length / 10));
      } catch (error) {
        console.error('Error fetching flights:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchFlightsAndAirlines();
  }, [flightType]);

  useEffect(() => {
    let updatedFlights = [...flights];
  
    if (filters.airline) {
      updatedFlights = updatedFlights.filter(flight => flight.airlineName === filters.airline);
    }
    if (filters.maxPrice > 0) {
      updatedFlights = updatedFlights.filter(flight => flight.ticketPrice <= filters.maxPrice);
    }
    if (filters.source) {
      updatedFlights = updatedFlights.filter(flight => flight.source === filters.source);
    }
    if (filters.destination) {
      updatedFlights = updatedFlights.filter(flight => flight.destination === filters.destination);
    }
  
    if (filters.sortBy === 'price') {
      updatedFlights.sort((a, b) => a.ticketPrice - b.ticketPrice);
    } else if (filters.sortBy === 'duration') {
      updatedFlights.sort((a, b) => a.duration - b.duration);
    }
  
    // **Recalculate total pages based on filtered flights**
    const newTotalPages = Math.ceil(updatedFlights.length / 10);
    setTotalPages(newTotalPages);
  
    // **If the current page is out of range, reset to page 1**
    if (currentPage > newTotalPages) {
      setCurrentPage(1);
    }
  
    // **Apply pagination only on filtered flights**
    setFilteredFlights(updatedFlights.slice((currentPage - 1) * 10, currentPage * 10));
  }, [filters, flights, currentPage]);
  
  
  const handleFilterChange = (type, value) => {
    setFilters((prevFilters) => ({
      ...prevFilters,
      [type]: value,
    }));
  };

  const handleSortChange = (sortBy) => {
    setFilters((prevFilters) => ({
      ...prevFilters,
      sortBy: sortBy,
    }));
  };

  const handleFavoriteToggle = (flightId) => {
    console.log('Toggled favorite for flight:', flightId);
  };

  const isLoggedIn = localStorage.getItem('userToken');

  const handleBookClick = (flight) => {
    if (isLoggedIn) {
      setSelectedFlight(flight);
    } else {
      setShowLoginModal(true);
    }
  };

  // Update destination cities based on selected source
  useEffect(() => {
    if (filters.source) {
      const destinations = flights
        .filter(flight => flight.source === filters.source)
        .map(flight => flight.destination);
      const uniqueDestinations = [...new Set(destinations)];
      setDestinationCities(uniqueDestinations);
    } else {
      setDestinationCities([]);
    }
  }, [filters.source, flights]);

  return (
    <div className="container background">
      <h1 className="text-center text-primary mb-4">✈️ Flight Booking</h1>
      <div style={{
        backdropFilter: "blur(10px)", // Blurs the background
        backgroundColor: "rgba(255, 255, 0, 0.49)", // Adds transparency
        padding: "10px", // Adds spacing inside the container
        borderRadius: "10px", // Rounds the corners
        width: "45%", // Adjust width
        margin: "auto", // Centers the div
        // textAlign: "center", // Centers text inside
      }}>
        <div className="mb-4">
          <h3 htmlFor="flightType" className="form-label mt-3 text-dark " >Select Flight Type</h3>
          <select
            id="flightType"
            className="form-select bg-info text-succsess mt-4 mb-3"
            value={flightType}
            onChange={handleFlightTypeChange}
            style={{
              width: "300px",
              margin: "auto",
            }}
          >
            <option value="international">International</option>
            <option value="domestic">Domestic</option>
          </select>


          {(flightType === 'domestic' || flightType === 'international') && (
            <div className="row mb-4">
              <div className="col-md-6">
                <label htmlFor="source" className="form-label text-dark bg-warning px-1 rounded-pill">From</label>
                <select
                  id="source"
                  className="form-select"
                  value={filters.source}
                  onChange={(e) => handleFilterChange('source', e.target.value)}
                >
                  <option value="">Select Source</option>
                  {sourceCities.map((city, index) => (
                    <option key={index} value={city}>{city}</option>
                  ))}
                </select>
              </div>
              <div className="col-md-6">
                <label htmlFor="destination" className="form-label text-dark bg-warning px-2 rounded-pill">To</label>
                <select
                  id="destination"
                  className="form-select"
                  value={filters.destination}
                  onChange={(e) => handleFilterChange('destination', e.target.value)}
                  disabled={!filters.source}
                >
                  <option value="">Select Destination</option>
                  {destinationCities.map((city, index) => (
                    <option key={index} value={city}>{city}</option>
                  ))}
                </select>
              </div>
            </div>
          )}
        </div>
      </div>
      {flightType && (
        <>
          <FlightFilters
            onFilterChange={handleFilterChange}
            onSortChange={handleSortChange}
            filters={filters}
            airlines={airlines}
          />

          {loading ? (
            <div>Loading...</div>
          ) : (
            <div className="row">
              {filteredFlights.map((flight, index) => (
                <FlightCard
                  key={index}
                  flight={flight}
                  onFavoriteToggle={handleFavoriteToggle}
                  onBookClick={() => handleBookClick(flight)}
                />
              ))}
            </div>
          )}

          <Pagination
            currentPage={currentPage}
            totalPages={totalPages}
            onPageChange={setCurrentPage}
          />
        </>
      )}

      {selectedFlight && (
        <BookingForm
          flight={selectedFlight}
          onClose={() => setSelectedFlight(null)}
        />
      )}

      {showLoginModal && (
        <LoginPage setShowLogin={setShowLoginModal} />
      )}
    </div>
  );
};

export default SearchFlight;
