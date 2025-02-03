import { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { Landmark, Plane, MapPin, Clock, User, Mail, Phone, Calendar } from "lucide-react";

export default function BookingForm({ onClose }) {
  const location = useLocation();
  const navigate = useNavigate();
  const { flight } = location.state || {};
  const user = localStorage.getItem("username");

  const [formData, setFormData] = useState({
    FlightId: "",
    PassengerName: "",
    Email: "",
    PhoneNumber: "",
    BookingDate: "",  // <-- Track booking date
    DateOfTravel: "",
    isCanceled: false
  });

  const [errors, setErrors] = useState({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (flight) {
      setFormData({
        ...formData,
        FlightName: flight.airlineName,
        FlightId: flight.flightId,
        From: flight.source,
        To: flight.destination,
        Duration: `${Math.floor(flight.duration)}h ${Math.round((flight.duration % 1) * 60)}m`,
        StartTime: flight.departureTime,
        ReachTime: flight.arrivalTime,
        BookingDate: new Date().toISOString().split("T")[0],  // Set today’s date
      });
    }
  }, [flight]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });

    let updatedErrors = { ...errors };

    if (name === "PhoneNumber" && !validatePhoneNumber(value)) {
      updatedErrors.PhoneNumber = "Phone number must start with 6-9 and be 10 digits long.";
    } else {
      delete updatedErrors.PhoneNumber;
    }

    if (name === "PassengerName" && !validatePassengerName(value)) {
      updatedErrors.PassengerName = "Passenger name must contain only alphabets.";
    } else {
      delete updatedErrors.PassengerName;
    }

    if (name === "DateOfTravel" && !validateDate(value)) {
      updatedErrors.DateOfTravel = "Date of travel must be in the future (not today).";
    } else {
      delete updatedErrors.DateOfTravel;
    }
    setErrors(updatedErrors);
  };

  const generateUniqueBookingId = async () => {
    let bookingId;
    let idExists = true;

    while (idExists) {
      bookingId = Math.floor(Math.random() * (100000 - 100 + 1)) + 100;  // Generate bookingId in range 100-100000

      // Check if bookingId exists in the API
      const response = await fetch(`http://localhost:5087/api/Booking/${bookingId}`);
      if (response.status === 404) {  // Status 404 means bookingId doesn't exist
        idExists = false;
      }
    }

    return bookingId;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    let formErrors = {};

    if (!validatePhoneNumber(formData.PhoneNumber)) {
      formErrors.PhoneNumber = "Phone number must start with 6-9 and be 10 digits long.";
    }

    if (!validatePassengerName(formData.PassengerName)) {
      formErrors.PassengerName = "Passenger name must contain only alphabets.";
    }

    if (!validateDate(formData.DateOfTravel)) {
      formErrors.DateOfTravel = "Date of travel must be in the future (not today).";
    }

    // If errors exist, update state and stop submission
    if (Object.keys(formErrors).length > 0) {
      setErrors(formErrors);
      return;
    }

    setErrors({}); // Clear all errors when valid

    try {
      setIsSubmitting(true); // Disable button during submission

      const uniqueBookingId = await generateUniqueBookingId();

      // Prepare the payload for the Booking API
      const bookingData = {
        bookingId: uniqueBookingId.toString(), // Ensure it's a string
        flightId: formData.FlightId,
        passengerName: formData.PassengerName,
        email: formData.Email,
        phoneNumber: formData.PhoneNumber,
        bookingDate: new Date().toISOString(), // Current date in ISO format
        dateOfTravel: new Date(formData.DateOfTravel).toISOString(), // Convert to ISO format
        isCanceled: false,
      };

      // Make the Booking API request
      const bookingResponse = await fetch('http://localhost:5087/api/Booking', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(bookingData),
      });

      if (!bookingResponse.ok) {
        throw new Error("Booking request failed: " + bookingResponse.statusText);
      }

      // Once booking is successful, call the Ticket API
      const ticketApiUrl = `http://localhost:5087/api/Ticket?username=${encodeURIComponent(user)}&BookingID=${uniqueBookingId}`;

      const ticketResponse = await fetch(ticketApiUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
      });

      if (ticketResponse.ok) {
        console.log("Booking and ticket creation successful!");
        if (onClose && typeof onClose === 'function') {
          onClose(); // Close the form if onClose is passed as a function
        }
        navigate(`/TicketDetails/${bookingData.bookingId}`, { state: { bookingData } });
      } else {
        throw new Error("Ticket request failed: " + ticketResponse.statusText);
      }
    } catch (error) {
      console.error("Error during booking:", error);
      setErrors({ apiError: "An unexpected error occurred. Please try again." });
    } finally {
      setIsSubmitting(false); // Re-enable button after submission
    }
  };


  const validatePhoneNumber = (phone) => {
    const regex = /^[6-9]\d{9}$/;
    return regex.test(phone);
  };

  const validatePassengerName = (name) => {
    const regex = /^[A-Za-z\s]+$/;
    return regex.test(name);
  };

  const validateDate = (date) => {
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Set time to midnight to compare only the date
    const selectedDate = new Date(date);
    return selectedDate > today;
  };

  const handleClose = () => {
    console.log("Close button clicked");
    navigate('/SearchFlight');
  };

  return (
    <div className="modal show d-block mt-4">
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header bg-primary">
            <h5 className="modal-title text-light">✈️ Book Your Flight</h5>
            <button
              type="button"
              className="btn-close"
              onClick={handleClose}
              aria-label="Close"
            ></button>
          </div>
          <div className="modal-body" style={{ maxHeight: '70vh', overflowY: 'auto', padding: '10px 15px' }}>
            <form className="space-y-3" onSubmit={handleSubmit}>
              {/* Flight Information Fields */}
              <div>
                <h3 className="h6 font-weight-bold text-primary mb-2 text-sm">Flight Information</h3>
                <hr className="my-2" />
                {/* Flight Name */}
                <div className="mb-3">
                  <div className="d-flex align-items-center border rounded p-2">
                    <Landmark className="text-secondary me-1" />
                    <input
                      type="text"
                      name="FlightName"
                      value={formData.FlightName}
                      disabled
                      className="form-control form-control-sm border-0 fw-bolder bg-transparent"
                    />
                  </div>
                </div>
                {/* Flight ID */}
                <div className="mb-3">
                  <div className="d-flex align-items-center border rounded p-2">
                    <Plane className="text-secondary me-1" />
                    <input
                      type="text"
                      name="FlightId"
                      value={formData.FlightId}
                      disabled
                      className="form-control form-control-sm border-0 fw-bolder bg-transparent"
                    />
                  </div>
                </div>
                {/* From and To */}
                <div className="row mb-3">
                  <div className="col-6">
                    <div className="d-flex align-items-center border rounded p-2">
                      <MapPin className="text-danger me-1" />
                      <input
                        type="text"
                        name="From"
                        value={formData.From}
                        disabled
                        className="form-control form-control-sm border-0 fw-bolder bg-transparent"
                      />
                    </div>
                  </div>
                  <div className="col-6">
                    <div className="d-flex align-items-center border rounded p-2">
                      <MapPin className="text-success me-1" />
                      <input
                        type="text"
                        name="To"
                        value={formData.To}
                        disabled
                        className="form-control form-control-sm border-0 fw-bolder bg-transparent"
                      />
                    </div>
                  </div>
                </div>
                {/* Time */}
                <div className="row mb-3">
                  <div className="col-4">
                    <label className="text-secondary text-sm">Start Time</label>
                    <div className="d-flex align-items-center border rounded p-2">
                      <Clock className="text-blue-500 me-1" />
                      <input
                        type="time"
                        name="StartTime"
                        value={formData.StartTime}
                        onChange={handleChange}
                        disabled
                        className="form-control form-control-sm border-0 bg-transparent fw-bolder text-danger"
                      />
                    </div>
                  </div>
                  <div className="col-4 text-center position-relative">
                    <span className="position-absolute top-50 start-50 translate-middle fw-bolder text-info text-sm">
                      Duration
                    </span>
                    <div className="text-center bg-dark text-warning rounded">{formData.Duration}</div>
                  </div>
                  <div className="col-4">
                    <label className="text-secondary text-sm">End Time</label>
                    <div className="d-flex align-items-center border rounded p-2">
                      <Clock className="text-purple-500 me-1" />
                      <input
                        type="time"
                        name="ReachTime"
                        value={formData.ReachTime}
                        onChange={handleChange}
                        disabled
                        className="form-control form-control-sm border-0 bg-transparent fw-bolder text-success"
                      />
                    </div>
                  </div>
                </div>
              </div>

              {/* User Details Fields */}
              <div className="mb-3">
                <h3 className="h6 font-weight-bold text-primary mb-2 text-sm">User Details</h3>
                <hr className="my-2" />
                {/* Name */}
                <div className="d-flex align-items-center border rounded p-2">
                  <User className="text-secondary me-1" />
                  <input
                    type="text"
                    name="PassengerName"
                    value={formData.PassengerName}
                    onChange={handleChange}
                    placeholder="Full Name"
                    className="form-control form-control-sm border-0 fw-bolder bg-transparent"
                  />
                </div>
                {errors.PassengerName && <div className="text-danger">{errors.PassengerName}</div>}
                {/* Email */}
                <div className="d-flex align-items-center border rounded p-2">
                  <Mail className="text-secondary me-1" />
                  <input
                    type="email"
                    name="Email"
                    value={formData.Email}
                    onChange={handleChange}
                    placeholder="Email Address"
                    className="form-control form-control-sm border-0 fw-bolder bg-transparent"
                  />
                </div>
                {errors.Email && <div className="text-danger">{errors.Email}</div>}
                {/* Phone */}
                <div className="d-flex align-items-center border rounded p-2">
                  <Phone className="text-secondary me-1" />
                  <input
                    type="text"
                    name="PhoneNumber"
                    value={formData.PhoneNumber}
                    onChange={handleChange}
                    placeholder="Phone Number"
                    className="form-control form-control-sm border-0 fw-bolder bg-transparent"
                  />
                </div>
                {errors.PhoneNumber && <div className="text-danger">{errors.PhoneNumber}</div>}
                {/* Date of Travel */}
                <label className="text-dark ">DateOfTravel:</label>
                <div className="d-flex align-items-center border rounded p-2">
                  <Calendar className="text-secondary me-1" />
                  <input
                    type="date"
                    name="DateOfTravel"
                    value={formData.DateOfTravel}
                    onChange={handleChange}
                    className="form-control form-control-sm border-0 fw-bolder bg-transparent"
                  />
                </div>
                {errors.DateOfTravel && <div className="text-danger">{errors.DateOfTravel}</div>}
              </div>

              {/* Submit Button */}
              <button
                type="submit"
                className="btn btn-primary w-100"
                disabled={isSubmitting || Object.keys(errors).length > 0}
              >
                {isSubmitting ? 'Booking...' : 'Book Flight'}
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}
