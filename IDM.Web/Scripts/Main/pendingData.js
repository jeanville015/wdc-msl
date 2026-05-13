var PendingModule = (function () {

    // Main function to get source and destination table and load pending data
    function loadPendingData(page, pageSize) {
        console.log('before table');

        return new Promise((resolve, reject) => { 
            if (pageSize) pendingPageSize = parseInt(pageSize);

            $('#pendingTableContainer').html(
                '<div class="text-center text-muted py-4">Loading...</div>'
            );

            $.ajax({
                url: AppUrls.getPendingData,
                type: 'GET',
                data: {
                    page: page,
                    pageSize: pendingPageSize
                },
                //for the laoding overlay see your logic done on approve logic
                beforeSend: function() {
                    // Show overlay before the request starts
                },
                success: function(dataResponse) {
                    // Check if redirect is needed
                    if (dataResponse && dataResponse.redirect === true) {
                        ToastrHelper.notification("info", dataResponse.message, "Information");
                        setTimeout(function() {
                            window.location.href = dataResponse.redirectUrl;
                        }, 2000);
                        resolve();
                        return;
                    }
                    // Populate the div with the returned HTML (Partial View)
                    $("#partialContent").html(dataResponse);
                    resolve();
                },
                error: function(xhr, status, error) {
                    ToastrHelper.notification("error", "Error fetching pending data: " + error + "; Status: " + status, "Error");
                    reject(error);
                }
            });
        });
    }

    function loadShareDetails(page, pageSize) {
        if (pageSize) sharePageSize = parseInt(pageSize);

        $('#shareModalContent').html(
            '<div class="text-center text-muted py-4">Loading...</div>'
        );

        $.ajax({
            url: AppUrls.getPendingDataDetails,  
            type: 'GET',
            data: {
                deliveryDate: shareParams.deliveryDate,
                receivedDate: shareParams.receivedDate,
                lotNumber: shareParams.lotNumber,
                materialNo: shareParams.materialNo,
                jobNumber: shareParams.jobNumber,
                toolId: shareParams.toolId,
                page: page,
                pageSize: sharePageSize
            },
            success: function (html) {
                $('#GridWControls').html(html);
            },
            error: function (xhr, status, error) {
                $('#GridWControls').html(
                    '<div class="alert alert-danger">Failed to load details. Please try again.</div>'
                );
            }
        });
    }


    // Initialize staging data loading
    function initPendingData() {
        return new Promise((resolve, reject) => {

            loadPendingData(1,10)
                    .then(() => resolve())
                    .catch((error) => reject(error)); 
        });
    } 
     

    function bindEvents() {
        $(document).on('click', '#reject-btn', function (e) {
            e.preventDefault();

            const status = 'REJECTED';
            approvalFunction(status)
        });

        $(document).on('click', '#approve-btn', function (e) {
            e.preventDefault();

            const status = 'PASSED';
            approvalFunction(status)
        });

        // NEW — share icon click
        $(document).on('click', '.share-btn', function (e) {
            e.preventDefault();

            shareParams = {
                deliveryDate: $(this).data('delivery'),
                receivedDate: $(this).data('received'),
                lotNumber: $(this).data('lot'),
                materialNo: $(this).data('material'),
                jobNumber: $(this).data('job'),
                toolId: $(this).data('tool')
            };

            sharePageSize = 10;

            $('#shareModalParams').html(
                '<strong>Delivery:</strong> ' + shareParams.deliveryDate + ' &nbsp;|&nbsp; ' +
                '<strong>Received:</strong> ' + shareParams.receivedDate + ' &nbsp;|&nbsp; ' +
                '<strong>Lot:</strong> ' + shareParams.lotNumber + ' &nbsp;|&nbsp; ' +
                '<strong>Material:</strong> ' + shareParams.materialNo + ' &nbsp;|&nbsp; ' +
                '<strong>Job:</strong> ' + shareParams.jobNumber + ' &nbsp;|&nbsp; ' +
                '<strong>Tool:</strong> ' + shareParams.toolId
            );

            $('#shareModal').modal('show');
            loadShareDetails(1);
        });
    }

    return {
        init: function () {
            // Use global form preparation for Pending data page
            GlobalFormPrep.standardPage([
                // Include pending data initialization as a loading function
                initPendingData()
            ], {
                loadingMessage: 'Loading Pending data...',
                loadingSubtitle: 'Please wait while we load Pending data and prepare the interface',
                onAfterLoad: function (results) {
                    // Bind events after data is fully loaded
                    bindEvents();
                },
                onError: function (error) {
                    console.error('Failed to prepare Pending data page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        },
        // Expose loadPendingData so the partial view pagination can call it globally
        loadPendingData: function (page, pageSize) {
            loadPendingData(page, pageSize);
        },
        // NEW — expose for partial view pagination links
        loadShareDetails: function (page, pageSize) {
            loadShareDetails(page, pageSize);
        }
    };
})();

// Global wrapper so partial view's javascript:loadPendingData(2) can reach it
function loadPendingData(page, pageSize) {
    PendingModule.loadPendingData(page, pageSize);
}
function loadShareDetails(page, pageSize) {
    PendingModule.loadShareDetails(page, pageSize);
}

$(document).ready(function () {
    PendingModule.init(); 

});
