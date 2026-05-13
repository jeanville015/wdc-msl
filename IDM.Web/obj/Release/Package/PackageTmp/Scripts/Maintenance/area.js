var AreaModule = (function () {
    var table;

    function initAreaData() {
        // Return a promise that resolves when DataTable is fully loaded
        return new Promise((resolve, reject) => {
            // Hide the table initially to prevent showing DataTable's internal loading
            $('#areaTable').hide();
            
            table = $('#areaTable').DataTable({
                ajax: {
                    url: AppUrls.getArea,
                    type: 'GET',
                    dataSrc: '',
                    "error": function (xhr) {
                        ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                        reject(xhr);
                    }
                },
                columns: [
                    { data: 'area_Name' },
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
                ],
                // Disable DataTable's internal loading display
                processing: false,
                serverSide: false,
                autoWidth: false,
                initComplete: function() {
                    // Show the table when data is loaded
                    $('#areaTable').fadeIn(300);
                    // DataTable is fully loaded
                    resolve();
                }
            });
        });
    }

    function bindEvents() {
        $('#addArea').click(() => {
            clearForm();
            $('#areaModal').modal('show');
            $('#save-area').data('mode', 'add').removeData('id');
        });

        $('#areaTable').on('click', '.edit-btn', function () {
            const data = table.row($(this).closest('tr')).data();
            console.log(data.id);
            $('#txtArea').val(data.area_Name);
            $('#areaModal').modal('show');

            $('#save-area').data('mode', 'edit').data('id', data.id);
        });

        $('#areaTable').on('click', '.delete-btn', function () {
            const id = $(this).data('id');
            if (!confirm('Are you sure to delete this area?')) return;

            $.post(AppUrls.deleteArea, { id })
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "Area deleted successfully.", "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", "Failed to delete area.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#save-area').click(() => {
            const mode = $('#save-area').data('mode');
            const id = $('#save-area').data('id') || 0;
            const area = $('#txtArea').val().trim();

            if (!area) {
                ToastrHelper.notification("error", "Area are required.", "Validation");
                return;
            }

            const payload = {
                id: id,
                area_Name: area
            };

            const url = mode === 'add' ? AppUrls.addArea : AppUrls.editArea;
            console.log(url);
            console.log(payload);
            $.post(url, payload)
                .done(res => {
                    if (res.success) {
                        $('#areaModal').modal('hide');
                        ToastrHelper.notification("success", `Area ${mode === 'add' ? 'added' : 'updated'} successfully.`, "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to save area.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#cancel-area, #close-area').click(() => {
            $('#areaModal').modal('hide');
        });
    }

    function clearForm() {
        $('#txtArea').val('');
    }

    return {
        init: function () {
            // Use global form preparation for area page
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initAreaData()
            ], {
                loadingMessage: 'Loading Area Management...',
                loadingSubtitle: 'Please wait while we load areas and prepare the interface',
                onAfterLoad: function(results) {
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                },
                onError: function(error) {
                    console.error('Failed to prepare area page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    AreaModule.init();
});
