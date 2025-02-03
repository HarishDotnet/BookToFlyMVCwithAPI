// FlightDetailModal.js
import React from 'react';
import { Modal } from 'react-bootstrap'; // React Bootstrap
import 'bootstrap/dist/css/bootstrap.min.css';

const FlightDetailModal = ({ flight, show, onHide }) => {
  return (
    <Modal show={show} onHide={onHide}>
      <Modal.Header closeButton>
        <Modal.Title>{flight.airlineName}</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <p><strong>Flight ID:</strong> {flight.flightId}</p>
        <p><strong>From:</strong> {flight.source}</p>
        <p><strong>To:</strong> {flight.destination}</p>
        <p><strong>Arrival:</strong> {flight.arrivalTime}</p>
        <p><strong>Departure:</strong> {flight.departureTime}</p>
        <p><strong>Duration:</strong> {flight.duration} hrs</p>
        <p><strong>Available Seats:</strong> {flight.availableSeats}</p>
        <p><strong>Price:</strong> ${flight.ticketPrice}</p>
      </Modal.Body>
    </Modal>
  );
};

export default FlightDetailModal;
