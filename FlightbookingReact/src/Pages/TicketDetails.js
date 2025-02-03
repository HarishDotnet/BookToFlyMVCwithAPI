import { useState, useEffect, useRef } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { X, ArrowLeft } from "lucide-react";
import html2canvas from "html2canvas";

export default function TicketDetails() {
  const [ticketData, setTicketData] = useState(null);
  const [flightData, setFlightData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showCancelModal, setShowCancelModal] = useState(false);

  const location = useLocation();
  const navigate = useNavigate();
  const pathParts = location.pathname.split("/");
  const bookingId = pathParts[pathParts.length - 1];
  const ticketContainerRef = useRef();

  useEffect(() => {
    const fetchTicketData = async () => {
      try {
        const response = await fetch(`http://localhost:5087/api/Booking/${bookingId}`);
        if (!response.ok) throw new Error("Failed to fetch ticket details");
        const data = await response.json();
        setTicketData(data.booking);
        setFlightData(data.flight);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    if (bookingId) {
      fetchTicketData();
    }
  }, [bookingId]);

  const handleCancelBooking = () => setShowCancelModal(true);

  const confirmCancel = async () => {
    try {
      const response = await fetch(`http://localhost:5087/api/Booking/${bookingId}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" }
      });
  
      if (!response.ok) {
        throw new Error("Failed to cancel booking");
      }
  
      alert("Your booking has been canceled.");
      navigate("/TicketList"); // Redirect to ticket list after cancellation
    } catch (error) {
      alert("Error canceling the booking. Please try again.");
    }
  };
  
  const cancelCancel = () => setShowCancelModal(false);

  const downloadTicketAsImage = () => {
    html2canvas(ticketContainerRef.current).then((canvas) => {
      const link = document.createElement("a");
      link.href = canvas.toDataURL("image/png");
      link.download = `ticket_${ticketData.bookingId}.png`;
      link.click();
    });
  };

  if (loading) return <div className="text-center text-white">Loading...</div>;
  if (error) return <div className="text-center text-danger">Error: {error}</div>;

  return (
    <div className="d-flex flex-column justify-content-center align-items-center min-vh-30 bg-secondary p-4">
      {/* Back Button */}
      <div className="d-flex justify-content-start w-75 mt-3">
        <button
          onClick={() => navigate(-1)}
          className="btn btn-outline-success d-flex align-items-center text-warning bg-danger"
          aria-label="Go back"
        >
          <ArrowLeft className="me-2" /> Back
        </button>
      </div>

      {/* Ticket Container */}
      <div className="bg-white p-4 rounded shadow w-50" ref={ticketContainerRef}>
        <h2 className="text-center text-success mb-3">✈️ Ticket Confirmation</h2>
        <hr className="my-2" />

        {/* Booking Details */}
        <div className="mb-3">
          <h4 className="font-weight-bold text-primary mb-2">Booking Details</h4>
          <div className="d-flex justify-content-between">
            <span className="text-secondary">Booking ID:</span>
            <span>{ticketData.bookingId}</span>
          </div>
          <div className="d-flex justify-content-between">
            <span className="text-secondary">Flight ID:</span>
            <span>{ticketData.flightId}</span>
          </div>
          <div className="d-flex justify-content-between">
            <span className="text-secondary">Date of Travel:</span>
            <span>{new Date(ticketData.dateOfTravel).toDateString()}</span>
          </div>
        </div>

        <hr className="my-3" />

        {/* Passenger Details */}
        <div className="mb-3">
          <h4 className="font-weight-bold text-primary mb-2">Passenger Details</h4>
          <div className="d-flex justify-content-between">
            <span className="text-secondary">Name:</span>
            <span>{ticketData.passengerName}</span>
          </div>
          <div className="d-flex justify-content-between">
            <span className="text-secondary">Email:</span>
            <span>{ticketData.email}</span>
          </div>
          <div className="d-flex justify-content-between">
            <span className="text-secondary">Phone Number:</span>
            <span>{ticketData.phoneNumber}</span>
          </div>
        </div>

        {/* Flight Details */}
        {flightData && (
          <>
            <hr className="my-3" />
            <div className="mb-3">
              <h4 className="font-weight-bold text-primary mb-2">Flight Details</h4>
              <div className="d-flex justify-content-between">
                <span className="text-secondary">Airline Name:</span>
                <span>{flightData.airlineName}</span>
              </div>
              <div className="d-flex justify-content-between">
                <span className="text-secondary">From:</span>
                <span>{flightData.source}</span>
              </div>
              <div className="d-flex justify-content-between">
                <span className="text-secondary">To:</span>
                <span>{flightData.destination}</span>
              </div>
              <div className="d-flex justify-content-between">
                <span className="text-secondary">Departure Time:</span>
                <span>{flightData.departureTime}</span>
              </div>
              <div className="d-flex justify-content-between">
                <span className="text-secondary">Arrival Time:</span>
                <span>{flightData.arrivalTime}</span>
              </div>
              <div className="d-flex justify-content-between">
                <span className="text-secondary">Available Seats:</span>
                <span>{flightData.availableSeats}</span>
              </div>
              <div className="d-flex justify-content-between">
                <span className="text-secondary">Ticket Price:</span>
                <span>₹{flightData.ticketPrice}</span>
              </div>
            </div>
          </>
        )}
      </div>

      {/* Buttons */}
      <div className="w-50 mt-3">
        <button
          onClick={handleCancelBooking}
          className="btn btn-danger w-100 py-2 d-flex align-items-center justify-content-center"
          aria-label="Cancel booking"
        >
          <X className="me-2" /> Cancel My Booking
        </button>
        <button
          onClick={downloadTicketAsImage}
          className="btn btn-primary w-100 py-2 mt-3 d-flex align-items-center justify-content-center"
          aria-label="Download ticket"
        >
          📥 Download Ticket
        </button>
      </div>

      {/* Confirmation Modal */}
      {showCancelModal && (
        <div
          className="modal show"
          style={{ display: "block", backgroundColor: "rgba(0,0,0,0.5)" }}
          tabIndex="-1"
          role="dialog"
        >
          <div className="modal-dialog" role="document">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Confirm Cancellation</h5>
                <button type="button" className="btn-close" onClick={cancelCancel} aria-label="Close"></button>
              </div>
              <div className="modal-body">
                <p>Are you sure you want to cancel your booking?</p>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn-secondary" onClick={cancelCancel}>
                  No
                </button>
                <button type="button" className="btn btn-danger" onClick={confirmCancel}>
                  Yes, Cancel
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}