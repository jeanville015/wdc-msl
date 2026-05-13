var RoleModule = (function () {
    var table;

    function initRoleData() {
        // Return a promise that resolves when DataTable is fully loaded
        return new Promise((resolve, reject) => {
            // Hide the table initially to prevent showing DataTable's internal loading
            $('#roleTable').hide();
            
            table = $('#roleTable').DataTable({
                ajax: {
                    url: AppUrls.getRole,
                    type: 'GET',
                    dataSrc: '',
                    "error": function (xhr) {
                        ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                        reject(xhr);
                    }
                },
                columns: [
                    { data: 'user_Role' },
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
                    $('#roleTable').fadeIn(300);
                    // DataTable is fully loaded
                    resolve();
                }
            });
        });
    }

    function bindEvents() {
        $('#addRole').click(() => {
            clearForm();
            $('#roleModal').modal('show');
            $('#save-role').data('mode', 'add').removeData('id');
        });

        $('#roleTable').on('click', '.edit-btn', function () {
            const data = table.row($(this).closest('tr')).data();
            console.log(data.id);
            $('#txtRole').val(data.user_Role);
            $('#roleModal').modal('show');

            $('#save-role').data('mode', 'edit').data('id', data.id);
        });

        $('#roleTable').on('click', '.delete-btn', function () {
            const id = $(this).data('id');
            if (!confirm('Are you sure to delete this role?')) return;

            // Show loading during delete
            GlobalLoading.showWithFormDisable('Deleting...', 'Please wait while we delete the role');

            $.post(AppUrls.deleteRole, { id })
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "Role deleted successfully.", "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", "Failed to delete role.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                })
                .always(() => {
                    GlobalLoading.hideWithFormEnable();
                });
        });

        $('#save-role').click(() => {
            const mode = $('#save-role').data('mode');
            const id = $('#save-role').data('id') || 0;
            const role = $('#txtRole').val().trim();

            if (!role) {
                ToastrHelper.notification("error", "Role are required.", "Validation");
                return;
            }

            const payload = {
                id: id,
                user_Role: role
            };

            const url = mode === 'add' ? AppUrls.addRole : AppUrls.editRole;

            // Show loading during save
            GlobalLoading.showWithFormDisable('Saving...', `Please wait while we ${mode} the role`);

            $.post(url, payload)
                .done(res => {
                    if (res.success) {
                        $('#roleModal').modal('hide');
                        ToastrHelper.notification("success", `Role ${mode === 'add' ? 'added' : 'updated'} successfully.`, "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to save role.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                })
                .always(() => {
                    GlobalLoading.hideWithFormEnable();
                });
        });

        $('#cancel-role, #close-role').click(() => {
            $('#roleModal').modal('hide');
        });
    }

    function clearForm() {
        $('#txtRole').val('');
    }

    return {
        init: function () {
            // Use global form preparation for role page
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initRoleData()
            ], {
                loadingMessage: 'Loading Role Management...',
                loadingSubtitle: 'Please wait while we load roles and prepare the interface',
                onAfterLoad: function(results) {
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                },
                onError: function(error) {
                    console.error('Failed to prepare role page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    RoleModule.init();
});
