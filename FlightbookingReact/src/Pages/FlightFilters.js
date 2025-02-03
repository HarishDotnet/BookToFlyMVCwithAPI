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
    <div className="mb-4">
      <h5>Filter and Sort Flights</h5>
      
      {/* Filter by Airline */}
      <div className="mb-3">
        <label className="form-label">Filter by Airline:</label>
        <select
          className="form-select"
          onChange={(e) => onFilterChange('airline', e.target.value)}
        >
          <option value="">All Airlines</option>
          {airlines.map((airline, index) => (
            <option key={index} value={airline}>{airline}</option>
          ))}
        </select>
      </div>

      {/* Filter by Price using range slider */}
      <div className="mb-3">
        <label className="form-label">
          Max Price: <span>{`₹${price}`}</span>
        </label>
        
        <input
          type="range"
          className="form-range"
          min="0"
          max="50000"
          step="500"
          value={price}
          onChange={handlePriceChange}
          style={{ width: '70%' }}
        />
        <div className="d-flex justify-content-between">
          <span>₹0</span>
          <span>₹50,000</span>
        </div>
      </div>

      {/* Sort by Price or Duration */}
      <div className="mb-3">
        <label className="form-label">Sort By:</label>
        <select
          className="form-select"
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
