var TestingSiteModule = (function () {
    var table;

    function initTestingData() {
        table = $('#testingSiteTable').DataTable({
            ajax: {
                url: AppUrls.getSite,
                type: 'GET',
                dataSrc: '',
                "error": function (xhr) {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                }
            },
            columns: [
                { data: 'site_Name' },
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
        $('#addTestingSite').click(() => {
            clearForm();
            $('#testingSiteModal').modal('show');
            $('#save-testingSite').data('mode', 'add').removeData('id');
        });

        $('#testingSiteTable').on('click', '.edit-btn', function () {
            const data = table.row($(this).closest('tr')).data();
            console.log(data.id);
            $('#txtTestingSite').val(data.site_Name);
            $('#testingSiteModal').modal('show');

            $('#save-testingSite').data('mode', 'edit').data('id', data.id);
        });

        $('#testingSiteTable').on('click', '.delete-btn', function () {
            const id = $(this).data('id');
            if (!confirm('Are you sure to delete this testing site?')) return;

            $.post(AppUrls.deleteSite, { id })
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "Testing Site deleted successfully.", "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", "Failed to delete testing site.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#save-testingSite').click(() => {
            const mode = $('#save-testingSite').data('mode');
            const id = $('#save-testingSite').data('id') || 0;
            const site = $('#txtTestingSite').val().trim();

            if (!site) {
                ToastrHelper.notification("error", "Testing Site are required.", "Validation");
                return;
            }

            const payload = {
                id: id,
                site_Name: site
            };

            const url = mode === 'add' ? AppUrls.addSite : AppUrls.editSite;

            $.post(url, payload)
                .done(res => {
                    if (res.success) {
                        $('#testingSiteModal').modal('hide');
                        ToastrHelper.notification("success", `Testing Site ${mode === 'add' ? 'added' : 'updated'} successfully.`, "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to save testing site.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#cancel-testingSite, #close-testingSite').click(() => {
            $('#testingSiteModal').modal('hide');
        });
    }

    function clearForm() {
        $('#txtTestingSite').val('');
    }

    return {
        init: function () {
            // Use global form preparation for testing site page
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initTestingData()
            ], {
                loadingMessage: 'Loading Testing Site Management...',
                loadingSubtitle: 'Please wait while we load testing sites and prepare the interface',
                onAfterLoad: function(results) {
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                },
                onError: function(error) {
                    console.error('Failed to prepare testing site page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    TestingSiteModule.init();
});
