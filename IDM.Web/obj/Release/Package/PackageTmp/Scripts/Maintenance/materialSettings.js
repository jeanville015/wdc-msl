var MaterialSettingsModule = (function () {
    var table;

    function initMaterialSettingsData() {
        // Return a promise that resolves when DataTable is fully loaded
        return new Promise((resolve, reject) => {
            // Hide the table initially to prevent showing DataTable's internal loading
            $('#materialSettingsTable').hide();
            
            table = $('#materialSettingsTable').DataTable({
                ajax: {
                    url: AppUrls.getMaterialSettings,
                    type: 'GET',
                    dataSrc: '',
                    "error": function (xhr) {
                        ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                        reject(xhr);
                    }
                },
                columns: [
                    { data: 'toolTypeName' },
                    { data: 'materialNumber' },
                    { data: 'materialName' },
                    { data: 'settingsName' },
                    { data: 'settingsValue' },
                    {
                        data: null,
                        render: function (data) {
                            return `
                                <a href='#' class="edit-btn" data-id="${data.materialSettingsId}" title="Edit">
                                     <i class="fas fa-edit text-primary"></i>
                                </a>
                                <a href='#' class=" delete-btn" data-id="${data.materialSettingsId}" title="Delete">
                                    <i class="fas fa-trash-alt text-danger"></i>
                                </a>
                            `;
                        }
                    }
                ],
                // Sort by toolTypeName first, then materialNumber
                order: [[0, 'asc'], [1, 'asc']],
                // Color coding by tool type using global helper
                createdRow: RowColorHelper.getCreatedRowCallback('toolTypeName'),
                // Disable DataTable's internal loading display
                processing: false,
                serverSide: false,
                autoWidth: false,
                initComplete: function() {
                    // Show the table when data is loaded
                    $('#materialSettingsTable').fadeIn(300);
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

    function loadMaterial() {
        return $.get(AppUrls.getMaterial)
            .done(data => {
                const $ddl = $('#ddlMaterialNumber').empty().append(`<option value="0">-- Select Material Number --</option>`);
                $.each(data, (i, item) => {
                    $ddl.append(`<option value="${item.id}">${item.material_No}</option>`);
                });
            });
    }

    function bindEvents() {
        $('#addMaterialSettings').click(() => {
            clearForm();
            $('#materialSettingsModal').modal('show');
            $('#save-materialSettings').data('mode', 'add').removeData('id');
        });

        $('#materialSettingsTable').on('click', '.edit-btn', function () {
            const data = table.row($(this).closest('tr')).data();
            $('#txtSettingsName').val(data.settingsName);
            $('#txtSettingsValue').val(data.settingsValue);
            $('#ddlToolType').val(data.toolTypeId);
            $('#ddlMaterialNumber').val(data.materialNumberId);
            $('#materialSettingsModal').modal('show');

            $('#save-materialSettings').data('mode', 'edit').data('id', data.materialSettingsId);
        });

        $('#materialSettingsTable').on('click', '.delete-btn', function () {
            const id = $(this).data('id');
            if (!confirm('Are you sure to delete this material settings?')) return;

            $.post(AppUrls.deleteMaterialSettings, { id })
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "material settings deleted successfully.", "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", "Failed to delete material settings.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#save-materialSettings').click(() => {
            const mode = $('#save-materialSettings').data('mode');
            const materialSettingsId = $('#save-materialSettings').data('id') || 0;
            const settingsValue = $('#txtSettingsValue').val().trim();
            const settingsName = $('#txtSettingsName').val().trim();
            const toolTypeId = $('#ddlToolType').val();
            const materialNumberId = $('#ddlMaterialNumber').val();

            if (!settingsValue) {
                ToastrHelper.notification("error", "Settings Value are required.", "Validation");
                return;
            }
            if (!settingsName) {
                ToastrHelper.notification("error", "Setting Name are required.", "Validation");
                return;
            }

            if (!toolTypeId) {
                ToastrHelper.notification("error", "Tool Type is required.", "Validation");
                return;
            }

            if (!materialNumberId) {
                ToastrHelper.notification("error", "Material Number is required.", "Validation");
                return;
            }

            const payload = {
                materialSettingsId: materialSettingsId,
                settingsValue: settingsValue,
                settingsName: settingsName,
                materialNumberId: materialNumberId,
                toolTypeId: toolTypeId
            };

            const url = mode === 'add' ? AppUrls.addMaterialSettings : AppUrls.editMaterialSettings;

            $.post(url, payload)
                .done(res => {
                    if (res.success) {
                        $('#materialSettingsModal').modal('hide');
                        ToastrHelper.notification("success", `Material Settings ${mode === 'add' ? 'added' : 'updated'} successfully.`, "Success");
                        clearForm();
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to save material settings.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#cancel-materialSettings, #close-materialSettings').click(() => {
            $('#materialSettingsModal').modal('hide');
        });
    }

    function loadAllDropdowns() {
        return Promise.all([
            clearForm(),
            loadToolType(),
            loadMaterial(),
        ]);
    }

    function clearForm() {
        $('#txtSettingsValue').val('');
        $('#txtSettingsName').val('');
        $('#ddlToolType').val(0);
        $('#ddlMaterialNumber').val(0);
    }

    return {
        init: function () {
            // Use global form preparation for tool page
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initMaterialSettingsData(),
                loadAllDropdowns() 
            ], {
                loadingMessage: 'Loading Material Settings Management...',
                loadingSubtitle: 'Please wait while we load material settings and prepare the interface',
                onAfterLoad: function(results) {
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                },
                onError: function(error) {
                    console.error('Failed to prepare material settings page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    MaterialSettingsModule.init();
});
