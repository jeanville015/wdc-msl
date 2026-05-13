var UomModule = (function () {
    var table;

    function initUomData() {
        table = $('#uomTable').DataTable({
            ajax: {
                url: AppUrls.getUom,
                type: 'GET',
                dataSrc: '',
                "error": function (xhr) {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                }
            },
            columns: [
                { data: 'uom_Name' },
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
        $('#addUom').click(() => {
            clearForm();
            $('#uomModal').modal('show');
            $('#save-uom').data('mode', 'add').removeData('id');
        });

        $('#uomTable').on('click', '.edit-btn', function () {
            const data = table.row($(this).closest('tr')).data();
            console.log(data.id);
            $('#txtUom').val(data.uom_Name);
            $('#uomModal').modal('show');

            $('#save-uom').data('mode', 'edit').data('id', data.id);
        });

        $('#uomTable').on('click', '.delete-btn', function () {
            const id = $(this).data('id');
            if (!confirm('Are you sure to delete this uom?')) return;

            $.post(AppUrls.deleteUom, { id })
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "Uom deleted successfully.", "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", "Failed to delete uom.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#save-uom').click(() => {
            const mode = $('#save-uom').data('mode');
            const id = $('#save-uom').data('id') || 0;
            const uom = $('#txtUom').val().trim();

            if (!uom) {
                ToastrHelper.notification("error", "Uom are required.", "Validation");
                return;
            }

            const payload = {
                id: id,
                uom_Name: uom
            };

            const url = mode === 'add' ? AppUrls.addUom : AppUrls.editUom;

            $.post(url, payload)
                .done(res => {
                    if (res.success) {
                        $('#uomModal').modal('hide');
                        ToastrHelper.notification("success", `Uom ${mode === 'add' ? 'added' : 'updated'} successfully.`, "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to save uom.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#cancel-uom, #close-uom').click(() => {
            $('#uomModal').modal('hide');
        });
    }

    function clearForm() {
        $('#txtUom').val('');
    }

    return {
        init: function () {
            // Use global form preparation for UOM page
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initUomData()
            ], {
                loadingMessage: 'Loading UOM Management...',
                loadingSubtitle: 'Please wait while we load units of measure and prepare the interface',
                onAfterLoad: function(results) {
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                },
                onError: function(error) {
                    console.error('Failed to prepare UOM page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    UomModule.init();
});
