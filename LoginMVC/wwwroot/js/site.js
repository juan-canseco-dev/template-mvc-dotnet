// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    // Listen for clicks on any logout button element
    $(document).on('click', '.btn-logout', function (e) {
        e.preventDefault();

        Swal.fire({
            title: 'Sign Out?',
            text: "Are you sure you want to end your current session?",
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Yes, sign out',
            cancelButtonText: 'Stay logged in',
            customClass: {
                popup: 'border-0 shadow-lg rounded-3'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                // Show a quick loader while the request processes
                Swal.fire({
                    title: 'Signing out...',
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });

                // Submit the hidden security form
                $('#logoutForm').submit();
            }
        });
    });
});