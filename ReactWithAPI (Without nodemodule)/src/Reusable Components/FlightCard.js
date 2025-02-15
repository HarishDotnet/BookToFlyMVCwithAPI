import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import FlightDetailModal from './FlightDetailModal';


const FlightCard = ({ flight }) => {
  const [showModal, setShowModal] = useState(false);
  const navigate = useNavigate();

  // Check if the user is logged in (assuming token is saved in localStorage)
  const isLoggedIn = localStorage.getItem('userToken');  // Update this to match your actual storage logic

  const handleBookClick = (e) => {
    if (isLoggedIn) {
      e.preventDefault();
      // Redirect to the booking form page with the flight data in the state
      navigate(`/BookingForm/${flight.flightId}`, { state: { flight } });
    } else {
       // Show login form instead of flight details
       e.currentTarget.setAttribute("href", "/");
    }
  };
  return (
    <div className="col-md-6 mb-4">
      <div className="card shadow-lg border-0">
        <div className="card-body">
          <h5 className="card-title text-primary">{flight.airlineName}</h5>
          <p className="card-text text-secondary">
            <strong>{flight.source}</strong> to <strong>{flight.destination}</strong>
          </p>
          <button
            className="btn btn-info"
            onClick={() => setShowModal(true)}
          >
            View Details
          </button>
          <a href="/" onClick={handleBookClick}  className="btn btn-success ms-2">
          Book Flight
                </a>
        </div>
      </div>
      <FlightDetailModal
        flight={flight}
        show={showModal}
        onHide={() => setShowModal(false)}
      />
    </div>
  );
};

export default FlightCard;
