import React from "react";
import reportWebVitals from './reportWebVitals';
import ReactDOM from "react-dom/client";
import App from "./App";
import { AuthProvider } from "./Context/AuthContext";

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(
  //<React.StrictMode> it helps find problems while developing.not used in protection
  <React.StrictMode>
    <AuthProvider> 
      <App className="background"/> 
    </AuthProvider>
  </React.StrictMode>
);
//it help us to measure the perfomance of the website
reportWebVitals();