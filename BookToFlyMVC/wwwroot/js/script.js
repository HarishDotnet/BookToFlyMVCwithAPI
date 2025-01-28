// function togglePasswordVisibility(show, element) {
//     const passwordInput = document.getElementById('Password');
//     const toggleIcon = element.querySelector('i'); // Target the <i> icon inside the <span>

//     if (show) {
//         passwordInput.type = 'text'; // Show password
//         toggleIcon.classList.remove('bi-eye'); // Change icon to eye-slash
//         toggleIcon.classList.add('bi-eye-slash');
//     } else {
//         passwordInput.type = 'password'; // Hide password
//         toggleIcon.classList.remove('bi-eye-slash');
//         toggleIcon.classList.add('bi-eye');
//     }
// }
// // Function to toggle the visibility of password
// function togglePasswordVisibility(show, element) {
//     var passwordField = document.getElementById('Password');
//     var confirmPasswordField = document.getElementById('ConfirmPassword');
//     if (show) {
//         passwordField.type = 'text';
//         confirmPasswordField.type = 'text';
//     } else {
//         passwordField.type = 'password';
//         confirmPasswordField.type = 'password';
//     }
// }

// // Client-Side Password Confirmation Validation
// const form = document.getElementById('registrationForm');
// const passwordInput = document.getElementById('Password');
// const confirmPasswordInput = document.getElementById('ConfirmPassword');
// const confirmPasswordError = document.getElementById('confirmPasswordError');

// form.addEventListener('submit', function (event) {
//     if (passwordInput.value !== confirmPasswordInput.value) {
//         event.preventDefault(); // Prevent form submission if passwords don't match
//         confirmPasswordError.textContent = 'Password and Confirm Password must match.';
//     } else {
//         confirmPasswordError.textContent = ''; // Clear error if passwords match
//     }
// });