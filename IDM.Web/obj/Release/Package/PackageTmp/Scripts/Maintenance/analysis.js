var AnalysisModule = (function () {
    var table;

    function initAnalysisData() {
        // Return a promise that resolves when DataTable is fully loaded
        return new Promise((resolve, reject) => {
            // Hide the table initially to prevent showing DataTable's internal loading
            $('#analysisTable').hide();
            
            table = $('#analysisTable').DataTable({
                ajax: {
                    url: AppUrls.getAnalysis,
                    type: 'GET',
                    dataSrc: '',
                    "error": function (xhr) {
                        ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                        reject(xhr);
                    }
                },
                columns: [
                    { data: 'toolTypeName' },
                    { data: 'analysisName' },
                    { data: 'sourceTable' },
                    { data: 'destinationTable' },
                    {
                        data: null,
                        render: function (data) {
                            return `
                                <a href='#' class="edit-btn" data-id="${data.analysisId}" title="Edit">
                                     <i class="fas fa-edit text-primary"></i>
                                </a>
                                <a href='#' class=" delete-btn" data-id="${data.analysisId}" title="Delete">
                                    <i class="fas fa-trash-alt text-danger"></i>
                                </a>
                            `;
                        }
                    }
                ],
                // Sort by toolTypeName first, then analysisName
                order: [[0, 'asc'], [1, 'asc']],
                // Color coding by tool type using global helper
                createdRow: RowColorHelper.getCreatedRowCallback('toolTypeName'),
                // Disable DataTable's internal loading display
                processing: false,
                serverSide: false,
                autoWidth: false,
                initComplete: function() {
                    // Show the table when data is loaded
                    $('#analysisTable').fadeIn(300);
                    // DataTable is fully loaded
                    resolve();
                }
            });
        });
    }

    function loadToolType() {
        return $.get(AppUrls.getToolType)
            .done(data => {
                const $ddl = $('#ddlToolType').empty().append(`<option value="0">-- Select Tool Type --</option>`);
                $.each(data, (i, item) => {
                    $ddl.append(`<option value="${item.toolTypeId}">${item.toolTypeName}</option>`);
                });
            });
    }

    function bindEvents() {
        $('#addAnalysis').click(() => {
            clearForm();
            $('#analysisModal').modal('show');
            $('#save-analysis').data('mode', 'add').removeData('id');
        });

        $('#analysisTable').on('click', '.edit-btn', function () {
            const data = table.row($(this).closest('tr')).data();
            $('#txtAnalysisName').val(data.analysisName);
            $('#txtSourceTable').val(data.sourceTable);
            $('#txtDestinationTable').val(data.destinationTable);
            $('#ddlToolType').val(data.toolTypeId);
            $('#analysisModal').modal('show');

            $('#save-analysis').data('mode', 'edit').data('id', data.analysisId);
        });

        $('#analysisTable').on('click', '.delete-btn', function () {
            const id = $(this).data('id');
            if (!confirm('Are you sure to delete this analysis?')) return;

            $.post(AppUrls.deleteAnalysis, { id })
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "analysis deleted successfully.", "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", "Failed to delete analysis.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#save-analysis').click(() => {
            const mode = $('#save-analysis').data('mode');
            const analysisId = $('#save-analysis').data('id') || 0;
            const analysisName = $('#txtAnalysisName').val().trim();
            const sourceTable = $('#txtSourceTable').val().trim();
            const destinationTable = $('#txtDestinationTable').val().trim();
            const toolTypeId = $('#ddlToolType').val();

            if (!analysisName) {
                ToastrHelper.notification("error", "Analysis Name are required.", "Validation");
                return;
            }

            if (!toolTypeId) {
                ToastrHelper.notification("error", "Tool Type is required.", "Validation");
                return;
            }

            if (!sourceTable) {
                ToastrHelper.notification("error", "Source Table is required.", "Validation");
                return;
            }

            if (!destinationTable) {
                ToastrHelper.notification("error", "Destination Table is required.", "Validation");
                return;
            }

            const payload = {
                analysisId: analysisId,
                analysisName: analysisName,
                toolTypeId: toolTypeId,
                sourceTable: sourceTable,
                destinationTable: destinationTable
            };

            const url = mode === 'add' ? AppUrls.addAnalysis : AppUrls.editAnalysis;

            $.post(url, payload)
                .done(res => {
                    if (res.success) {
                        $('#analysisModal').modal('hide');
                        ToastrHelper.notification("success", `Analysis ${mode === 'add' ? 'added' : 'updated'} successfully.`, "Success");
                        clearForm();
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to save analysis.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#cancel-analysis, #close-analysis').click(() => {
            $('#analysisModal').modal('hide');
        });
    }

    function loadAllDropdowns() {
        return Promise.all([
            clearForm(),
            loadToolType(),
        ]);
    }

    function clearForm() {
        $('#txtAnalysisName').val('');
        $('#txtSourceTable').val('');
        $('#txtDestinationTable').val('');
        $('#ddlToolType').val(0);
    }

    return {
        init: function () {
            // Use global form preparation for analysis page
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initAnalysisData(),
                loadAllDropdowns() 
            ], {
                loadingMessage: 'Loading Analysis Management...',
                loadingSubtitle: 'Please wait while we load analysis and prepare the interface',
                onAfterLoad: function(results) {
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                },
                onError: function(error) {
                    console.error('Failed to prepare analysis page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    AnalysisModule.init();
});
