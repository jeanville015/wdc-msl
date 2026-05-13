var SupplierModule = (function () {
    var table;

    function initSupplierData() {
        table = $('#supplierTable').DataTable({
            ajax: {
                url: AppUrls.getSupplier,
                type: 'GET',
                dataSrc: '',
                "error": function (xhr) {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                }
            },
            columns: [
                { data: 'supplier_Name' },
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
        $('#addSupplier').click(() => {
            clearForm();
            $('#supplierModal').modal('show');
            $('#save-supplier').data('mode', 'add').removeData('id');
        });

        $('#supplierTable').on('click', '.edit-btn', function () {
            const data = table.row($(this).closest('tr')).data();
            console.log(data.id);
            $('#txtSupplier').val(data.supplier_Name);
            $('#supplierModal').modal('show');

            $('#save-supplier').data('mode', 'edit').data('id', data.id);
        });

        $('#supplierTable').on('click', '.delete-btn', function () {
            const id = $(this).data('id');
            if (!confirm('Are you sure to delete this supplier?')) return;

            $.post(AppUrls.deleteSupplier, { id })
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "Supplier deleted successfully.", "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", "Failed to delete supplier.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#save-supplier').click(() => {
            const mode = $('#save-supplier').data('mode');
            const id = $('#save-supplier').data('id') || 0;
            const supplier = $('#txtSupplier').val().trim();

            if (!supplier) {
                ToastrHelper.notification("error", "Supplier are required.", "Validation");
                return;
            }

            const payload = {
                id: id,
                supplier_Name: supplier
            };

            const url = mode === 'add' ? AppUrls.addSupplier : AppUrls.editSupplier;

            $.post(url, payload)
                .done(res => {
                    if (res.success) {
                        $('#supplierModal').modal('hide');
                        ToastrHelper.notification("success", `Supplier ${mode === 'add' ? 'added' : 'updated'} successfully.`, "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to save supplier.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#cancel-supplier, #close-supplier').click(() => {
            $('#supplierModal').modal('hide');
        });
    }

    function clearForm() {
        $('#txtSupplier').val('');
    }

    return {
        init: function () {
            // Use global form preparation for supplier page
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initSupplierData()
            ], {
                loadingMessage: 'Loading Supplier Management...',
                loadingSubtitle: 'Please wait while we load suppliers and prepare the interface',
                onAfterLoad: function(results) {
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                },
                onError: function(error) {
                    console.error('Failed to prepare supplier page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    SupplierModule.init();
});
