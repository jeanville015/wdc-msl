// Example: How to use Global Loading and Form Preparation on any page
// Copy this code to any page's JavaScript section

$(function () {
    // Example 1: Basic page preparation
    GlobalFormPrep.standardPage([
        // Add your data loading functions here
        // loadUsers(),
        // loadRoles(),
        // loadDepartments()
    ], {
        loadingMessage: 'Loading User Management...',
        loadingSubtitle: 'Please wait while we prepare your data',
        onAfterLoad: function(results) {
            // Setup your page after data loads
            console.log('Page data loaded successfully!');
            // setupUserTable();
            // initializeFilters();
        }
    });
    
    // Example 2: Custom page preparation with more control
    /*
    GlobalFormPrep.preparePage({
        loadingMessage: 'Loading Dashboard...',
        loadingSubtitle: 'Analyzing your data, please wait...',
        loadDataFunctions: [
            loadDashboardData(),
            loadUserPermissions(),
            loadSettings()
        ],
        onBeforeLoad: function() {
            console.log('Starting page preparation...');
        },
        onAfterLoad: function(results) {
            const [dashboardData, permissions, settings] = results;
            setupDashboard(dashboardData, permissions, settings);
        },
        onError: function(error) {
            console.error('Custom error handling:', error);
        },
        hideContentDuringLoad: true
    });
    */
    
    // Example 3: Simple loading overlay for specific actions
    /*
    $('#saveButton').on('click', function() {
        GlobalLoading.showWithFormDisable('Saving...', 'Processing your request');
        
        $.ajax({
            url: '/YourEndpoint/Save',
            type: 'POST',
            data: formData,
            success: function(response) {
                // Handle success
            },
            error: function(xhr, status, error) {
                // Handle error
            },
            complete: function() {
                GlobalLoading.hideWithFormEnable();
            }
        });
    });
    */
});

// Example data loading functions (replace with your actual functions)
function loadUsers() {
    return $.get('/Api/Users').promise();
}

function loadRoles() {
    return $.get('/Api/Roles').promise();
}

function loadDepartments() {
    return $.get('/Api/Departments').promise();
}

function loadDashboardData() {
    return $.get('/Api/Dashboard').promise();
}

function loadUserPermissions() {
    return $.get('/Api/UserPermissions').promise();
}

function loadSettings() {
    return $.get('/Api/Settings').promise();
}
