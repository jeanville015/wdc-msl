var ParameterModule = (function () {
    var table;

    function initParameterData() {
        table = $('#parameterTable').DataTable({
            ajax: {
                url: AppUrls.getParameter,
                type: 'GET',
                dataSrc: '',
                "error": function (xhr) {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                }
            },
            columns: [
                { data: 'parameter_Name' },
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
        $('#addParameter').click(() => {
            clearForm();
            $('#parameterModal').modal('show');
            $('#save-parameter').data('mode', 'add').removeData('id');
        });

        $('#parameterTable').on('click', '.edit-btn', function () {
            const data = table.row($(this).closest('tr')).data();
            console.log(data.id);
            $('#txtParameter').val(data.parameter_Name);
            $('#parameterModal').modal('show');

            $('#save-parameter').data('mode', 'edit').data('id', data.id);
        });

        $('#parameterTable').on('click', '.delete-btn', function () {
            const id = $(this).data('id');
            if (!confirm('Are you sure to delete this parameter?')) return;

            $.post(AppUrls.deleteParameter, { id })
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "Parameter deleted successfully.", "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", "Failed to delete Parameter.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#save-parameter').click(() => {
            const mode = $('#save-parameter').data('mode');
            const id = $('#save-parameter').data('id') || 0;
            const parameter = $('#txtParameter').val().trim();

            if (!parameter) {
                ToastrHelper.notification("error", "Parameter are required.", "Validation");
                return;
            }

            const payload = {
                id: id,
                parameter_Name: parameter
            };

            const url = mode === 'add' ? AppUrls.addParameter : AppUrls.editParameter;

            $.post(url, payload)
                .done(res => {
                    if (res.success) {
                        $('#parameterModal').modal('hide');
                        ToastrHelper.notification("success", `Parameter ${mode === 'add' ? 'added' : 'updated'} successfully.`, "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to save parameter.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#cancel-parameter, #close-parameter').click(() => {
            $('#parameterModal').modal('hide');
        });
    }

    function clearForm() {
        $('#txtParameter').val('');
    }

    return {
        init: function () {
            // Use global form preparation for parameter page
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initParameterData()
            ], {
                loadingMessage: 'Loading Parameter Management...',
                loadingSubtitle: 'Please wait while we load parameters and prepare the interface',
                onAfterLoad: function(results) {
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                },
                onError: function(error) {
                    console.error('Failed to prepare parameter page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    ParameterModule.init();
});
