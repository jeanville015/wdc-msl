var ToolModule = (function () {
    var table;

    function initToolData() {
        // Return a promise that resolves when DataTable is fully loaded
        return new Promise((resolve, reject) => {
            // Hide the table initially to prevent showing DataTable's internal loading
            $('#toolTable').hide();
            
            table = $('#toolTable').DataTable({
                ajax: {
                    url: AppUrls.getTool,
                    type: 'GET',
                    dataSrc: '',
                    "error": function (xhr) {
                        ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                        reject(xhr);
                    }
                },
                columns: [
                    { data: 'toolTypeName' },
                    { data: 'toolName' },
                    {
                        data: null,
                        render: function (data) {
                            return `
                                <a href='#' class="edit-btn" data-id="${data.toolId}" title="Edit">
                                     <i class="fas fa-edit text-primary"></i>
                                </a>
                                <a href='#' class=" delete-btn" data-id="${data.toolId}" title="Delete">
                                    <i class="fas fa-trash-alt text-danger"></i>
                                </a>
                            `;
                        }
                    }
                ],
                // Sort by toolTypeName first, then toolName
                order: [[0, 'asc'], [1, 'asc']],
                // Color coding by tool type using global helper
                createdRow: RowColorHelper.getCreatedRowCallback('toolTypeName'),
                // Disable DataTable's internal loading display
                processing: false,
                serverSide: false,
                autoWidth: false,
                initComplete: function() {
                    // Show the table when data is loaded
                    $('#toolTable').fadeIn(300);
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
        $('#addTool').click(() => {
            clearForm();
            $('#toolModal').modal('show');
            $('#save-tool').data('mode', 'add').removeData('id');
        });

        $('#toolTable').on('click', '.edit-btn', function () {
            const data = table.row($(this).closest('tr')).data();
            $('#txtToolName').val(data.toolName);
            $('#ddlToolType').val(data.toolTypeId);
            $('#toolModal').modal('show');

            $('#save-tool').data('mode', 'edit').data('id', data.toolId);
        });

        $('#toolTable').on('click', '.delete-btn', function () {
            const id = $(this).data('id');
            if (!confirm('Are you sure to delete this tool?')) return;

            $.post(AppUrls.deleteTool, { id })
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "tool deleted successfully.", "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", "Failed to delete tool.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#save-tool').click(() => {
            const mode = $('#save-tool').data('mode');
            const toolId = $('#save-tool').data('id') || 0;
            const toolName = $('#txtToolName').val().trim();
            const toolTypeId = $('#ddlToolType').val();

            if (!toolName) {
                ToastrHelper.notification("error", "Tool Name are required.", "Validation");
                return;
            }

            if (!toolTypeId) {
                ToastrHelper.notification("error", "Tool Type is required.", "Validation");
                return;
            }

            const payload = {
                toolId: toolId,
                toolName: toolName,
                toolTypeId: toolTypeId
            };

            const url = mode === 'add' ? AppUrls.addTool : AppUrls.editTool;

            $.post(url, payload)
                .done(res => {
                    if (res.success) {
                        $('#toolModal').modal('hide');
                        ToastrHelper.notification("success", `Tool ${mode === 'add' ? 'added' : 'updated'} successfully.`, "Success");
                        clearForm();
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to save tool.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#cancel-tool, #close-tool').click(() => {
            $('#toolModal').modal('hide');
        });
    }

    function loadAllDropdowns() {
        return Promise.all([
            clearForm(),
            loadToolType(),
        ]);
    }

    function clearForm() {
        $('#txtToolName').val('');
        $('#ddlToolType').val(0);
    }

    return {
        init: function () {
            // Use global form preparation for tool page
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initToolData(),
                loadAllDropdowns() 
            ], {
                loadingMessage: 'Loading Tool Management...',
                loadingSubtitle: 'Please wait while we load tools and prepare the interface',
                onAfterLoad: function(results) {
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                },
                onError: function(error) {
                    console.error('Failed to prepare tool page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    ToolModule.init();
});
