import React, { useEffect, useState } from "react";
import "./ProfileCard.css";

const ProfileCard = () => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const username = localStorage.getItem("username");

  useEffect(() => {
    if (!username) {
      setError("No username found in localStorage.");
      setLoading(false);
      return;
    }

    const fetchUserDetails = async () => {
      try {
        console.log("Fetching user details for:", username);
        const response = await fetch(`http://localhost:5087/api/User/${username}`);

        console.log("API Response Status:", response.status);
        if (!response.ok) {
          throw new Error("User not found.");
        }

        const data = await response.json();
        console.log("Fetched User Data:", data); // Debugging

        setUser(data);
      } catch (err) {
        console.error("Fetch Error:", err.message);
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchUserDetails();
  }, [username]);

  if (loading) return <div className="profile-card"><p>Loading profile...</p></div>;
  if (error) return <div className="profile-card error"><p>{error}</p></div>;

  return (
    <div className="profile-card">
      <div className="profile-header">
        <h1>✨ {user?.fullName}'s Profile ✨</h1>
      </div>
      <div className="profile-details">
        <p><strong>👤 Username:</strong> {user?.username}</p>
        <p><strong>📧 Email:</strong> {user?.email}</p>
        <p><strong>📞 Phone Number:</strong> {user?.phoneNumber}</p>
        <p><strong>👨‍💻 Full Name:</strong> {user?.fullName}</p>
      </div>
    </div>
  );
};

export default ProfileCard;
