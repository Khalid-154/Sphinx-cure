document.addEventListener('DOMContentLoaded', function () {
    // Initialize theme
    initializeTheme();

    // Update current time
    function updateTime() {
        const now = new Date();
        const options = {
            weekday: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        };
        document.getElementById('currentTime').textContent = now.toLocaleDateString('en-US', options);
    }

    updateTime();
    setInterval(updateTime, 60000);

    // Sidebar toggle for desktop
    const sidebarToggleDesktop = document.getElementById('sidebarToggleDesktop');
    const sidebarWrapper = document.querySelector('.sidebar-wrapper');

    if (sidebarToggleDesktop) {
        sidebarToggleDesktop.addEventListener('click', function () {
            sidebarWrapper.classList.toggle('collapsed');
        });
    }

    // Sidebar toggle for mobile
    const sidebarToggleMobile = document.getElementById('sidebarToggleMobile');
    const mobileMenuToggle = document.getElementById('mobileMenuToggle');

    if (sidebarToggleMobile) {
        sidebarToggleMobile.addEventListener('click', function () {
            sidebarWrapper.classList.toggle('mobile-open');
        });
    }

    // Mobile menu toggle button (if exists)
    if (mobileMenuToggle) {
        mobileMenuToggle.addEventListener('click', function () {
            sidebarWrapper.classList.toggle('mobile-open');
        });
    }

    // Close sidebar on mobile when clicking outside
    document.addEventListener('click', function (event) {
        if (window.innerWidth <= 992) {
            const sidebar = document.querySelector('.sidebar-wrapper');
            const mobileToggle = document.querySelector('.mobile-menu-toggle');

            if (!sidebar.contains(event.target) &&
                (!mobileToggle || !mobileToggle.contains(event.target)) &&
                sidebar.classList.contains('mobile-open')) {
                sidebar.classList.remove('mobile-open');
            }
        }
    });

    // Add animation to cards on load
    const cards = document.querySelectorAll('.stat-card, .quick-action-btn');
    cards.forEach((card, index) => {
        card.style.animationDelay = `${index * 0.1}s`;
        card.classList.add('animate__animated', 'animate__fadeInUp');
    });

    // Update active navigation link
    const currentPath = window.location.pathname.toLowerCase();
    const navLinks = document.querySelectorAll('.sidebar-nav .nav-link');

    navLinks.forEach(link => {
        const href = link.getAttribute('href').toLowerCase();
        if (currentPath.includes(href.replace('dashboard', '')) ||
            (currentPath === '/' && href.includes('dashboard'))) {
            link.classList.add('active');
        } else {
            link.classList.remove('active');
        }
    });

    // Theme toggle functionality
    function initializeTheme() {
        const themeToggle = document.getElementById('themeToggle');
        const savedTheme = localStorage.getItem('theme') || 'light';

        // Apply saved theme
        document.documentElement.setAttribute('data-theme', savedTheme);

        // Update toggle button icon
        updateThemeToggleIcon(savedTheme);

        // Add event listener to theme toggle button
        if (themeToggle) {
            themeToggle.addEventListener('click', toggleTheme);
        }
    }

    function toggleTheme() {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';

        // Apply new theme
        document.documentElement.setAttribute('data-theme', newTheme);

        // Save to localStorage
        localStorage.setItem('theme', newTheme);

        // Update toggle button icon
        updateThemeToggleIcon(newTheme);

        // Dispatch custom event for other components
        document.dispatchEvent(new CustomEvent('themeChanged', { detail: newTheme }));
    }

    function updateThemeToggleIcon(theme) {
        const themeToggle = document.getElementById('themeToggle');
        if (themeToggle) {
            const icon = themeToggle.querySelector('i');
            if (icon) {
                // Icon will be automatically updated via CSS
                // This function is for any additional updates if needed
            }
        }
    }

    // Listen for system theme changes
    if (window.matchMedia) {
        const systemThemeQuery = window.matchMedia('(prefers-color-scheme: dark)');

        systemThemeQuery.addEventListener('change', (e) => {
            // Only auto-switch if user hasn't made a manual choice
            if (!localStorage.getItem('theme')) {
                const newTheme = e.matches ? 'dark' : 'light';
                document.documentElement.setAttribute('data-theme', newTheme);
                updateThemeToggleIcon(newTheme);
            }
        });
    }
});
