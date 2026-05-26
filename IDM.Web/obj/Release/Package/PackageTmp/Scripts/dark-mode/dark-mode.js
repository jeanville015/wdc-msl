// Dark Mode Module
var DarkModeModule = (function () {
    'use strict';

    const DARK_MODE_KEY = 'darkModeEnabled';

    function init() {
        loadDarkModePreference();
        bindEvents();
    }

    function bindEvents() {
        // Dark mode toggle switch
        $('#darkModeToggle').on('change', function() {
            toggleDarkMode($(this).is(':checked'));
        });

        // Optional: Add keyboard shortcut (Ctrl/Cmd + Shift + D)
        $(document).on('keydown', function(e) {
            if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'D') {
                e.preventDefault();
                const currentMode = isDarkModeEnabled();
                toggleDarkMode(!currentMode);
                $('#darkModeToggle').prop('checked', !currentMode);
            }
        });
    }

    function toggleDarkMode(enabled) {
        if (enabled) {
            $('body').addClass('dark-mode');
            localStorage.setItem(DARK_MODE_KEY, 'true');
            // Update text to "Dark Mode" when dark mode is enabled
            $('.navbar-nav .nav-item span').first().text('Dark Mode ');

            // Show moon icon in dark mode
            $('#darkModeIcon')
                .removeClass('fa-sun')
                .addClass('fa-moon');
            console.log('Dark mode enabled');
        } else {
            $('body').removeClass('dark-mode');
            localStorage.setItem(DARK_MODE_KEY, 'false');
            // Update text to "Light Mode" when light mode is enabled
            $('.navbar-nav .nav-item span').first().text('Light Mode ');

            // Show sun icon in light mode
            $('#darkModeIcon')
                .removeClass('fa-moon')
                .addClass('fa-sun');
            console.log('Dark mode disabled');
        }
    }

    function isDarkModeEnabled() {
        return localStorage.getItem(DARK_MODE_KEY) === 'true';
    }

    function loadDarkModePreference() {
        const darkModePref = localStorage.getItem(DARK_MODE_KEY);
        const systemPrefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        
        let enableDarkMode = false;
        
        if (darkModePref === 'true') {
            enableDarkMode = true;
        } else if (darkModePref === 'false') {
            enableDarkMode = false;
        } else if (systemPrefersDark) {
            // Use system preference if no saved preference
            enableDarkMode = true;
        }
        
        // Apply dark mode, update toggle, and ensure label/icon are in sync
        toggleDarkMode(enableDarkMode);
        $('#darkModeToggle').prop('checked', enableDarkMode);
    }

    function setDarkMode(enabled) {
        toggleDarkMode(enabled);
        $('#darkModeToggle').prop('checked', enabled);
    }

    // Listen for system theme changes
    function watchSystemTheme() {
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function(e) {
            // Only change if user hasn't explicitly set a preference
            if (localStorage.getItem(DARK_MODE_KEY) === null) {
                setDarkMode(e.matches);
            }
        });
    }

    // Initialize system theme watcher
    watchSystemTheme();

    return {
        init: init,
        toggle: toggleDarkMode,
        set: setDarkMode,
        isEnabled: isDarkModeEnabled
    };
})();

$(document).ready(function () {
    DarkModeModule.init();
});
