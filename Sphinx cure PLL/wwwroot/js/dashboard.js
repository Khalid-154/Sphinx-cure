document.addEventListener('DOMContentLoaded', function () {
    // Initialize theme
    initializeTheme();

    // Update current time with hours and minutes
    function updateTime() {
        const now = new Date();
        const options = {
            weekday: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        };
        document.getElementById('currentTime').textContent = now.toLocaleDateString('en-US', options);
    }

    updateTime();
    setInterval(updateTime, 1000); // Update every second for seconds display

    // Add animation to cards on load
    setTimeout(function () {
        const cards = document.querySelectorAll('.stat-card');
        cards.forEach((card, index) => {
            card.style.animationDelay = `${index * 0.1}s`;
        });
    }, 100);

    // Check and apply saved sidebar state
    const savedSidebarState = localStorage.getItem('sidebarCollapsed');
    if (savedSidebarState === 'true' && layoutContainer) {
        layoutContainer.classList.add('sidebar-collapsed');
    }

    // Update active navigation link
    function updateActiveNavLink() {
        const currentPath = window.location.pathname.toLowerCase();
        const navLinks = document.querySelectorAll('.sidebar-menu .menu-item');

        navLinks.forEach(link => {
            link.classList.remove('active');
            const href = link.getAttribute('href').toLowerCase();

            // Check if current path matches the href
            if (href && currentPath.includes(href.replace('/home/dashboard', '').replace('/dashboard', '')) ||
                (currentPath === '/' && href.includes('dashboard'))) {
                link.classList.add('active');
            }
        });
    }

    updateActiveNavLink();

    // Theme toggle functionality
    function initializeTheme() {
        const themeToggle = document.getElementById('themeToggle');
        const prefersDarkScheme = window.matchMedia('(prefers-color-scheme: dark)');

        // Get saved theme or use system preference
        let savedTheme = localStorage.getItem('theme');

        if (!savedTheme) {
            savedTheme = prefersDarkScheme.matches ? 'dark' : 'light';
        }

        // Apply saved theme
        document.documentElement.setAttribute('data-theme', savedTheme);

        // Update toggle button icon
        updateThemeToggleIcon(savedTheme);

        // Add event listener to theme toggle button
        if (themeToggle) {
            themeToggle.addEventListener('click', toggleTheme);
        }

        // Listen for system theme changes (only if no manual theme is set)
        prefersDarkScheme.addEventListener('change', function (e) {
            if (!localStorage.getItem('theme')) {
                const newTheme = e.matches ? 'dark' : 'light';
                document.documentElement.setAttribute('data-theme', newTheme);
                updateThemeToggleIcon(newTheme);
            }
        });
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

        // Add smooth transition
        document.body.style.transition = 'background-color 0.3s ease, color 0.3s ease';
        setTimeout(() => {
            document.body.style.transition = '';
        }, 300);
    }

    function updateThemeToggleIcon(theme) {
        const themeToggle = document.getElementById('themeToggle');
        if (themeToggle) {
            const sunIcon = themeToggle.querySelector('.fa-sun');
            const moonIcon = themeToggle.querySelector('.fa-moon');

            if (sunIcon && moonIcon) {
                // Remove any existing classes first
                sunIcon.classList.remove('d-none');
                moonIcon.classList.remove('d-none');

                // Add Bootstrap d-none class to hide/show
                if (theme === 'dark') {
                    sunIcon.classList.add('d-none');
                    moonIcon.classList.remove('d-none');
                } else {
                    sunIcon.classList.remove('d-none');
                    moonIcon.classList.add('d-none');
                }
            }
        }
    }

    // Initialize tooltips for collapsed sidebar
    function initializeTooltips() {
        const menuItems = document.querySelectorAll('.menu-item');
        menuItems.forEach(item => {
            // Ensure tooltip attribute exists
            if (!item.hasAttribute('data-tooltip')) {
                const menuText = item.querySelector('.menu-text');
                if (menuText) {
                    item.setAttribute('data-tooltip', menuText.textContent.trim());
                }
            }
        });
    }

    initializeTooltips();

    // Add loading animation for better UX
    document.body.style.opacity = '0';
    document.body.style.transition = 'opacity 0.3s ease';
    setTimeout(() => {
        document.body.style.opacity = '1';
    }, 100);
});