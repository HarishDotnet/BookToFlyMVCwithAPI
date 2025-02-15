import { useState } from "react";
import { FaUser, FaEnvelope, FaLock, FaPhoneAlt, FaEyeSlash, FaEye } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
  
export default function UserRegistration({ onClose = () => {} }) { // Ensure onClose is always a function
  const [formData, setFormData] = useState({
    Username: "",
    Email: "",
    Password: "",
    ConfirmPassword: "",
    FullName: "",
    PhoneNumber: "",
  });

  const [errors, setErrors] = useState({});
  const [success, setSuccess] = useState(false);
  const [failure, setFailure] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [backendError, setBackendError] = useState("");

  // const navigate = useNavigate();

  const validateField = (name, value) => {
    let error = "";

    switch (name) {
      case "Username":
        if (!value) error = "Username is required.";
        else if (!/[a-zA-Z]/.test(value))
          error = "Username must contain at least one alphabet character.";
        else if (value.length < 4)
          error = "Username must be at least 4 characters long.";
        break;

      case "Email":
        if (!value) error = "Email is required.";
        else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value))
          error = "Please enter a valid email address.";
        else if (!/\.[a-zA-Z]{2,}$/.test(value))  // Additional validation
          error = "Email must have a valid domain (e.g., .com, .org).";
        break;

      case "Password":
        if (!value) error = "Password is required.";
        else if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{6,15}$/.test(value))
          error = "Password must contain at least one uppercase letter, one lowercase letter, and one special character.";
        break;

      case "ConfirmPassword":
        if (value !== formData.Password) error = "Passwords do not match.";
        break;

      case "FullName":
        if (!value) error = "Full Name is required.";
        else if (!/^[A-Za-z\s]+$/.test(value))
          error = "Full Name should only contain letters and spaces.";
        break;

      case "PhoneNumber":
        if (!value) error = "Phone number is required.";
        else if (!/^(?:[6-9][0-9]{9}|10[0-9]{8})$/.test(value))
          error = "Enter 10-digit number starting with(6,7,8,9).";
        break;

      default:
        break;
    }

    return error;
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
    setErrors({ ...errors, [name]: validateField(name, value) });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    console.log("Form submitted", formData);

    const newErrors = {};
    Object.keys(formData).forEach((key) => {
      newErrors[key] = validateField(key, formData[key]);
    });

    if (Object.values(newErrors).some((err) => err)) {
      setErrors(newErrors);
      console.log("Validation errors", newErrors);
      return;
    }

    try {
      console.log("Sending request to backend...");
      const response = await fetch("http://localhost:5087/api/User/register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(formData),
      });

      console.log("Response received", response);

      if (response.ok) {
        setSuccess(true);
        setFormData({
          Username: "",
          Email: "",
          Password: "",
          ConfirmPassword: "",
          FullName: "",
          PhoneNumber: "",
        });
        setTimeout(() => {
          setSuccess(false);
          onClose(); // Close the form
          window.location.reload(); // Redirect to home page
        }, 3000);
      } else {
        const data = await response.json();
        setTimeout(() => {
          setFailure(true);
          setBackendError(data.message || "Registration failed.");
          console.error("Registration failed:", data.message);
        }, 3000);
        
      }
    } catch (error) {
      setFailure(true);
      setBackendError("Something went wrong. Please try again by connecting API.");
      console.error("Error during registration:", error);
    }
  };

  return (
    <div className="p-4 rounded-lg" style={{ maxWidth: "500px", margin: "0 auto", backgroundColor: "#f8f9fa" }}>
      {success && (
        <div className="alert alert-success text-center">Registration successful! Redirecting...</div>
      )}
      {failure && (
        <div className="alert alert-danger text-center">{backendError}</div>
      )}

      <form onSubmit={handleSubmit}>

        <div className="mb-3">
          <div className="input-group">
            <span className="input-group-text">
              <FaUser />
            </span>
            <input
              type="text"
              name="Username"
              value={formData.Username}
              onChange={handleChange}
              className="form-control"
              placeholder="Enter username"
            />
          </div>
          {errors.Username && <div className="text-danger">{errors.Username}</div>}
        </div>

        <div className="mb-3">
          <div className="input-group">
            <span className="input-group-text">
              <FaEnvelope />
            </span>
            <input
              type="email"
              name="Email"
              value={formData.Email}
              onChange={handleChange}
              className="form-control"
              placeholder="Enter email"
            />
          </div>
          {errors.Email && <div className="text-danger">{errors.Email}</div>}
        </div>

        <div className="mb-3">
          <div className="input-group">
            <span className="input-group-text">
              <FaUser />
            </span>
            <input
              type="text"
              name="FullName"
              value={formData.FullName}
              onChange={handleChange}
              className="form-control"
              placeholder="Enter full name"
            />
          </div>
          {errors.FullName && <div className="text-danger">{errors.FullName}</div>}
        </div>

        <div className="mb-3">
          <div className="input-group">
            <span className="input-group-text">
              <FaLock />
            </span>
            <input
              type={showPassword ? "text" : "password"}
              name="Password"
              value={formData.Password}
              onChange={handleChange}
              className="form-control"
              placeholder="Enter password"
            />
            <button
              type="button"
              className="btn btn-outline-secondary p-2"
              onClick={() => setShowPassword(!showPassword)}
              aria-label={showPassword ? "Hide password" : "Show password"}
            >
              {showPassword ? <FaEyeSlash /> : <FaEye />}
            </button>
          </div>
          {errors.Password && <div className="text-danger">{errors.Password}</div>}
        </div>

        <div className="mb-3">
          <div className="input-group">
            <span className="input-group-text">
              <FaLock />
            </span>
            <input
              type={"password"}
              name="ConfirmPassword"
              value={formData.ConfirmPassword}
              onChange={handleChange}
              className="form-control"
              placeholder="Confirm password"
            />
          </div>
          {errors.ConfirmPassword && <div className="text-danger">{errors.ConfirmPassword}</div>}
        </div>

        <div className="mb-3">
          <div className="input-group">
            <span className="input-group-text">
              <FaPhoneAlt />
            </span>
            <input
              type="text"
              name="PhoneNumber"
              value={formData.PhoneNumber}
              onChange={handleChange}
              className="form-control"
              placeholder="Enter phone number"
            />
          </div>
          {errors.PhoneNumber && <div className="text-danger">{errors.PhoneNumber}</div>}
        </div>

        <div className="mb-3">
          <button type="submit" className="btn btn-primary w-100">
            Register
          </button>
        </div>
      </form>
    </div>
  );
}
