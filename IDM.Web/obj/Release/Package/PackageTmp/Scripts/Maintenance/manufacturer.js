var ManufacturerModule = (function () {
    var table;

    function initManufacturerData() {
        table = $('#manufacturerTable').DataTable({
            ajax: {
                url: AppUrls.getManufacturer,
                type: 'GET',
                dataSrc: '',
                "error": function (xhr) {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                }
            },
            columns: [
                { data: 'manufacturer_Name' },
                {
                    data: null,
                    render: function (data) {
                        return `
                            <a href='#' class="edit-btn" data-id="${data.id}" title="Edit">
                                 <i class="fas fa-edit text-primary"></i>
                            </a>
                            <a href='#' class=" delete-btn" data-id="${data.id}" title="Delete">
                                <i class="fas fa-trash-alt text-danger"></i>
                            </a>
                        `;
                    }
                }
            ]
        });
    }

    function bindEvents() {
        $('#addManufacturer').click(() => {
            clearForm();
            $('#manufacturerModal').modal('show');
            $('#save-manufacturer').data('mode', 'add').removeData('id');
        });

        $('#manufacturerTable').on('click', '.edit-btn', function () {
            const data = table.row($(this).closest('tr')).data();
            console.log(data.id);
            $('#txtManufacturer').val(data.manufacturer_Name);
            $('#manufacturerModal').modal('show');

            $('#save-manufacturer').data('mode', 'edit').data('id', data.id);
        });

        $('#manufacturerTable').on('click', '.delete-btn', function () {
            const id = $(this).data('id');
            if (!confirm('Are you sure to delete this manufacturer?')) return;

            $.post(AppUrls.deleteManufacturer, { id })
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "Manufacturer deleted successfully.", "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", "Failed to delete manufacturer.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#save-manufacturer').click(() => {
            const mode = $('#save-manufacturer').data('mode');
            const id = $('#save-manufacturer').data('id') || 0;
            const manufacturer = $('#txtManufacturer').val().trim();

            if (!manufacturer) {
                ToastrHelper.notification("error", "Manufacturer are required.", "Validation");
                return;
            }

            const payload = {
                id: id,
                manufacturer_Name: manufacturer
            };

            const url = mode === 'add' ? AppUrls.addManufacturer : AppUrls.editManufacturer;

            $.post(url, payload)
                .done(res => {
                    if (res.success) {
                        $('#manufacturerModal').modal('hide');
                        ToastrHelper.notification("success", `Manufacturer ${mode === 'add' ? 'added' : 'updated'} successfully.`, "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to save manufacturer.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#cancel-manufacturer, #close-manufacturer').click(() => {
            $('#manufacturerModal').modal('hide');
        });
    }

    function clearForm() {
        $('#txtManufacturer').val('');
    }

    return {
        init: function () {
            // Use global form preparation for manufacturer page
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initManufacturerData()
            ], {
                loadingMessage: 'Loading Manufacturer Management...',
                loadingSubtitle: 'Please wait while we load manufacturers and prepare the interface',
                onAfterLoad: function(results) {
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                },
                onError: function(error) {
                    console.error('Failed to prepare manufacturer page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    ManufacturerModule.init();
});
