import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import './App.css';
import HomePage from './Pages/HomePage';
import '../node_modules/bootstrap/dist/js/bootstrap.bundle';
import '../node_modules/bootstrap/dist/css/bootstrap.min.css';
import Navbar from './Header/Navbar';
import Register from './Pages/Register';
import 'bootstrap/dist/css/bootstrap.min.css';
import UserDashboard from './Pages/UserDashboard';
import TicketList from './Pages/TicketList';
import TicketDetails from './Pages/TicketDetails';
import BookingForm from './Pages/BookingForm';
import SearchFlight from './Pages/SearchFlight';
import { AuthProvider } from './Context/AuthContext'; // Import AuthProvider

function App() {
  return (
    <AuthProvider> {/* Wrap the entire app with AuthProvider */}
      <Router>
        <div className='App'>
          <Navbar /> {/* Navbar can access authentication state via useAuth */}

          <main>
            <Routes>
              <Route path='/' element={<HomePage />} />
              <Route path='/register' element={<Register />} />
              <Route path="/UserDashboard" element={<UserDashboard />} />
              <Route path="/TicketList" element={<TicketList />} />
              <Route path="/ticket-list/:id" element={<TicketList />} />
              <Route path="/SearchFlight" element={<SearchFlight />} />
              <Route path="/TicketDetails/:bookId" element={<TicketDetails />} />
              <Route path="/BookingForm/:flightId" element={<BookingForm />} />
            </Routes>
          </main>
        </div>
      </Router>
    </AuthProvider>
  );
}

export default App;