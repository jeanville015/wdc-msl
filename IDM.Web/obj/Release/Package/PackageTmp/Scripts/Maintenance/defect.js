var DefectModule = (function () {
    var table;

    function initDefectData() {
        table = $('#defectTable').DataTable({
            ajax: {
                url: AppUrls.getDefect,
                type: 'GET',
                dataSrc: '',
                "error": function (xhr) {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                }
            },
            columns: [
                { data: 'analysisApplicable' },
                { data: 'defectType' },
                { data: 'defectName' },
                { data: '_2KxSem' },
                { data: 'talc' },
                { data: 'overAllPhysicalDefect' },
                {
                    data: null,
                    render: function (data) {
                        return `
                            <a href='#' class="edit-btn" data-id="${data.defectId}" title="Edit">
                                 <i class="fas fa-edit text-primary"></i>
                            </a>
                            <a href='#' class=" delete-btn" data-id="${data.defectId}" title="Delete">
                                <i class="fas fa-trash-alt text-danger"></i>
                            </a>
                        `;
                    }
                }
            ]
        });
    }

    function bindEvents() {
        $('#uploadFile').click(() => {
            $('#csvFileInput').click();
        });

        $('#csvFileInput').on('change', function () {
            var file = this.files[0];
            if (!file) return;

            var formData = new FormData();
            formData.append("file", file);

            //$.ajax({
            //    url: AppUrls.uploadFileDefect,
            //    type: 'POST',
            //    data: formData,
            //    processData: false,
            //    contentType: false,
            //    success: function (response) {
            //        alert("Upload successful!");
            //    },
            //    error: function () {
            //        alert("Upload failed.");
            //    }
            //});

            $.ajax({
                url: AppUrls.uploadFileDefect,
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    // Response is the JSON object from your controller
                    if (response.success) {
                        ToastrHelper.notification("success", "Upload successful!", "Success");
                        table.ajax.reload();
                        // Optional: clear the file input
                        $('#csvFileInput').val('');
                    } else {
                        // If success is false, show the specific error message from the controller
                        // Fallback to a generic message if message is null
                        var errorMsg = response.message || "An unknown error occurred during upload.";
                        ToastrHelper.notification("error", "Upload failed: " + errorMsg, "Error");
                    }
                },
                error: function (xhr, status, error) {
                    // This triggers if the server returns a 500 error or the network fails
                    alert("System error: Could not connect to the server.");
                }
            });

        });

        $('#addDefect').click(() => {
            clearForm();
            $('#defectModal').modal('show');
            $('#save-defect').data('mode', 'add').removeData('id');
        });

        $('#defectTable').on('click', '.edit-btn', function () {
            const data = table.row($(this).closest('tr')).data();
            console.log(data.id);
            $('#txtAnalysis').val(data.analysisApplicable);
            $('#txtDefectType').val(data.defectType);
            $('#txtDefect').val(data.defectName);
            $('#txt2KxSem').val(data._2KxSem);
            $('#txtTalc').val(data.talc);
            $('#txtOverAllPhysicalDefect').val(data.overAllPhysicalDefect);
            $('#defectModal').modal('show');

            $('#save-defect').data('mode', 'edit').data('id', data.defectId);
        });

        $('#defectTable').on('click', '.delete-btn', function () {
            const id = $(this).data('id');
            if (!confirm('Are you sure to delete this defect?')) return;

            $.post(AppUrls.deleteDefect, { id })
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "Defect deleted successfully.", "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", "Failed to delete defect.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#save-defect').click(() => {
            const mode = $('#save-defect').data('mode');
            const defectId = $('#save-defect').data('id') || 0;
            const analysis = $('#txtAnalysis').val().trim();
            const defectType = $('#txtDefectType').val().trim();
            const defectName = $('#txtDefect').val().trim();
            const _2kxSem = $('#txt2KxSem').val().trim();
            const talc = $('#txtTalc').val().trim();
            const overallPhysicalDefect = $('#txtOverAllPhysicalDefect').val().trim();

            if (!defectName) {
                ToastrHelper.notification("error", "Defect are required.", "Validation");
                return;
            }

            if (!analysis) {
                ToastrHelper.notification("error", "Analysis are required.", "Validation");
                return;
            }

            if (!defectType) {
                ToastrHelper.notification("error", "Defect Type are required.", "Validation");
                return;
            }

            const payload = {
                defectId: defectId,
                analysisApplicable: analysis,
                defectType: defectType,
                defectName: defectName,
                _2KxSem: _2kxSem,
                talc: talc,
                overAllPhysicalDefect: overallPhysicalDefect
            };

            const url = mode === 'add' ? AppUrls.addDefect : AppUrls.editDefect;

            $.post(url, payload)
                .done(res => {
                    if (res.success) {
                        $('#defectModal').modal('hide');
                        ToastrHelper.notification("success", `Defect ${mode === 'add' ? 'added' : 'updated'} successfully.`, "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to save defect.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#cancel-defect, #close-defect').click(() => {
            $('#defectModal').modal('hide');
        });
    }

    function clearForm() {
        $('#txtAnalysis').val('');
        $('#txtDefectType').val('');
        $('#txtDefect').val('');
        $('#txt2KxSem').val('');
        $('#txtTalc').val('');
        $('#txtOverAllPhysicalDefect').val('');
    }

    return {
        init: function () {
            // Use global form preparation for defect page
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initDefectData()
            ], {
                loadingMessage: 'Loading Defect Management...',
                loadingSubtitle: 'Please wait while we load units of measure and prepare the interface',
                onAfterLoad: function(results) {
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                },
                onError: function(error) {
                    console.error('Failed to prepare Defect page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    DefectModule.init();
});
