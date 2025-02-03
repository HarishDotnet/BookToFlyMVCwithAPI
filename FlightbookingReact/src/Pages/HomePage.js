import React, { useState, useEffect } from "react";
import LoginPage from "./LoginPage";
import SearchFlight from "./SearchFlight";
import 'bootstrap/dist/css/bootstrap.min.css';


const FlightBookingApp = () => {
  const [showLogin, setShowLogin] = useState(true);

  useEffect(() => {
    setShowLogin(true);
  }, []);

  return (
    <div className="app-container" style={{ position: "relative" }}>
      <SearchFlight />
      
      {showLogin && (
        <div className="popup-overlay " style={{
          position: "fixed",
          top: 0,
          left: 0,
          width: "100vw",
          height: "100vh",
          backgroundColor: "rgba(0, 0, 0, 0.5)",
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          zIndex: 1000
        }}>
          <div className="popup-content mt-5" style={{
            background: "#fff",
            padding: "20px",
            borderRadius: "10px",
            boxShadow: "0 4px 10px rgba(0, 0, 0, 0.3)",
            position: "relative",
            width: "30rem"
          }}>
            <button 
              className="close-btn bg-danger" 
              onClick={() => setShowLogin(false)}
              style={{
                position: "absolute",
                top: "8px",
                right: "8px",
                background: "transparent",
                border: "none",
                fontSize: "1.0rem",
                cursor: "pointer"
              }}
            >
              &times;
            </button>
            <LoginPage setShowLogin={setShowLogin} /> 
          </div>
        </div>
      )}
    </div>
  );
};

export default FlightBookingApp;
