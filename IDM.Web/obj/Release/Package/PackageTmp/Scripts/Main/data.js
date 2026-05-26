var DataModule = (function () {
    var table;

    function initData() {
        return new Promise((resolve, reject) => {
            table = $('#dataTable').DataTable({
                ajax: {
                    url: AppUrls.getAllData,
                    type: 'GET',
                    dataSrc: '',
                    "error": function (xhr) {
                        ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                        reject(xhr);
                    }
                },
                columns: [
                    { 
                        data: 'deliveryDate',
                        render: function(data) {
                            if (!data) return '';
                            const date = new Date(data);
                            const options = { month: 'short', day: 'numeric', year: 'numeric' };
                            return date.toLocaleDateString('en-US', options);
                        }
                    },
                    { 
                        data: 'receivedDate',
                        render: function(data) {
                            if (!data) return '';
                            const date = new Date(data);
                            const options = { month: 'short', day: 'numeric', year: 'numeric' };
                            return date.toLocaleDateString('en-US', options);
                        }
                    },
                    { data: 'lotNumber' },
                    { data: 'materialNumber' },
                    { data: 'materialName' },
                    { data: 'jobNumber' },
                    { data: 'toolId' },
                    {
                        data: null,
                        render: function (data) {
                            return `
                                <a href='#' class="view-btn" 
                                   data-delivery-date="${data.deliveryDate}"
                                   data-received-date="${data.receivedDate}"
                                   data-material-no="${data.materialNumber}"
                                   data-lot-number="${data.lotNumber}"
                                   data-job-number="${data.jobNumber}"
                                   data-tool-id="${data.toolId}"
                                   title="View Details">
                                     <i class="fas fa-eye text-info"></i>
                                </a>
                                <a href='#' class=" delete-btn" data-id="${data.materialNumber}" title="Delete">
                                    <i class="fas fa-trash-alt text-danger"></i>
                                </a>
                            `;
                        }
                    }
                ],
                // Disable DataTable's internal loading display
                processing: false,
                serverSide: false,
                autoWidth: false,
                initComplete: function () {
                    // Show the table when data is loaded
                    $('#dataTable').fadeIn(300);
                    // DataTable is fully loaded
                    resolve();
                }
            });
        });
    }

    function bindEvents() {
        // View details button click event
        $(document).on('click', '.view-btn', function(e) {
            e.preventDefault();
            const btn = $(this);
            
            const params = {
                deliveryDate: btn.data('delivery-date'),
                receivedDate: btn.data('received-date'),
                materialNo: btn.data('material-no'),
                lotNumber: btn.data('lot-number'),
                jobNumber: btn.data('job-number'),
                toolId: btn.data('tool-id')
            };
            
            // Call the getData endpoint with parameters
            $.ajax({
                url: AppUrls.getData,
                type: 'GET',
                data: params,
                success: function(response) {
                    // Populate modal fields with response data
                    if (response && response.length > 0) {
                        const data = response[0]; // Take first result
                        $('#viewDeliveryDate').text(data.delivery_Date || '');
                        $('#viewReceivedDate').text(data.received_Date || '');
                        $('#viewMaterialNo').text(data.material_No || '');
                        $('#viewLotNumber').text(data.lotNumber || '');
                        $('#viewJobNumber').text(data.job_Number || '');
                        $('#viewToolId').text(data.toolId || '');
                    } else {
                        // Clear fields if no data
                        $('#viewDeliveryDate').text('');
                        $('#viewReceivedDate').text('');
                        $('#viewMaterialNo').text('');
                        $('#viewLotNumber').text('');
                        $('#viewJobNumber').text('');
                        $('#viewToolId').text('');
                    }
                    
                    // Show the modal
                    $('#viewModal').modal('show');
                },
                error: function(xhr) {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "Failed to load data details.", "Error");
                }
            });
        });

        $('#cancel-role, #close-role').click(() => {
            $('#roleModal').modal('hide');
        });

        // View modal close events
        $('#close-view, #close-view-btn').click(() => {
            $('#viewModal').modal('hide');
        });
    }

    function clearForm() {
        $('#txtRole').val('');
    }

    return {
        init: function () {
            // Use global form preparation for role page
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initData()
            ], {
                loadingMessage: 'Loading Incoming Data list...',
                loadingSubtitle: 'Please wait while we load incoming data and prepare the interface',
                onAfterLoad: function (results) {
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                },
                onError: function (error) {
                    console.error('Failed to prepare incoming (data) page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    DataModule.init();
});
