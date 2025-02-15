import React, { useState } from 'react';

const FlightFilters = ({ onFilterChange, onSortChange, airlines }) => {
  const [price, setPrice] = useState(50000); // Default max price

  // Update price when slider is changed
  const handlePriceChange = (e) => {
    const newPrice = e.target.value;
    setPrice(newPrice);
    onFilterChange('maxPrice', newPrice);
  };

  return (
    
    <div className="mb-4"   style={{
      backdropFilter: "blur(10px)", // Blurs the background
      backgroundImage: "linear-gradient(to right, rgba(59, 230, 161, 0.50), rgba(153, 82, 193, 0.7))",
      padding: "20px", // Adds spacing inside the container
      borderRadius: "10px", // Rounds the corners
      width: "80%", // Adjust width
      margin: "auto", // Centers the div
      textAlign: "center", // Centers text inside
    }}>
      <h5>Filter and Sort Flights</h5>
      
      {/* Filter by Airline */}
      <div className="mb-3">
        <label className="form-label text-dark bg-warning px-2 rounded-pill">Filter by Airline:</label>
        <div className='d-flex justify-content-center mx-auto '>
        <select
          className="form-select"
          onChange={(e) => onFilterChange('airline', e.target.value)}
          style={{width: "70%",textAlign: "center"}}
        >
          <option value="">All Airlines</option>
          {airlines.map((airline, index) => (
            <option key={index} value={airline}>{airline}</option>
          ))}
        </select> 
        </div>
        
      </div>

      {/* Filter by Price using range slider */}
      <div className="mb-3">
        <label className="form-label text-light">
          Max Price: <span className='text-light bg-success px-1 rounded-pill'>{`₹${price}`}</span>
        </label>
        
        <input
          type="range"
          className="form-range"
          min="0"
          max="50000"
          step="500"
          value={price}
          onChange={handlePriceChange}
          style={{ width: '50%' }}
        />
      </div>

      {/* Sort by Price or Duration */}
      <div className="mb-3">
        <label className="form-label text-dark bg-warning px-2 rounded-pill">Sort By:</label>
        <select
          className="form-select text-center"
          onChange={(e) => onSortChange(e.target.value)}

        >
          <option value="price">Price</option>
          <option value="duration">Duration</option>
        </select>
      </div>
    </div>
  );
};

export default FlightFilters;
