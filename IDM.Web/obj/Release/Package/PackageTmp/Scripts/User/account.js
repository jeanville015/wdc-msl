var AccountModule = (function () {
    var table;

    function initAccountData() {
        console.log('initAccountData() called');
        
        // Return a promise that resolves when DataTable is fully loaded
        return new Promise((resolve, reject) => {
            console.log('Creating promise for DataTable initialization');
            
            // Hide the table initially to prevent showing DataTable's internal loading
            $('#accountTable').hide();
            console.log('Table hidden initially');
            
            table = $('#accountTable').DataTable({
                ajax: {
                    url: AppUrls.getAccount,
                    type: 'GET',
                    dataSrc: '',
                    "error": function (xhr) {
                        console.error('DataTable AJAX error:', xhr);
                        ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                        reject(xhr);
                    }
                },
                columns: [
                    { data: 'user_Id' },
                    { data: 'adName' },
                    { data: 'user_Role' },
                    { data: 'user_Group' },
                    { data: 'user_Analysis_String_Group' },
                    {
                        data: 'lastLoginTs',
                        render: function (data, type, row) {
                            if (!data) return '';
                            const date = new Date(data);
                            return formatDate(date);
                        }
                    },
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
                    console.log('DataTable initComplete called');
                    // Show the table when data is loaded
                    $('#accountTable').fadeIn(300);
                    console.log('Table shown with data');
                    // DataTable is fully loaded
                    resolve();
                }
            });
            
            console.log('DataTable initialization started');
        });
    }

    function formatDate(date) {
        const options = {
            month: 'short',
            day: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
            hour12: true
        };

        const formatter = new Intl.DateTimeFormat('en-US', options);
        const parts = formatter.formatToParts(date);

        const map = {};
        parts.forEach(p => map[p.type] = p.value);

        return `${map.month}. ${map.day}, ${map.year} - ${map.hour}:${map.minute} ${map.dayPeriod}`;
    }


    function loadRoles() {
        return $.get(AppUrls.getRoles)
            .done(data => {
                const $ddl = $('#ddlRole').empty().append(`<option value="0">-- Select Role --</option>`);
                $.each(data, (i, item) => {
                    $ddl.append(`<option value="${item.id}">${item.user_Role}</option>`);
                });
            });
    }
    function loadGroups(preselectedValue) {
        //- OLD CODE--------------------------------------------------------------------------------------
        //return $.get(AppUrls.getGroups)
        //    .done(data => {
        //        const $ddl = $('#ddlGroup').empty().append(`<option value="0">-- Select Role --</option>`);
        //        $.each(data, (i, item) => {
        //            $ddl.append(`<option value="${item}">${item}</option>`);
        //        });
        //    });
        //------------------------------------------------------------------------------------------------

        $.ajax({
            url: AppUrls.getGroups,
            type: 'GET',
            success: function (data) {
                var $select = $('#ddlGroup');

                // Clear existing options, keep the placeholder
                $select.empty();
                $select.append($('<option>').val('').text('-- Select Group --'));

                // data is List<string> from controller
                // each item is both the label and the value
                $.each(data, function (i, item) {
                    $select.append(
                        $('<option>').val(item).text(item)
                    );
                });

                 //Pre-select if value is provided (Edit mode)
                if (preselectedValue) {
                    $select.val(preselectedValue);
                } else {
                    //$select.val(''); // reset to placeholder (Add mode)
                }
            },
            error: function () {
                alert('Failed to load Group list.');
            }
        });

    }

    function loadAnalysis(preselectedIds) {
        $('#selectAnalysis').select2({
            placeholder: '-- Select Analysis --',
            allowClear: true,
            width: '100%',
            dropdownParent: $('body') // ensures dropdown appends to body
        });

        $.ajax({
            url: AppUrls.getAnalysis,
            type: 'GET',
            success: function (data) {
                var $select = $('#selectAnalysis');

                // Clear existing options before re-populating
                $select.empty();

                $.each(data, function (i, item) {
                    var option = new Option(
                        item.analysisName,  // display label
                        item.analysisId,    // value (passed to controller)
                        false,
                        false
                    );
                    $select.append(option);
                    //console.log('ID:'+item.analysisId+'; VAL:'+item.analysisName);
                });

                // Trigger select2 to re-render the updated option list
                $select.trigger('change');

                // Pre-select if IDs are provided (Edit mode)
                if (preselectedIds && preselectedIds.length > 0) {
                    $select.val(preselectedIds).trigger('change');
                } else {
                    $select.val(null).trigger('change'); // clear for Add mode
                }
            },
            error: function () {
                alert('Failed to load analysis list.');
            }
        });
    }

    function bindEvents() {
        $('#addAccount').click(() => {
            clearForm();
            $('#accountModal').modal('show');
            $('#save-account').data('mode', 'add').removeData('id');

            // Load dropdowns in background without blocking modal
            loadAnalysis();

            loadGroups(null).catch(error => {
                console.error('Error loading dropdowns:', error);
                ToastrHelper.notification("error", "Failed to load group options", "Error");
            });

            loadRoles().catch(error => {
                console.error('Error loading dropdowns:', error);
                ToastrHelper.notification("error", "Failed to load group options", "Error");
            });

        });

        $('#accountTable').on('click', '.edit-btn', function () {
            const data = table.row($(this).closest('tr')).data();
            console.log(data.id);
            
            // Show modal immediately
            $('#txtAccount').val(data.user_Id);
            $('#accountModal').modal('show');
            $('#save-account').data('mode', 'edit').data('id', data.id);
            
            // Load dropdowns and set role in background
            loadRoles().then(() => {
                $('#ddlRole').val(data.roleId);
            }).catch(error => {
                console.error('Error loading dropdowns:', error);
                ToastrHelper.notification("error", "Failed to load role options", "Error");
            });
            loadGroups(data.user_Group);
            console.log('User-Group:   ' + data.user_Group);

            // Load dropdowns in background without blocking modal
            loadAnalysis(data.user_Analysis);

            //loadGroups(data.user_Group).then(() => {
            //    $('#ddlGroup').val(data.user_Group);
                //console.log('User-Group:   ' + data.user_Group);
            //}).catch(error => {
            //    console.error('Error loading dropdowns:', error);
            //    ToastrHelper.notification("error", "Failed to load group options", "Error");
            //});

        });

        $('#accountTable').on('click', '.delete-btn', function () {
            const id = $(this).data('id');
            if (!confirm('Are you sure to delete this account?')) return;

            // Show loading during delete
            GlobalLoading.showWithFormDisable('Deleting...', 'Please wait while we delete the account');

            $.post(AppUrls.deleteAccount, { id })
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "Account deleted successfully.", "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", "Failed to delete account.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                })
                .always(() => {
                    GlobalLoading.hideWithFormEnable();
                });
        });

        $('#save-account').click(() => {
            const mode = $('#save-account').data('mode');
            const id = $('#save-account').data('id') || 0;
            const account = $('#txtAccount').val().trim();
            const role = $('#ddlRole').val().trim();
            const roleText = $('#ddlRole option:selected').text().trim();
            const groupText = $('#ddlGroup option:selected').text().trim();

            //selectAnalysis handling --------------------------------------------------------
            var selectedIds = $('#selectAnalysis').val(); // returns string[] or null
            if (!selectedIds || selectedIds.length === 0) {
                alert('Please select at least one Analysis.');
                return;
            }
            // Store as comma-separated string in hidden field (form POST)
            $('#hdnAnalysisIds').val(selectedIds.join(','));
            //--------------------------------------------------------------------------------

            if (!account || !roleText) {
                ToastrHelper.notification("error", "Account and Role are required.", "Validation");
                return;
            }

            const payload = {
                id: id,
                user_Id: account,
                user_Role: roleText,
                user_Group: groupText,
                user_Analysis: selectedIds,
            };

            console.log(payload);

            const url = mode === 'add' ? AppUrls.addAccount : AppUrls.editAccount;

            // Show loading during save
            GlobalLoading.showWithFormDisable('Saving...', `Please wait while we ${mode} the account`);

            $.post(url, payload)
                .done(res => {
                    if (res.success) {
                        $('#accountModal').modal('hide');
                        ToastrHelper.notification("success", `Account ${mode === 'add' ? 'added' : 'updated'} successfully.`, "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to save account.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                })
                .always(() => {
                    GlobalLoading.hideWithFormEnable();
                });
        });

        $('#cancel-account, #close-account').click(() => {
            $('#accountModal').modal('hide');
        });

        $('#txtAccount').on('input', function () {
            this.value = this.value.replace(/[^0-9]/g, '');
        });

    }

    function clearForm() {
        $('#txtAccount').val('');
        $('#ddlRole').val(0);
    }

    function loadAllDropdowns() {
        return Promise.all([
            clearForm(),
            loadRoles()
        ]);
    }

    return {
        init: function () {
            console.log('AccountModule.init() called');
            
            // Ensure global loading is available
            if (typeof GlobalFormPrep === 'undefined') {
                console.error('GlobalFormPrep is not available!');
                // Fallback to direct initialization
                initAccountData().then(() => {
                    bindEvents();
                });
                return;
            }
            
            // Use global form preparation for user account page
            console.log('Starting GlobalFormPrep.standardPage for account page');
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initAccountData(),
                // Include dropdown loading in page initialization
                loadAllDropdowns()
            ], {
                loadingMessage: 'Loading User Management...',
                loadingSubtitle: 'Please wait while we load user accounts and prepare the interface',
                onAfterLoad: function(results) {
                    console.log('Account page loading completed', results);
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                },
                onError: function(error) {
                    console.error('Failed to prepare user account page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    AccountModule.init();
});
