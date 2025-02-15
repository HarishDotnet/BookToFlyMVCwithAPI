import React, { useEffect, useState } from 'react';
import { useLocation, useNavigate } from "react-router-dom";
import { Link } from 'react-router-dom';
import { Button } from 'react-bootstrap';

const TicketList = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const [bookings, setBookings] = useState([]);
  const user = localStorage.getItem("username");

  useEffect(() => {
    if (location.state?.bookings) {
      const formattedBookings = location.state.bookings.map(booking => {
        console.log("Booking Data: ", booking); // Debug: Check the actual booking object
        return {
          id: booking.bookingId || booking.id,
          flight: booking.flightId || booking.flight,
          from: booking.flight?.source || booking.from,
          to: booking.flight?.destination || booking.to,
          date: booking.bookingDate || booking.date,
          status: booking.status || "Confirmed" // Directly use status from the response
        };
      });
      setBookings(formattedBookings);
      localStorage.setItem("bookings", JSON.stringify(formattedBookings));
    } else {
      const storedBookings = localStorage.getItem("bookings");
      if (storedBookings) {
        setBookings(JSON.parse(storedBookings));
      }
    }
  }, [location.state]);

  const sortedBookings = [...bookings].sort((a, b) => (a.status === "Cancelled" ? 1 : -1));

  return (
    <div className="container mt-4">
      <div className="d-flex justify-content-start mb-3">
        <Button variant="secondary" onClick={() => navigate("/UserDashboard")}>
          &larr; Back
        </Button>
      </div>

      <h2 className="text-primary">Your Ticket List</h2>

      {sortedBookings.length === 0 ? (
        <p style={{
          color:"red",
          fontSize:"40px",
          marginTop: "120px",
        }}
        >No bookings available.</p>
      ) : (
        <div className="mt-4">
          <table className="table table-bordered">
            <thead>
              <tr>
                <th>Booking ID</th>
                <th>Flight Name</th>
                <th>From</th>
                <th>To</th>
                <th>Date</th>
                <th>Status</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {sortedBookings.map((booking) => (
                <tr key={booking.id} className={booking.status === "Cancelled" ? "table-danger" : ""}>
                  <td>{booking.id}</td>
                  <td>{booking.flight}</td>
                  <td>{booking.from}</td>
                  <td>{booking.to}</td>
                  <td>{new Date(booking.date).toLocaleDateString()}</td>
                  <td>{booking.status}</td>
                  <td>
                    {booking.status !== "Cancelled" ? (
                      <Button variant="info" as={Link} to={`/TicketDetails/${booking.id}`}>
                        View Ticket
                      </Button>
                    ) : (
                      <span className="text-danger fw-bold">Ticket Not Available</span>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default TicketList;
