var MaterialModule = (function () {
    var table;
    var tableMaterialParameter;

    function initMaterialData() {
        // Return a promise that resolves when DataTable is fully loaded
        return new Promise((resolve, reject) => {
            // Hide the table initially to prevent showing DataTable's internal loading
            $('#materialTable').hide();
            
            table = $('#materialTable').DataTable({
                ajax: {
                    url: AppUrls.getMaterial,
                    type: 'GET',
                    dataSrc: '',
                    "error": function (xhr) {
                        ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                        reject(xhr);
                    }
                },
                columns: [
                    { data: 'material_No' },
                    { data: 'material_Name' },
                    { data: 'manufacturer_Name' },
                    { data: 'supplier_Name' },
                    { data: 'area_Name' },
                    {
                        data: null,
                        render: function (data) {
                            return `
                                <a href='#' class="assign-btn" data-id="${data.material_No}" title="Assign Parameters">
                                     <i class="fas fa-tasks text-warning"></i>
                                </a>
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
                    $('#materialTable').fadeIn(300);
                    // DataTable is fully loaded
                    resolve();
                }
            });
        });
    }

    function materialParameterData(id) {
        if ($.fn.DataTable.isDataTable('#materialParameterTable')) {
            $('#materialParameterTable').DataTable().clear().destroy();
        }

        tableMaterialParameter = $('#materialParameterTable').DataTable({
            ajax: {
                url: AppUrls.getMaterialParameter,
                type: 'GET',
                data: function (d) {
                    return { id: id };
                },
                dataSrc: '',
                "error": function (xhr) {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                }
            },
            searching: false,
            lengthChange: false,
            info: false,
            paging: false,
            columns: [
                { data: 'parameter_Name' },
                { data: 'uom_Name' },
                { data: 'site_Name' },
                {
                    data: 'edcSpcFlag',
                    render: function (data, type, row) {
                        // If data is 1 → show FontAwesome check, else FontAwesome times
                        return data == 1 ? '<i class="fas fa-check text-success"></i>' : '<i class="fas fa-times text-danger"></i>';
                    }
                },
                { data: 'lowerSpecsLimit' },
                { data: 'upperSpecsLimit' },
                { data: 'lowerControlLimit' },
                { data: 'upperControlLimit' },
                {
                    data: null,
                    render: function (data) {
                        return `
                            <a href='#' class="editMaterialParameter-btn" data-id="${data.id}" title="Edit">
                                 <i class="fas fa-edit text-primary"></i>
                            </a>
                            <a href='#' class=" deleteMaterialParameter-btn" data-id="${data.id}" title="Delete">
                                <i class="fas fa-trash-alt text-danger"></i>
                            </a>
                        `;
                    }
                }
            ]
        });
    }

    function loadManufacturer() {
        return $.get(AppUrls.getManufacturer)
            .done(data => {
                const $ddl = $('#ddlManufacturer').empty().append(`<option value="0">-- Select Manufacturer --</option>`);
                $.each(data, (i, item) => {
                    $ddl.append(`<option value="${item.id}">${item.manufacturer_Name}</option>`);
                });
            });
    }

    function loadSupplier() {
        return $.get(AppUrls.getSupplier)
            .done(data => {
                const $ddl = $('#ddlSuplier').empty().append(`<option value="0">-- Select Supplier --</option>`);
                $.each(data, (i, item) => {
                    $ddl.append(`<option value="${item.id}">${item.supplier_Name}</option>`);
                });
            });
    }

    function loadArea() {
        return $.get(AppUrls.getArea)
            .done(data => {
                const $ddl = $('#ddlArea').empty().append(`<option value="0">-- Select Area --</option>`);
                $.each(data, (i, item) => {
                    $ddl.append(`<option value="${item.id}">${item.area_Name}</option>`);
                });
            });
    }

    function loadParameter() {
        return $.get(AppUrls.getParameter)
            .done(data => {
                const $ddl = $('#ddlParameter').empty().append(`<option value="0">-- Select Parameter --</option>`);
                $.each(data, (i, item) => {
                    $ddl.append(`<option value="${item.id}">${item.parameter_Name}</option>`);
                });
            });
    }

    function loadUom() {
        return $.get(AppUrls.getUom)
            .done(data => {
                const $ddl = $('#ddlUom').empty().append(`<option value="0">-- Select Uom --</option>`);
                $.each(data, (i, item) => {
                    $ddl.append(`<option value="${item.id}">${item.uom_Name}</option>`);
                });
            });
    }

    function loadSite() {
        return $.get(AppUrls.getSite)
            .done(data => {
                const $ddl = $('#ddlSite').empty().append(`<option value="0">-- Select Site --</option>`);
                $.each(data, (i, item) => {
                    $ddl.append(`<option value="${item.id}">${item.site_Name}</option>`);
                });
            });
    }

    function bindEventsMaterialParameter() {
        $('#close-materialParameter, #cancel-materialParameter').on('click', () => {
            $('#materialParameterModal').modal('hide');
        });

        $('#materialTable').on('click', '.assign-btn', function () {
            const data = table.row($(this).closest('tr')).data();
            const id = $(this).data('id');
            materialParameterData(id);
            loadAllDropdownsMaterialParameter().then(() => {
                $('#materialParameterModal').modal('show');
                $('#save-materialParameter').data('mode', 'add').removeData('id');
                $('#save-materialParameter').data('matid', id);
            });
        });

        $('#materialParameterTable').on('click', '.editMaterialParameter-btn', function () {
            const data = tableMaterialParameter.row($(this).closest('tr')).data();
            loadAllDropdowns().then(() => {
                $('#ddlParameter').val(data.parameterId);
                $('#ddlUom').val(data.uomId);
                $('#ddlSite').val(data.siteId);
                $('#chkIsEdspc').prop('checked', data.edcSpcFlag === '1');
                $('#txtLowerSpecsLimit').val(data.lowerSpecsLimit);
                $('#txtUpperSpecsLimit').val(data.upperSpecsLimit);
                $('#txtLowerControlLimit').val(data.lowerControlLimit);
                $('#txtUpperControlLimit').val(data.upperControlLimit);
            });
            $('#save-materialParameter').data('mode', 'edit').data('id', data.id);
        });

        $('#materialParameterTable').on('click', '.deleteMaterialParameter-btn', function () {
            const id = $(this).data('id');
            if (!confirm('Are you sure to delete this material parameter?')) return;

            $.post(AppUrls.deleteMaterialParameter, { id })
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "Material Parameter deleted successfully.", "Success");
                        clearFormMaterialParameter();
                        tableMaterialParameter.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", "Failed to delete material parameter.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#save-materialParameter').click(() => {
            const mode = $('#save-materialParameter').data('mode');
            const material = $('#save-materialParameter').data('matid');
            const id = $('#save-materialParameter').data('id') || 0;
            const parameter = $('#ddlParameter').val().trim();
            const parameterText = $('#ddlParameter option:selected').text().trim();
            const uom = $('#ddlUom').val().trim();
            const uomText = $('#ddlUom option:selected').text().trim();
            const site = $('#ddlSite').val().trim();
            const siteText = $('#ddlSite option:selected').text().trim();
            const edcspc = $('#chkIsEdspc').prop('checked') ? 1 : 0;
            const lowerSpecsLimit = $('#txtLowerSpecsLimit').val().trim();
            const upperSpecsLimit = $('#txtUpperSpecsLimit').val().trim();
            const lowerControlLimit = $('#txtLowerControlLimit').val().trim();
            const upperControlLimit = $('#txtUpperControlLimit').val().trim();

            if (!parameter) {
                ToastrHelper.notification("error", "Parameter are required.", "Validation");
                return;
            }
            if (!uom || !site) {
                ToastrHelper.notification("error", "Uom and Site are required.", "Validation");
                return;
            }

            if (!lowerSpecsLimit || !upperSpecsLimit || !lowerControlLimit || !upperControlLimit) {
                ToastrHelper.notification("error", "Upper and Lower Limits are required.", "Validation");
                return;
            }

            if (!isNaN(lowerSpecsLimit) && !isNaN(upperSpecsLimit)) {
                const lower = parseFloat(lowerSpecsLimit);
                const upper = parseFloat(upperSpecsLimit);

                if (lower > upper) {
                    ToastrHelper.notification("error", "Invalid input Lower must be less than Upper.", "Validation");
                    return;
                } 
            }

            if (!isNaN(lowerControlLimit) && !isNaN(upperControlLimit)) {
                const lower = parseFloat(lowerControlLimit);
                const upper = parseFloat(upperControlLimit);

                if (lower > upper) {
                    ToastrHelper.notification("error", "Invalid input Lower must be less than Upper.", "Validation");
                    return;
                }
            }

            const payload = {
                id: id,
                materialId: material,
                material_No: material, // Send material number as well
                parameterId: parameter,
                parameter_Name: parameterText, // Send the selected text instead of ID
                uomId: uom,
                uom_Name: uomText, // Send the selected text instead of ID
                siteId: site,
                site_Name: siteText, // Send the selected text instead of ID
                EdcSpcFlag: edcspc, // Changed from isEdcspc to EdcSpcFlag
                lowerSpecsLimit: lowerSpecsLimit,
                upperSpecsLimit: upperSpecsLimit,
                lowerControlLimit: lowerControlLimit,
                upperControlLimit: upperControlLimit
            };
            const url = mode === 'add' ? AppUrls.addMaterialParameter : AppUrls.editMaterialParameter;

            $.post(url, payload)
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", `Material Parameter ${mode === 'add' ? 'added' : 'updated'} successfully.`, "Success");
                        clearFormMaterialParameter();
                        tableMaterialParameter.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to save material parameter.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });
    }

    function bindEvents() {
        $('#addMaterial').click(() => {
            clearForm();
            $('#materialModal').modal('show');
            $('#save-material').data('mode', 'add').removeData('id');
            
            // Dropdowns are already loaded during page initialization
            // No need to wait for AJAX calls
        });

        $('#materialTable').on('click', '.edit-btn', function () {
            const data = table.row($(this).closest('tr')).data();
            console.log(data);
            
            // Show modal immediately
            $('#txtMaterialName').val(data.material_Name);
            $('#txtMaterialNo').val(data.material_No);
            $('#ddlManufacturer').val(data.manufacturerId);
            $('#ddlSuplier').val(data.supplierId);
            $('#ddlArea').val(data.areaId);
            $('#materialModal').modal('show');
            $('#save-material').data('mode', 'edit').data('id', data.id);
            
            // Dropdowns are already loaded during page initialization
        });

        $('#materialTable').on('click', '.delete-btn', function () {
            const id = $(this).data('id');
            if (!confirm('Are you sure to delete this material?')) return;

            $.post(AppUrls.deleteMaterial, { id })
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "Material deleted successfully.", "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", "Failed to delete material.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#save-material').click(() => {
            const mode = $('#save-material').data('mode');
            const id = $('#save-material').data('id') || 0;
            const materialName = $('#txtMaterialName').val().trim();
            const materialNo = $('#txtMaterialNo').val().trim();
            //const manufacturer = $('#ddlManufacturer').val().trim();
            //const supplier = $('#ddlSuplier').val().trim();
            //const area = $('#ddlArea').val().trim();

            const manufacturer = $('#ddlManufacturer option:selected').text().trim();
            const supplier = $('#ddlSuplier option:selected').text().trim();
            const area = $('#ddlArea option:selected').text().trim();


            if (!materialName || !materialNo) {
                ToastrHelper.notification("error", "Material No and Material Name are required.", "Validation");
                return;
            }
            if (!manufacturer || !area || !supplier) {
                ToastrHelper.notification("error", "Manufacturer, Supplier and Area are required.", "Validation");
                return;
            }

            const payload = {
                id: id,
                material_Name: materialName,
                material_No: materialNo,
                manufacturer_Name: manufacturer,
                supplier_Name: supplier,
                area_Name: area
            };

            console.log('save');
            console.log(payload);

            const url = mode === 'add' ? AppUrls.addMaterial : AppUrls.editMaterial;

            $.post(url, payload)
                .done(res => {
                    if (res.success) {
                        $('#materialModal').modal('hide');
                        ToastrHelper.notification("success", `Material ${mode === 'add' ? 'added' : 'updated'} successfully.`, "Success");
                        table.ajax.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to save material.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                });
        });

        $('#cancel-material, #close-material').click(() => {
            $('#materialModal').modal('hide');
        });

        
    }

    function clearFormMaterialParameter() {
        $('#txtLowerSpecsLimit').val('');
        $('#txtUpperSpecsLimit').val('');
        $('#txtLowerControlLimit').val('');
        $('#txtUpperControlLimit').val('');
        $('#ddlSite').val(0);
        $('#ddlUom').val(0);
        $('#ddlParameter').val(0);
        $('#chkIsEdspc').prop('checked', false);
    }

    function clearForm() {
        $('#txtMaterialName').val('');
        $('#txtMaterialNo').val('');
        $('#ddlManufacturer').val(0);
        $('#ddlSuplier').val(0);
        $('#ddlArea').val(0);
    }

    function loadAllDropdowns() {
        return Promise.all([
            clearForm(),
            loadManufacturer(),
            loadArea(),
            loadSupplier()
        ]);
    }

    function loadAllDropdownsMaterialParameter() {
        return Promise.all([
            loadSite(),
            loadUom(), 
            loadParameter()
   
        ]);
    }

    return {
        init: function () {
            // Use global form preparation for material page
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initMaterialData(),
                // Include dropdown loading in page initialization
                loadAllDropdowns()
            ], {
                loadingMessage: 'Loading Material Management...',
                loadingSubtitle: 'Please wait while we load materials and prepare the interface',
                onAfterLoad: function(results) {
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                    bindEventsMaterialParameter();
                },
                onError: function(error) {
                    console.error('Failed to prepare material page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                    bindEventsMaterialParameter();
                }
            });
        }
    };
})();



$(document).ready(function () {
    MaterialModule.init();
});

$(document).ready(function () {
    $('.select2').select2({
        width: '100%'
    });
});