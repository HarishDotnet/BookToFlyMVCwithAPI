import React, { createContext, useContext, useState, useEffect } from 'react';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  // Initialize state from localStorage if available
  const [isLoggedIn, setIsLoggedIn] = useState(() => {
    const storedAuth = localStorage.getItem('auth');
    return storedAuth ? JSON.parse(storedAuth).isLoggedIn : false;
  });

  const [role, setRole] = useState(() => {
    const storedAuth = localStorage.getItem('auth');
    return storedAuth ? JSON.parse(storedAuth).role : null;
  });

  // Update localStorage whenever isLoggedIn or role changes
  useEffect(() => {
    localStorage.setItem('auth', JSON.stringify({ isLoggedIn, role }));
  }, [isLoggedIn, role]);

  const login = (userRole) => {
    setIsLoggedIn(true);
    setRole(userRole);
  };

  const logout = () => {
    setIsLoggedIn(false);
    setRole(null);
    localStorage.removeItem('auth'); // Clear auth data from localStorage
  };

  return (
    <AuthContext.Provider value={{ isLoggedIn, role, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);