var ToolTypeModule = (function () {
    var table;

    function initToolTypeData() {
        // Return a promise that resolves when DataTable is fully loaded
        return new Promise((resolve, reject) => {
            // Hide the table initially to prevent showing DataTable's internal loading
            $('#toolTypeTable').hide();
            
            table = $('#toolTypeTable').DataTable({
                ajax: {
                    url: AppUrls.getToolType,
                    type: 'GET',
                    dataSrc: '',
                    "error": function (xhr) {
                        ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                        reject(xhr);
                    }
                },
                columns: [
                    { data: 'toolTypeName' },
                    { 
                        data: 'requireApproval',
                        render: function (data) {
                            if (data === '1') {
                                return '<i class="fas fa-check text-success"></i>';
                            } else {
                                return '<i class="fas fa-times text-danger"></i>';
                            }
                        }
                    },
                    {
                        data: null,
                        render: function (data) {
                            return `
                                <a href='#' class="edit-btn" data-id="${data.toolTypeId}" title="Edit">
                                     <i class="fas fa-edit text-primary"></i>
                                </a>
                                <a href='#' class=" delete-btn" data-id="${data.toolTypeId}" title="Delete">
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
                    $('#toolTypeTable').fadeIn(300);
                    // DataTable is fully loaded
                    resolve();
                }
            });
        });
    }

    function bindEvents() {
        $('#addToolType').click(() => {
            clearForm();
            $('#toolTypeModal').modal('show');
            $('#save-toolType').data('mode', 'add').removeData('id');
        });

        $('#toolTypeTable').on('click', '.edit-btn', function () {
            const data = table.row($(this).closest('tr')).data();
            $('#txtToolTypeName').val(data.toolTypeName);
            // Set checkbox: if requireApproval is 1, check it; otherwise uncheck
            $('#chkRequireApproval').prop('checked', data.requireApproval === '1');
            $('#toolTypeModal').modal('show');

            $('#save-toolType').data('mode', 'edit').data('id', data.toolTypeId);
        });

        $('#toolTypeTable').on('click', '.delete-btn', function () {
            const id = $(this).data('id');
            if (!confirm('Are you sure to delete this tool type?')) return;

            $.post(AppUrls.deleteToolType, { id })
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "tool type deleted successfully.", "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", "Failed to delete tool type.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#save-toolType').click(() => {
            const mode = $('#save-toolType').data('mode');
            const toolTypeId = $('#save-toolType').data('id') || 0;
            const toolTypeName = $('#txtToolTypeName').val().trim();
            // Get checkbox value: if checked = 1, else = 0
            const requireApproval = $('#chkRequireApproval').is(':checked') ? '1' : '0';

            if (!toolTypeName) {
                ToastrHelper.notification("error", "Tool Name are required.", "Validation");
                return;
            }

            const payload = {
                toolTypeId: toolTypeId,
                toolTypeName: toolTypeName,
                requireApproval: requireApproval
            };

            const url = mode === 'add' ? AppUrls.addToolType : AppUrls.editToolType;

            $.post(url, payload)
                .done(res => {
                    if (res.success) {
                        $('#toolTypeModal').modal('hide');
                        ToastrHelper.notification("success", `Tool Type ${mode === 'add' ? 'added' : 'updated'} successfully.`, "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to save tool type.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#cancel-toolType, #close-toolType').click(() => {
            $('#toolTypeModal').modal('hide');
        });
    }

    function clearForm() {
        $('#txtToolTypeName').val('');
        $('#chkRequireApproval').prop('checked', false);
    }

    return {
        init: function () {
            // Use global form preparation for tool page
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initToolTypeData()
            ], {
                loadingMessage: 'Loading Tool Type Management...',
                loadingSubtitle: 'Please wait while we load tool types and prepare the interface',
                onAfterLoad: function(results) {
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                },
                onError: function(error) {
                    console.error('Failed to prepare tool type page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    ToolTypeModule.init();
});
