import React from 'react'

export default function Footer() {
  return (
    <div>
        <footer className="bg-dark text-white py-4">
        <div className="container text-center">
          <p className="mb-0">
            &copy; {new Date().getFullYear()} Flight Booking App. All rights
            reserved.
          </p>
          <nav>
            <a
              href="/terms"
              className="text-white mx-2 text-decoration-none"
            >
              Terms of Service
            </a>
            <a
              href="/privacy"
              className="text-white mx-2 text-decoration-none"
            >
              Privacy Policy
            </a>
          </nav>
        </div>
      </footer>
    </div>
  )
}
