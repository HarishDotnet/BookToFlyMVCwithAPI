import { useAuth } from "../Context/AuthContext";
import { useNavigate } from "react-router-dom";
import React, { useState } from "react";
import UserRegistration from "./Register";
import { FaEye, FaEyeSlash } from "react-icons/fa"; // Import FontAwesome icons

const LoginPage = ({ setShowLogin }) => {
  const { login } = useAuth();
  const [formData, setFormData] = useState({ username: "", password: "" });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [showPassword, setShowPassword] = useState(false); // State for password visibility
  const navigate = useNavigate();

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleCloseModal = () => {
    setShowModal(false); // Close the modal
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    const response = await fetch("http://localhost:5087/api/user/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(formData),
    });

    const data = await response.json();
    setLoading(false);

    if (response.ok) {
      login(data.role); // Update as per your logic
      localStorage.setItem("userToken", data.token); // Store user token or identification
      alert("Login Successful");    
      localStorage.setItem("username",formData.username);
      // console.log("11111111111111111",formData.username);
      setShowLogin(false); // Close modal on success
      navigate("/UserDashboard", {}); // Redirect to dashboard
    } else {
      setError(data.message || "Login failed. Please try again.");
    }
  };

  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword); // Toggle password visibility
  };

  const handleRegisterClick = (e) => {
    e.preventDefault();
    setShowModal(true); // Show the modal when the "Register" link is clicked
  };

  return (
    <div className="container mt-3 d-flex justify-content-center">
      <div className="card shadow-sm p-4" style={{ width: "30rem", borderRadius: "15px" }}>
        <h2 className="text-center mt-2 text-success">Login</h2>
        <hr />
        {error && <div className="alert alert-danger p-2">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label htmlFor="username" className="form-label">Username:</label>
            <input
              type="text"
              id="username"
              name="username"
              className="form-control"
              placeholder="Enter your username"
              value={formData.username}
              onChange={handleInputChange}
              required
            />
          </div>

          <div className="mb-3 position-relative">
            <label htmlFor="password" className="form-label">Password:</label>
            <div className="input-group">
              <input
                type={showPassword ? "text" : "password"} // Toggle input type
                id="password"
                name="password"
                className="form-control"
                placeholder="Enter your password"
                value={formData.password}
                onChange={handleInputChange}
                required
              />
              <span
                className="input-group-text"
                style={{ cursor: "pointer" }}
                onClick={togglePasswordVisibility} // Toggle visibility on click
              >
                {showPassword ? <FaEyeSlash /> : <FaEye />} {/* Toggle between icons */}
              </span>
            </div>
          </div>

          <div className="d-grid">
            <button type="submit" className="btn btn-primary" disabled={loading}>
              {loading ? "Logging in..." : "Login"}
            </button>
          </div>
        </form>

        <div className="text-center mt-4">
          <p>
            Don't have an account?{" "}
            <span
              style={{ color: "blue", cursor: "pointer", textDecoration: "underline" }}
              onClick={handleRegisterClick}
            >
              Register here
            </span>
          </p>
        </div>
      </div>

      {/* Modal for User Registration */}
      <div
        className={`modal ${showModal ? "show" : ""}`}
        tabIndex="-1"
        style={{ display: showModal ? "block" : "none", marginTop: 50 }}
        aria-labelledby="registerModalLabel"
        aria-hidden={!showModal}
      >
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h4 className="modal-title text-success" id="registerModalLabel">
                User Registration
              </h4>
              <button type="button" className="btn-close  bg-danger" onClick={handleCloseModal} aria-label="Close"></button>
            </div>
            <div className="modal-body">
              <UserRegistration />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;