// Password toggle function
function togglePassword(inputId) {
    const passwordInput = document.getElementById(inputId);
    const toggleIcon = passwordInput.nextElementSibling.querySelector('i');

    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        toggleIcon.classList.remove('bi-eye');
        toggleIcon.classList.add('bi-eye-slash');
    } else {
        passwordInput.type = 'password';
        toggleIcon.classList.remove('bi-eye-slash');
        toggleIcon.classList.add('bi-eye');
    }
}

// Form submission feedback
$(document).ready(function () {
    $('form').on('submit', function () {
        const submitBtn = $(this).find('button[type="submit"]');
        submitBtn.prop('disabled', true);
        submitBtn.html('<i class="bi bi-hourglass-split"></i> Signing in...');
    });
});

// Dark/Light Mode Toggle
$(document).ready(function () {
    const themeToggle = $('#theme-toggle');
    const body = $('body');
    const card = $('.card');

    // Check for saved theme preference or default to light mode
    const currentTheme = localStorage.getItem('theme') || 'light';
    if (currentTheme === 'dark') {
        body.addClass('dark-mode');
        card.removeClass('bg-light').addClass('bg-dark text-light');
        themeToggle.find('i').removeClass('bi-moon').addClass('bi-sun');
    }

    // Toggle theme on button click
    themeToggle.on('click', function () {
        if (body.hasClass('dark-mode')) {
            // Switch to light mode
            body.removeClass('dark-mode');
            card.removeClass('bg-dark text-light').addClass('bg-light');
            themeToggle.find('i').removeClass('bi-sun').addClass('bi-moon');
            localStorage.setItem('theme', 'light');
        } else {
            // Switch to dark mode
            body.addClass('dark-mode');
            card.removeClass('bg-light').addClass('bg-dark text-light');
            themeToggle.find('i').removeClass('bi-moon').addClass('bi-sun');
            localStorage.setItem('theme', 'dark');
        }
    });
});
