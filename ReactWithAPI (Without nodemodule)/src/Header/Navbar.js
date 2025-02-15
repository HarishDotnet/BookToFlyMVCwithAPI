import React, { useState } from 'react';
import { useAuth } from '../Context/AuthContext'; // Import authentication context
import { useNavigate } from 'react-router-dom';
import UserRegistration from '../Pages/Register';
import 'bootstrap-icons/font/bootstrap-icons.css';
export default function Navbar() {
  const { isLoggedIn, logout } = useAuth();
  const [showModal, setShowModal] = useState(false);
  const navigate = useNavigate();

  const handleCloseModal = () => {
    setShowModal(false); // Close the modal
  };

  const handleLogout = () => {
    logout();
    localStorage.removeItem('userToken');
    alert("Logged out successfully!");
    navigate("/");  // Redirect to home page or login page
  };
  const handleclick=()=>{
    if(isLoggedIn){
      navigate("/UserDashboard");
    }
    else{
      navigate("/");
    }
  }
  return (
    <div>
      <header className="bg-primary text-white py-3 rounded-pill mt-1" style={{ background: "linear-gradient(1deg,rgb(0, 132, 255),rgb(178, 211, 225))"}}>
        <div className="container d-flex justify-content-between align-items-center">
          <h1><a href="" className="mb-0 text-muted" onClick={handleclick}><i class="bi bi-feather"></i>Book To Fly</a></h1>
          <nav>
            {!isLoggedIn ? (
              <>
                <a href="/" className="text-dark mx-3 ">
                  Login<i class="bi bi-door-open"></i>
                </a>
                {/* <a href="/register" className="text-white mx-3 text-decoration-none" onClick={handleRegisterClick}>
                  Register
                </a> */}
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