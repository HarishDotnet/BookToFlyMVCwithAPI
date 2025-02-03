import React, { useState } from 'react';
import { useAuth } from '../Context/AuthContext'; // Import authentication context
import { useNavigate } from 'react-router-dom';
import UserRegistration from '../Pages/Register';

export default function Navbar() {
  const { isLoggedIn, logout } = useAuth();
  const [showModal, setShowModal] = useState(false);
  const navigate = useNavigate();

  const handleRegisterClick = (e) => {
    e.preventDefault();
    setShowModal(true); // Show the modal when the "Register" link is clicked
  };

  const handleCloseModal = () => {
    setShowModal(false); // Close the modal
  };

  const handleLogout = () => {
    logout();
    localStorage.removeItem('userToken');
    alert("Logged out successfully!");
    navigate("/");  // Redirect to home page or login page
  };

  return (
    <div>
      <header className="bg-primary text-white py-3">
        <div className="container d-flex justify-content-between align-items-center">
          <h1 className="mb-0 text-warning">Book To Fly</h1>
          <nav>
            {!isLoggedIn ? (
              <>
                <a href="/" className="text-white mx-3 text-decoration-none">
                  Login
                </a>
                <a href="/register" className="text-white mx-3 text-decoration-none" onClick={handleRegisterClick}>
                  Register
                </a>
              </>
            ) : (
              <button className="btn btn-danger mx-3" onClick={handleLogout}>
                Logout
              </button>
            )}
          </nav>
        </div>
      </header>

      {/* Modal for User Registration */}
      <div
        className={`modal ${showModal ? 'show' : ''}`}
        tabIndex="-1"
        style={{ display: showModal ? 'block' : 'none', marginTop: 50 }}
        aria-labelledby="registerModalLabel"
        aria-hidden={!showModal}
      >
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h4 className="modal-title text-success" id="registerModalLabel">
                User Registration
              </h4>
              <button type="button" className="btn-close" onClick={handleCloseModal} aria-label="Close"></button>
            </div>
            <div className="modal-body">
              <UserRegistration />
            </div>
          </div>
        </div>
      </div>

      {/* Overlay when modal is open */}
      {showModal && <div className="modal-backdrop fade show" onClick={handleCloseModal}></div>}
    </div>
  );
}