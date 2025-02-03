import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import FlightDetailModal from './FlightDetailModal';
import LoginPage from './LoginPage';  // Import the login component


const FlightCard = ({ flight, onFavoriteToggle }) => {
  const [showModal, setShowModal] = useState(false);
  const [isFavorite, setIsFavorite] = useState(false);
  const [showLoginModal, setShowLoginModal] = useState(false);  // State for login modal visibility
  const navigate = useNavigate();

  // Check if the user is logged in (assuming token is saved in localStorage)
  const isLoggedIn = localStorage.getItem('userToken');  // Update this to match your actual storage logic

  const handleFavoriteToggle = () => {
    setIsFavorite(!isFavorite);
    onFavoriteToggle(flight.flightId);
  };

  const handleBookClick = () => {
    if (isLoggedIn) {
      console.log("Flight data:", flight);
      // Redirect to the booking form page with the flight data in the state
      navigate(`/BookingForm/${flight.flightId}`, { state: { flight } });
    } else {
      // Show login modal if the user is not logged in
      setShowLoginModal(true);
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
          <button
            className="btn btn-success ms-2"
            onClick={handleBookClick}
          >
            Book Flight
          </button>
        </div>
      </div>
      <FlightDetailModal
        flight={flight}
        show={showModal}
        onHide={() => setShowModal(false)}
      />
      
      {/* Login Modal - only shown when the user is not logged in */}
      {showLoginModal && (
        <div className="modal show d-block" tabIndex="-1">
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Login</h5>
                <button
                  type="button"
                  className="btn-close"
                  onClick={() => setShowLoginModal(false)}
                  aria-label="Close"
                ></button>
              </div>
              <div className="modal-body">
                <LoginPage setShowLogin={setShowLoginModal} />
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default FlightCard;
