document.addEventListener('DOMContentLoaded', function () {
    // Restore active menu state from localStorage
    const activeMenu = localStorage.getItem('activeMenu');
    const activeSubMenu = localStorage.getItem('activeSubMenu');

    if (activeMenu) {
        document.querySelectorAll('.nav-item').forEach(item => item.classList.remove('menu-open', 'active'));
        document.querySelector(`.nav-item[data-menu="${activeMenu}"]`).classList.add('menu-open');
    }

    if (activeSubMenu) {
        document.querySelectorAll('.nav-link').forEach(link => link.classList.remove('active'));
        const subMenu = document.querySelector(`.nav-link[data-submenu="${activeSubMenu}"]`);
        if (subMenu) {
            subMenu.classList.add('active');
        }
    }

    // Save the clicked menu state
    document.querySelectorAll('.nav-link').forEach(link => {
        link.addEventListener('click', function () {
            const parentMenu = this.closest('.nav-item[data-menu]');
            if (parentMenu) {
                localStorage.setItem('activeMenu', parentMenu.dataset.menu);
            }
            localStorage.setItem('activeSubMenu', this.dataset.submenu || '');
        });
    });
});

// site.js or a shared helper file
var ToastrHelper = (function () {
    function notification(type, message, title = "") {
        toastr.options = {
            closeButton: true,
            progressBar: true,
            positionClass: "toast-top-right",
            timeOut: "3000",
        };

        switch (type) {
            case "success":
                toastr.success(message, title);
                break;
            case "error":
                toastr.error(message, title);
                break;
            case "info":
                toastr.info(message, title);
                break;
            case "warning":
                toastr.warning(message, title);
                break;
            default:
                console.error("Invalid Toastr type specified.");
        }
    }

    function validateRequiredFields(fields) {
        let isValid = true;

        fields.forEach((field) => {
            const { selector, name } = field;
            const value = $(selector).val().trim();

            if (!value) {
                isValid = false;
                showToastrNotification("error", `The ${name} field is required.`, "Validation Error");
            }
        });

        return isValid;
    }

    // Expose methods
    return {
        notification,
        validateRequiredFields,
    };
})();

// Attach to global window object if needed
window.ToastrHelper = ToastrHelper;

// Global Loading Utility Functions
window.GlobalLoading = {
    show: function(message = 'Loading...', subtitle = 'Please wait...') {
        // Remove any existing loading overlay
        this.hide();
        
        // Create loading overlay
        $('body').append(`
            <div id="globalLoadingOverlay" style="
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background-color: rgba(0, 0, 0, 0.7);
                z-index: 9999;
                display: flex;
                justify-content: center;
                align-items: center;
                color: white;
                font-size: 18px;
            ">
                <div style="text-align: center;">
                    <i class="fas fa-spinner fa-spin fa-3x mb-3"></i>
                    <div>${message}</div>
                    <div style="font-size: 14px; margin-top: 10px;">${subtitle}</div>
                </div>
            </div>
        `);
    },
    
    hide: function() {
        $('#globalLoadingOverlay').fadeOut(300, function() {
            $(this).remove();
        });
    },
    
    showWithFormDisable: function(message = 'Loading...', subtitle = 'Please wait...') {
        this.disableForm();
        this.show(message, subtitle);
    },
    
    hideWithFormEnable: function() {
        this.enableForm();
        this.hide();
    },
    
    disableForm: function() {
        // Disable all form controls
        $('input, select, button, textarea').prop('disabled', true);
        // Add visual indication that page is loading
        $('body').css('cursor', 'wait');
    },
    
    enableForm: function() {
        // Enable all form controls
        $('input, select, textarea').prop('disabled', false);
        $('button').prop('disabled', false);
        // Reset cursor
        $('body').css('cursor', 'default');
    }
};

// Global Row Color Coding Utility
window.RowColorHelper = {
    colors: [
        '#e3f2fd', '#f3e5f5', '#e8f5e8', '#fff3e0', '#fce4ec',
        '#e0f2f1', '#f1f8e9', '#fff8e1', '#f9fbe7', '#e8eaf6'
    ],
    
    getColorForGroup: function(groupName) {
        // Generate consistent color based on group name string
        var colorIndex = 0;
        for (var i = 0; i < groupName.length; i++) {
            colorIndex += groupName.charCodeAt(i);
        }
        colorIndex = colorIndex % this.colors.length;
        return this.colors[colorIndex];
    },
    
    applyColorToRow: function(row, groupName) {
        var color = this.getColorForGroup(groupName);
        $(row).css('background-color', color);
        $(row).css('border-bottom', '1px solid #dee2e6');
    },
    
    // DataTable createdRow callback for color coding by group
    getCreatedRowCallback: function(groupFieldName) {
        return function(row, data, dataIndex) {
            var groupName = data[groupFieldName];
            RowColorHelper.applyColorToRow(row, groupName);
        };
    }
};

// Global Form Preparation Utility
window.GlobalFormPrep = {
    preparePage: function(config = {}) {
        console.log('GlobalFormPrep.preparePage called with:', config);
        
        const {
            loadingMessage = 'Loading Page...',
            loadingSubtitle = 'Please wait while we prepare your page',
            loadDataFunctions = [],
            onBeforeLoad = null,
            onAfterLoad = null,
            onError = null,
            enableFormAfterLoad = true
        } = config;

        console.log('About to show loading with message:', loadingMessage);
        
        // Show loading immediately
        GlobalLoading.showWithFormDisable(loadingMessage, loadingSubtitle);
        console.log('Loading shown');
        
        // Hide page content during loading (optional)
        if (config.hideContentDuringLoad) {
            $('.page-content').hide();
        }

        // Execute before load callback
        if (onBeforeLoad && typeof onBeforeLoad === 'function') {
            onBeforeLoad();
        }

        console.log('About to load data functions:', loadDataFunctions);
        
        // Load all data
        return Promise.all(loadDataFunctions)
            .then(results => {
                console.log('All data loaded successfully:', results);
                
                // Execute after load callback
                if (onAfterLoad && typeof onAfterLoad === 'function') {
                    onAfterLoad(results);
                }
                
                return results;
            })
            .catch(error => {
                console.error('Error preparing page:', error);
                
                // Execute error callback
                if (onError && typeof onError === 'function') {
                    onError(error);
                } else {
                    // Default error handling
                    ToastrHelper.notification("error", "Failed to load page data. Please refresh the page.", "Error");
                }
                
                throw error;
            })
            .finally(() => {
                console.log('Page preparation completed, hiding loading');
                // Always hide loading and enable form
                if (enableFormAfterLoad) {
                    GlobalLoading.hideWithFormEnable();
                } else {
                    GlobalLoading.hide();
                }
                
                // Show page content if it was hidden
                if (config.hideContentDuringLoad) {
                    $('.page-content').fadeIn(300);
                }
            });
    },

    // Template for other pages - customize as needed
    standardPage: function(loadFunctions, options = {}) {
        console.log('GlobalFormPrep.standardPage called with:', loadFunctions, options);
        
        return this.preparePage({
            loadingMessage: options.loadingMessage || 'Loading Page...',
            loadingSubtitle: options.loadingSubtitle || 'Please wait...',
            loadDataFunctions: loadFunctions,
            onAfterLoad: function(results) {
                console.log('GlobalFormPrep.standardPage onAfterLoad called with:', results);
                if (options.onAfterLoad && typeof options.onAfterLoad === 'function') {
                    options.onAfterLoad(results);
                }
            },
            onError: function(error) {
                console.log('GlobalFormPrep.standardPage onError called with:', error);
                if (options.onError && typeof options.onError === 'function') {
                    options.onError(error);
                }
            },
            hideContentDuringLoad: options.hideContentDuringLoad || false
        });
    }
};
