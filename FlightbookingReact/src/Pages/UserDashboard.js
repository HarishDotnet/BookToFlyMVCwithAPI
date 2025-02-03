import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { User, Plane, X } from "lucide-react";
import SearchFlight from "./SearchFlight";
import ProfileCard from "./ProfileCard"; // Import the ProfileCard component
import "../CSS/Bookings.css";

export default function UserDashboard() {
  const navigate = useNavigate();
  const [bookings, setBookings] = useState([]);
  const [searchFlight, setSearchFlight] = useState(false);
  const [selectedBooking, setSelectedBooking] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [showProfile, setShowProfile] = useState(false); // State to toggle ProfileCard
  const username = localStorage.getItem("username");

  // Fetch user tickets when the component mounts or the username changes
  useEffect(() => {
    if (username) {
      fetchUserTickets(username);
    }
  }, [username]);

  // Fetch tickets for the logged-in user
  const fetchUserTickets = async (username) => {
    try {
      const response = await fetch(`http://localhost:5087/api/Ticket/${username}`);
      const data = await response.json();

      if (response.ok && data.ticketList?.length > 0) {
        fetchBookings(data.ticketList); // Fetch bookings for each ticket
      } else {
        setBookings([]); // No tickets found
      }
    } catch (error) {
      console.error("Fetch error:", error);
    }
  };

  // Fetch bookings for each ticket
  const fetchBookings = async (ticketList) => {
    try {
      const bookingPromises = ticketList.map(async (ticketId) => {
        const response = await fetch(`http://localhost:5087/api/Booking/${ticketId}`);
        return response.ok ? response.json() : null;
      });

      const bookingResults = await Promise.all(bookingPromises);
      const validBookings = bookingResults.filter((b) => b !== null);
      setBookings(validBookings); // Set the valid bookings in state
    } catch (error) {
      console.error("Error fetching bookings:", error);
    }
  };

  // Toggle the search flight component
  const handleSearchFlights = () => {
    setSearchFlight(!searchFlight);
    setShowProfile(false); // Hide ProfileCard when searching flights
  };

  // Toggle the ProfileCard component
  const handleProfileClick = () => {
    setShowProfile(!showProfile);
    setSearchFlight(false); // Hide SearchFlight when showing ProfileCard
  };

  // Navigate to the TicketList page with serialized bookings data
  const navigateToViewAllBookings = () => {
    const serializedBookings = bookings.map((booking) => ({
      id: booking.booking.bookingId, // Unique ID for each booking
      flight: booking.flight?.flightId, // Flight ID
      from: booking.flight?.source, // Source airport
      to: booking.flight?.destination, // Destination airport
      date: booking.booking?.bookingDate, // Booking date
      status: booking.booking.isCanceled ? "Cancelled" : "Confirmed", // Correct status check
    }));

    navigate("/TicketList", { state: { bookings: serializedBookings } });
  };

  // Handle click on a booking to show details in a modal
  const handleBookingClick = (booking) => {
    setSelectedBooking(booking);
    setShowModal(true);
  };

  return (
    <div className="container mt-5">
      <div className="d-flex justify-content-around mt-4">
        <button className="btn btn-success "  style={{ height: "50px", padding: "4px 10px" }} onClick={handleProfileClick}>
          <User className="text-warning" /> Profile
        </button>
       <p className="text-danger"><p className="text-warning">Note:</p>Tap (Profile or Book) again to head back to the User Dashboard.</p>
        <button className="btn btn-success" style={{ height: "50px", padding: "4px 10px" }} onClick={handleSearchFlights}>
          <Plane className="text-warning" /> Book Ticket
        </button>
      </div>

      {/* Show ProfileCard if showProfile is true */}
      {showProfile && (
        <div className="row mt-5 d-flex justify-content-center">
          <div className="col-md-5">
            <ProfileCard />
          </div>
        </div>
      )}

      {/* Show SearchFlight if searchFlight is true */}
      {searchFlight && <SearchFlight />}

      {/* Show Bookings if neither ProfileCard nor SearchFlight is active */}
      {!showProfile && !searchFlight && (
        <div className="row mt-5 d-flex justify-content-center">
          <div className="col-md-5">
            <div
              className="card shadow-lg border-0 rounded-4 p-4"
              style={{
                background: "linear-gradient(135deg, rgba(0,0,0,0.7), rgba(20,20,20,0.9))",
                backdropFilter: "blur(15px)",
                WebkitBackdropFilter: "blur(15px)",
                borderRadius: "16px",
                color: "#fff",
                transition: "transform 0.3s ease-in-out",
                boxShadow: "0 10px 25px rgba(0, 0, 0, 0.3)",
                border: "1px solid rgba(255, 255, 255, 0.2)",
              }}
            >
              <div className="card-body text-center">
                <h5
                  className="card-title fw-bold mb-3 text-uppercase"
                  style={{
                    letterSpacing: "1px",
                    fontSize: "1.3rem",
                    color: "#FFD700",
                    textShadow: "0 2px 10px rgba(255, 215, 0, 0.5)",
                  }}
                >
                  📌 Your Bookings
                </h5>

                <ul className="list-group mt-3" style={{ background: "transparent", padding: 0 }}>
                  {bookings.length > 0 && bookings.some((booking) => !booking.booking.isCanceled) ? (
                    bookings
                      .filter((booking) => !booking.booking.isCanceled) // ✅ Show only confirmed bookings
                      .map((booking, index) => (
                        <li
                          key={`${booking.bookingId}-${index}`}
                          className="list-group-item text-light border-0 mb-2 p-3 rounded-3"
                          style={{
                            background: "rgba(255, 255, 255, 0.15)",
                            backdropFilter: "blur(12px)",
                            WebkitBackdropFilter: "blur(12px)",
                            transition: "transform 0.3s ease-in-out, background 0.3s ease-in-out",
                            cursor: "pointer",
                            borderRadius: "12px",
                            fontSize: "1rem",
                            fontWeight: "500",
                            color: "#fff",
                            textShadow: "0px 1px 2px rgba(255,255,255,0.2)",
                            border: "1px solid rgba(255,255,255,0.2)",
                          }}
                          onMouseOver={(e) => {
                            e.currentTarget.style.transform = "scale(1.05)";
                            e.currentTarget.style.background = "rgba(255, 255, 255, 0.3)";
                          }}
                          onMouseOut={(e) => {
                            e.currentTarget.style.transform = "scale(1)";
                            e.currentTarget.style.background = "rgba(255, 255, 255, 0.15)";
                          }}
                          onClick={() => handleBookingClick(booking)}
                        >
                          ✈️ {booking.flightId} - {booking.flight.source} to {booking.flight.destination} (
                          {new Date(booking.booking.bookingDate).toLocaleDateString()})
                          <br />
                          {booking.flightType && (
                            <small className="text-warning">Flight Type: {booking.flightType}</small>
                          )}
                          <br />
                          Status: Confirmed
                        </li>
                      ))
                  ) : (
                    <p className="text-danger text-center border-success mt-3">No confirmed bookings found</p>
                  )}
                </ul>

                <button
                  className="btn px-4 py-2 rounded-3 mt-3"
                  onClick={navigateToViewAllBookings}
                  style={{
                    fontWeight: "600",
                    letterSpacing: "0.5px",
                    fontSize: "1rem",
                    padding: "10px 20px",
                    borderRadius: "12px",
                    border: "2px solid rgba(255, 255, 255, 0.5)",
                    color: "#fff",
                    background: "rgba(255, 255, 255, 0.1)",
                    transition: "background 0.3s ease-in-out, transform 0.2s",
                    textShadow: "0 1px 2px rgba(255, 255, 255, 0.3)",
                  }}
                  onMouseOver={(e) => {
                    e.currentTarget.style.background = "rgba(255, 255, 255, 0.3)";
                    e.currentTarget.style.transform = "scale(1.05)";
                  }}
                  onMouseOut={(e) => {
                    e.currentTarget.style.background = "rgba(255, 255, 255, 0.1)";
                    e.currentTarget.style.transform = "scale(1)";
                  }}
                >
                  View All 🔍
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Modal for booking details */}
      {showModal && selectedBooking && (
        <div className="modal-overlay">
          <div className="modal-content">
            <button className="close-btn" onClick={() => setShowModal(false)}>
              <X size={24} />
            </button>
            <h3 className="modal-title">✈️ Booking Details</h3>
            <p>
              <strong>Flight:</strong> {selectedBooking.booking.flightId}
            </p>
            <p>
              <strong>From:</strong> {selectedBooking.flight.source}
            </p>
            <p>
              <strong>To:</strong> {selectedBooking.flight.destination}
            </p>
            <p>
              <strong>Date:</strong> {selectedBooking.booking.dateOfTravel}
            </p>
          </div>
        </div>
      )}
    </div>
  );
}