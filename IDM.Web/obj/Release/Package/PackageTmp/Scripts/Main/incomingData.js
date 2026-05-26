var IncomingDataModule = (function () {
    var table;
    let materialList = [];
    let parameterList = [];
    let preSubmitData = []; // Array to store pre-submitted parameter data
    let trialData = []; // Array to store trial values in memory

    function loadAccount() {
        return $.get(AppUrls.getAccount)
            .done(data => {
                const $ddlReceivedBy = $('#ddlReceivedBy').empty().append(`<option value="0">-- Select User --</option>`);
                $.each(data, (i, item) => {
                    $ddlReceivedBy.append(`<option value="${item.id}">${item.user_Id}</option>`);
                });

                const $ddlInspectedBy = $('#ddlInspectedBy').empty().append(`<option value="0">-- Select User --</option>`);
                $.each(data, (i, item) => {
                    $ddlInspectedBy.append(`<option value="${item.id}">${item.user_Id}</option>`);
                });

                const $ddlEncodedBy = $('#ddlEncodedBy').empty().append(`<option value="0">-- Select User --</option>`);
                $.each(data, (i, item) => {
                    $ddlEncodedBy.append(`<option value="${item.id}">${item.user_Id}</option>`);
                });
            });
    }

    function loadMaterial() {
        return $.get(AppUrls.getMaterial)
            .done(data => {
                materialList = data;
                
                // Initialize Select2 with search functionality
                const $ddlMaterial = $('#ddlMaterial');
                
                // Clear and populate options
                $ddlMaterial.empty().append(`<option value="0">-- Select Material --</option>`);
                $.each(data, (i, item) => {
                    $ddlMaterial.append(`<option value="${item.id}">${item.material_No}</option>`);
                });
                
                // Destroy existing Select2 if it exists
                $ddlMaterial.select2('destroy');
                
                // Initialize Select2 with default theme to match existing design
                $ddlMaterial.select2({
                    placeholder: "-- Select Material --",
                    allowClear: true,
                    width: '100%',
                    minimumInputLength: 0
                });
            });
    }

    function loadParameter(id) {
        return $.get(AppUrls.getMaterialParameter, { id: id })
            .done(data => {
                parameterList = data;
                console.log('load parameter');
                console.log(data);
                
                // Get existing parameter+site combinations from pre-submit data
                const existingCombinations = preSubmitData.map(item => ({
                    parameterName: item.parameter_Name,
                    site: item.site_Name
                }));
                
                const ddl = $('#ddlParameter').empty().append(`<option value="0">-- Select Parameter --</option>`);
                $.each(data, (i, item) => {
                    // Check if this parameter+site combination already exists
                    const isExisting = existingCombinations.some(existing => 
                        existing.parameterName === item.parameter_Name && existing.site === item.site_Name
                    );
                    
                    // Only add option if it doesn't exist or if site is empty
                    if (!isExisting || !item.site_Name) {
                        ddl.append(`<option value="${item.id}">${item.parameter_Name}</option>`);
                    }
                });
            });
    }

    function bindEvents() {
        // Initialize Select2 after page loads
        $(document).ready(function() {
            console.log('Select2 loaded:', typeof $.fn.select2);
            
            // Small delay to ensure all data is loaded
            setTimeout(function() {
                if (typeof $.fn.select2 === 'function') {
                    $('#ddlMaterial').select2({
                        placeholder: "-- Select Material --",
                        allowClear: true,
                        width: '100%',
                        minimumInputLength: 0
                    });
                    console.log('Select2 initialized successfully');
                } else {
                    console.error('Select2 is not loaded');
                }
            }, 500);
        });
        
        // Use global form preparation system
        prepareIncomingDataPage()
            .then(() => {
                console.log('Incoming data page prepared successfully');
                // Continue with any additional setup after page is ready
            })
            .catch(error => {
                console.error('Failed to prepare incoming data page:', error);
            });

        // Initialize submit button as disabled (redundant since onAfterLoad handles it, but kept for safety)
        $('#btnSubmit').prop('disabled', true).attr('title', 'No parameters to submit');

        $('#addTrialInput').on('click', () => {
            // Validate required fields before showing trial modal
            const deliveryDate = $('#txtDeliveryDate').val().trim();
            const receivedDate = $('#txtReceivedDate').val().trim();
            const parameterName = $('#ddlParameter option:selected').text().trim();
            
            // Check if delivery date is null or empty
            if (!deliveryDate || deliveryDate === '') {
                ToastrHelper.notification("error", "Please select a Delivery Date before adding trial data.", "Validation");
                $('#txtDeliveryDate').focus();
                return;
            }
            
            // Check if received date is null or empty
            if (!receivedDate || receivedDate === '') {
                ToastrHelper.notification("error", "Please select a Received Date before adding trial data.", "Validation");
                $('#txtReceivedDate').focus();
                return;
            }
            
            // Check if parameter name is null or not selected
            if (!parameterName || parameterName === '-- Select Parameter --' || parameterName === '0') {
                ToastrHelper.notification("error", "Please select a Parameter before adding trial data.", "Validation");
                $('#ddlParameter').focus();
                return;
            }
            
            // All validations passed - show trial modal
            $('#trialInputModal').modal('show');
            $('#proceed-trialInput').data('mode', 'add').removeData('id');
            // Clear trial input and list when opening modal
            $('#txtRawDataTrial').val('');
            updateTrialList();
        });

        // Add trial value to memory
        $('#add-trialInput').on('click', function() {
            addTrialValue();
        });

        // Proceed with trial data
        $('#proceed-trialInput').on('click', function() {
            proceedWithTrialData();
        });

        // Close trial modal
        $('#close-trialInput').on('click', function() {
            $('#trialInputModal').modal('hide');
        });

        // Allow Enter key to add trial value
        $('#txtRawDataTrial').on('keypress', function(e) {
            if (e.which === 13) { // Enter key
                e.preventDefault();
                addTrialValue();
            }
        });


        $('#save-account').on('click', () => {
            const mode = $('#save-account').data('mode');
            const id = $('#save-account').data('id') || 0;
            const account = $('#txtAccount').val().trim();
            const role = $('#ddlRole').val().trim();

            if (!account || !role) {
                ToastrHelper.notification("error", "Account and Role are required.", "Validation");
                return;
            }

            const payload = {
                accountId: id,
                accountName: account,
                roleId: role,
            };

            console.log(payload);

            const url = mode === 'add' ? AppUrls.addAccount : AppUrls.editAccount;

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
                });
        });

        $('#cancel-account, #close-account').click(() => {
            $('#accountModal').modal('hide');
        });

        $('#txtAccount').on('input', function () {
            this.value = this.value.replace(/[^0-9]/g, '');
        });

    }

    function bindSelect() {
        $('#ddlMaterial').on('change', function () {
            const selectedId = parseInt($(this).val());
            const selectedMaterial = materialList.find(m => m.id === selectedId);
            console.log(selectedMaterial);
            
            // Clear all parameter-related fields first
            $('#ddlParameter').empty().append(`<option value="0">-- Select Parameter --</option>`);
            $('#txtParameterValue').val('');
            $('#txtUom').val('');
            $('#txtSite').val('');
            $('#txtLowerSpecsLimit').val('');
            $('#txtUpperSpecsLimit').val('');
            $('#txtLowerControlLimit').val('');
            $('#txtUpperControlLimit').val('');
            $('#txtSpecsJudgement').val('').css({
                'border-color': '',
                'background-color': ''
            });
            $('#txtControlJudgement').val('').css({
                'border-color': '',
                'background-color': ''
            });
            
            if (selectedMaterial) {
                // Enable parameter controls when material is selected
                enableParameterControls();
                
                $('#txtMaterialName').val(selectedMaterial.material_Name || '');
                $('#txtSupplierName').val(selectedMaterial.supplier_Name || '');
                $('#txtManufacturerName').val(selectedMaterial.manufacturer_Name || '');
                $('#txtAreaName').val(selectedMaterial.area_Name || '');

                loadParameter(selectedMaterial.material_No);

            } else {
                // Disable parameter controls when no material is selected
                disableParameterControls();
                $('#txtMaterialName, #txtSupplierName, #txtManufacturerName').val('');
            }
        });

        $('#ddlParameter').on('change', function () {
            const selectedId = parseInt($(this).val());
            const selectedParameter = parameterList.find(p => p.id === selectedId);
            console.log(selectedParameter);
            
            // Clear parameter value and judgements first
            $('#txtParameterValue').val('');
            $('#txtSpecsJudgement').val('').css({
                'border-color': '',
                'background-color': ''
            });
            $('#txtControlJudgement').val('').css({
                'border-color': '',
                'background-color': ''
            });
            
            if (selectedParameter) {
                $('#txtUom').val(selectedParameter.uom_Name || '');
                $('#txtSite').val(selectedParameter.site_Name || '');
                $('#txtLowerSpecsLimit').val(selectedParameter.lowerSpecsLimit || '');
                $('#txtUpperSpecsLimit').val(selectedParameter.upperSpecsLimit || '');
                $('#txtLowerControlLimit').val(selectedParameter.lowerControlLimit || '');
                $('#txtUpperControlLimit').val(selectedParameter.upperControlLimit || '');
                // Store edcSpcFlag data for later use in pre-submit
                $(this).data('edcSpcFlag', selectedParameter.edcSpcFlag || '');
            } else {
                $('#txtUom, #txtSite').val('');
                $(this).data('edcSpcFlag', '');
            }
        });

        // Add color styling for judgement dropdowns
        function styleJudgementDropdown(dropdownId) {
            const value = $(dropdownId).val();
            if (value === '1') { // PASSED
                $(dropdownId).css({
                    'border-color': 'green',
                    'background-color': '#d4edda'
                });
            } else if (value === '2') { // FAILED
                $(dropdownId).css({
                    'border-color': 'red',
                    'background-color': '#f8d7da'
                });
            } else if (value === '3') { // N/A
                $(dropdownId).css({
                    'border-color': 'orange',
                    'background-color': '#fff3cd'
                });
            } else { // Default option
                $(dropdownId).css({
                    'border-color': '',
                    'background-color': ''
                });
            }
        }

        $('#ddlPackaging').on('change', function() {
            styleJudgementDropdown('#ddlPackaging');
        });

        $('#ddlVisual').on('change', function() {
            styleJudgementDropdown('#ddlVisual');
        });

        $('#ddlPreQual').on('change', function() {
            styleJudgementDropdown('#ddlPreQual');
        });

        // Date validation for delivery vs expiration
        function validateDateDifference() {
            const deliveryDate = $('#txtDeliveryDate').val();
            const expirationDate = $('#txtExpirationDate').val();
            
            if (deliveryDate && expirationDate) {
                const delivery = new Date(deliveryDate);
                const expiration = new Date(expirationDate);
                const diffTime = Math.abs(expiration - delivery);
                const diffMonths = diffTime / (1000 * 60 * 60 * 24 * 30); // Approximate months
                
                if (diffMonths >= 3) {
                    // 3 months or more - green
                    $('#txtDeliveryDate').css({
                        'border-color': 'green',
                        'background-color': '#d4edda'
                    });
                    $('#txtExpirationDate').css({
                        'border-color': 'green',
                        'background-color': '#d4edda'
                    });
                } else {
                    // Less than 3 months - red
                    $('#txtDeliveryDate').css({
                        'border-color': 'red',
                        'background-color': '#f8d7da'
                    });
                    $('#txtExpirationDate').css({
                        'border-color': 'red',
                        'background-color': '#f8d7da'
                    });
                }
            } else {
                // Clear styling if dates are not complete
                $('#txtDeliveryDate').css({
                    'border-color': '',
                    'background-color': ''
                });
                $('#txtExpirationDate').css({
                    'border-color': '',
                    'background-color': ''
                });
            }
        }

        $('#txtDeliveryDate, #txtExpirationDate').on('change', function() {
            validateDateDifference();
        });

        // Pre-Submit button functionality
        $('#btnPreSubmit').on('click', function() {
            saveParameterToPreSubmit();
        });

        // Submit to Database button functionality
        $('#btnSubmit').on('click', function() {
            submitToDatabase();
        });

        function submitToDatabase() {
            // Validation: Check if there's data to submit
            if (preSubmitData.length === 0) {
                ToastrHelper.notification("error", "No parameters to submit. Please add parameters using Pre-Submit.", "Validation");
                return;
            }

            // Get form header data
            const formData = {
                material_No: $('#ddlMaterial option:selected').text(),
                lotNumber: $('#txtLotNumber').val(),
                delivery_Date: $('#txtDeliveryDate').val(),
                received_Date: $('#txtReceivedDate').val(),
                manufacturing_Date: $('#txtManufacturingDate').val(),
                expiration_Date: $('#txtExpirationDate').val(),
                material_Name: $('#txtMaterialName').val(),
                area_Name: $('#txtAreaName').val(),
                supplier_Name: $('#txtSupplierName').val(),
                manufacturer_Name: $('#txtManufacturerName').val(),
                receivedBy: $('#ddlReceivedBy option:selected').text(),
                inspection_Date: $('#txtInspectionDate').val(),
                inspectedBy: $('#ddlInspectedBy option:selected').text(),
                encodedBy: $('#ddlEncodedBy option:selected').text(),
                remarks: $('#txtRemarks').val(),
                packaging_Document_Check: $('#ddlPackaging option:selected').text(),
                view_Appearance_Check: $('#ddlVisual option:selected').text(),
                pre_Qualification_Test: $('#ddlPreQual option:selected').text(),
                toolId: $('#txtToolId').val(),
                job_Number: $('#txtJobNumber').val(),
                parameters: preSubmitData
            };

            // Validate required fields
            if (!formData.material_No || formData.material_No === '0') {
                ToastrHelper.notification("error", "Material is required.", "Validation");
                return;
            }
            if (!formData.delivery_Date) {
                ToastrHelper.notification("error", "Delivery date is required.", "Validation");
                return;
            }
            if (!formData.received_Date) {
                ToastrHelper.notification("error", "Received date is required.", "Validation");
                return;
            }
            if (!formData.manufacturing_Date) {
                ToastrHelper.notification("error", "Manufacturing date is required.", "Validation");
                return;
            }
            if (!formData.expiration_Date) {
                ToastrHelper.notification("error", "Expiration date is required.", "Validation");
                return;
            }
            if (!formData.receivedBy || formData.receivedBy === '0') {
                ToastrHelper.notification("error", "Received by is required.", "Validation");
                return;
            }
            if (!formData.inspection_Date) {
                ToastrHelper.notification("error", "Inspection date is required.", "Validation");
                return;
            }
            if (!formData.inspectedBy || formData.inspectedBy === '0') {
                ToastrHelper.notification("error", "Inspected by is required.", "Validation");
                return;
            }
            if (!formData.encodedBy || formData.encodedBy === '0') {
                ToastrHelper.notification("error", "Encoded by is required.", "Validation");
                return;
            }
            if (!formData.packaging_Document_Check || formData.packaging_Document_Check === '0') {
                ToastrHelper.notification("error", "Packaging judgement is required.", "Validation");
                return;
            }
            if (!formData.view_Appearance_Check || formData.view_Appearance_Check === '0') {
                ToastrHelper.notification("error", "Visual judgement is required.", "Validation");
                return;
            }
            if (!formData.pre_Qualification_Test || formData.pre_Qualification_Test === '0') {
                ToastrHelper.notification("error", "Pre-qualification judgement is required.", "Validation");
                return;
            }

            console.log('Submitting data:', formData);

            // Show loading state
            $('#btnSubmit').prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i> Submitting...');

            // Submit to database (you'll need to update this URL)
            $.post(AppUrls.submitIncomingData, formData)
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "Data submitted successfully to database.", "Success");
                        
                        // Clear all data after successful submission
                        clearAllData();
                        
                        // Optionally close modals or redirect
                        // window.location.reload();
                    } else {
                        ToastrHelper.notification("error", res.message || "Failed to submit data.", "Error");
                    }
                })
                .fail(xhr => {
                    ToastrHelper.notification("error", xhr.responseJSON?.message || "An error occurred while submitting data.", "Error");
                })
                .always(() => {
                    // Restore button state
                    $('#btnSubmit').prop('disabled', false).html('<i class="fas fa-database me-2"></i> Submit to Database');
                });
        }

        function clearAllData() {
            // Clear pre-submit data
            preSubmitData = [];
            updatePreSubmitTable();
            
            // Clear trial data
            clearTrialData();
            
            // Clear form fields
            $('#ddlMaterial').val('0');
            $('#txtLotNumber').val('');
            $('#txtDeliveryDate').val('');
            $('#txtReceivedDate').val('');
            $('#txtManufacturingDate').val('');
            $('#txtExpirationDate').val('');
            $('#txtMaterialName').val('');
            $('#txtAreaName').val('');
            $('#txtSupplierName').val('');
            $('#txtManufacturerName').val('');
            $('#ddlReceivedBy').val('0');
            $('#txtInspectionDate').val('');
            $('#ddlInspectedBy').val('0');
            $('#ddlEncodedBy').val('0');
            $('#txtRemarks').val('');
            $('#ddlPackaging').val('0');
            $('#ddlVisual').val('0');
            $('#ddlPreQual').val('0');
            
            // Clear parameter fields
            clearParameterFields();
            
            // Reset dropdown styling
            $('#ddlPackaging, #ddlVisual, #ddlPreQual').css({
                'border-color': '',
                'background-color': ''
            });
            
            // Reset date styling
            $('#txtDeliveryDate, #txtExpirationDate').css({
                'border-color': '',
                'background-color': ''
            });
        }

        function saveParameterToPreSubmit() {
            // Get current parameter data
            const parameterName = $('#ddlParameter option:selected').text();
            const parameterValue = $('#txtParameterValue').val();
            const uom = $('#txtUom').val();
            const site = $('#txtSite').val();
            const specsLimit = `${$('#txtLowerSpecsLimit').val()} - ${$('#txtUpperSpecsLimit').val()}`;
            const specsJudgement = $('#txtSpecsJudgement').val();
            const controlLimit = `${$('#txtLowerControlLimit').val()} - ${$('#txtUpperControlLimit').val()}`;
            const controlJudgement = $('#txtControlJudgement').val();
            const edcSpcFlag = $('#ddlParameter').data('edcSpcFlag') || '';

            // Validation
            if (!parameterName || parameterName === '-- Select Parameter --') {
                ToastrHelper.notification("error", "Please select a parameter.", "Validation");
                return;
            }
            if (!parameterValue) {
                ToastrHelper.notification("error", "Please enter a parameter value.", "Validation");
                return;
            }

            // Check for duplicate parameter (same parameter name and site name)
            const isDuplicate = preSubmitData.some(item => 
                item.parameter_Name === parameterName && item.site_Name === site
            );

            if (isDuplicate) {
                ToastrHelper.notification("warning", "This parameter with the same site already exists in the pre-submit data.", "Duplicate Warning");
                return;
            }

            // Create parameter object
            const parameterData = {
                id: Date.now(), // Unique ID for the row
                parameter_Name: parameterName,
                parameter_Value: parameterValue,
                uom_Name: uom,
                site_Name: site,
                Lower_Specs_Limit: $('#txtLowerSpecsLimit').val(),
                Upper_Specs_Limit: $('#txtUpperSpecsLimit').val(),
                specs_Judgement: specsJudgement,
                Lower_Control_Limit: $('#txtLowerControlLimit').val(),
                Upper_Control_Limit: $('#txtUpperControlLimit').val(),
                control_Judgement: controlJudgement,
                edcSpcFlag: edcSpcFlag
            };

            // Add to array
            preSubmitData.push(parameterData);

            // Update table
            updatePreSubmitTable();

            // Clear parameter fields for next entry
            clearParameterFields();

            ToastrHelper.notification("success", "Parameter added to pre-submit data.", "Success");
        }

        function updatePreSubmitTable() {
            const tbody = $('#preSubmitDataTable tbody');
            tbody.empty();

            preSubmitData.forEach(data => {
                // Create badge for specs judgement
                let specsBadge = '';
                if (data.specs_Judgement === 'PASSED') {
                    specsBadge = '<span class="badge badge-success">PASSED</span>';
                } else if (data.specs_Judgement === 'FAILED') {
                    specsBadge = '<span class="badge badge-danger">FAILED</span>';
                } else if (data.specs_Judgement === 'NA') {
                    specsBadge = '<span class="badge badge-warning">NA</span>';
                } else {
                    specsBadge = data.specs_Judgement || '';
                }

                // Create badge for control judgement
                let controlBadge = '';
                if (data.control_Judgement === 'PASSED') {
                    controlBadge = '<span class="badge badge-success">PASSED</span>';
                } else if (data.control_Judgement === 'OOC') {
                    controlBadge = '<span class="badge badge-danger">OOC</span>';
                } else if (data.control_Judgement === 'NA') {
                    controlBadge = '<span class="badge badge-warning">NA</span>';
                } else {
                    controlBadge = data.control_Judgement || '';
                }

                // Create badge for edcSpcFlag
                let edcSpcBadge = '';
                if (data.edcSpcFlag === '1' || data.edcSpcFlag === 1) {
                    edcSpcBadge = '<span class="badge badge-primary">TRUE</span>';
                } else {
                    edcSpcBadge = '<span class="badge badge-secondary">FALSE</span>';
                }

                const row = `
                    <tr>
                        <td>${data.parameter_Name}</td>
                        <td>${data.parameter_Value}</td>
                        <td>${data.uom_Name}</td>
                        <td>${data.site_Name}</td>
                        <td>${data.Lower_Specs_Limit}</td>
                        <td>${data.Upper_Specs_Limit}</td>
                        <td>${specsBadge}</td>
                        <td>${data.Lower_Control_Limit}</td>
                        <td>${data.Upper_Control_Limit}</td>
                        <td>${controlBadge}</td>
                        <td>${edcSpcBadge}</td>
                        <td>
                            <button class="btn btn-sm btn-danger" onclick="removePreSubmitItem(${data.id})">
                                <i class="fas fa-trash"></i>
                            </button>
                        </td>
                    </tr>
                `;
                tbody.append(row);
            });
            
            // Enable/disable submit button based on data availability
            if (preSubmitData.length === 0) {
                $('#btnSubmit').prop('disabled', true).attr('title', 'No parameters to submit');
            } else {
                $('#btnSubmit').prop('disabled', false).removeAttr('title');
            }
            
            // Refresh parameter dropdown to update available options
            const selectedMaterialId = $('#ddlMaterial').val();
            if (selectedMaterialId && selectedMaterialId !== '0') {
                const selectedMaterial = materialList.find(m => m.id === parseInt(selectedMaterialId));
                if (selectedMaterial) {
                    loadParameter(selectedMaterial.material_No);
                }
            }
        }

        function clearParameterFields() {
            $('#ddlParameter').val('0');
            $('#txtParameterValue').val('');
            $('#txtUom').val('');
            $('#txtSite').val('');
            $('#txtLowerSpecsLimit').val('');
            $('#txtUpperSpecsLimit').val('');
            $('#txtLowerControlLimit').val('');
            $('#txtUpperControlLimit').val('');
            $('#txtSpecsJudgement').val('').css({
                'border-color': '',
                'background-color': ''
            });
            $('#txtControlJudgement').val('').css({
                'border-color': '',
                'background-color': ''
            });
        }

        // Make removePreSubmitItem globally accessible
        window.removePreSubmitItem = function(id) {
            preSubmitData = preSubmitData.filter(item => item.id !== id);
            updatePreSubmitTable();
            ToastrHelper.notification("info", "Parameter removed from pre-submit data.", "Info");
        };

        $('#txtParameterValue').on('input', function () {
            // Allow only numbers and decimal point
            let value = $(this).val();
            let sanitizedValue = value.replace(/[^0-9.]/g, '');
            
            // Prevent multiple decimal points
            let decimalCount = (sanitizedValue.match(/\./g) || []).length;
            if (decimalCount > 1) {
                // Keep only the first decimal point
                let parts = sanitizedValue.split('.');
                sanitizedValue = parts[0] + '.' + parts.slice(1).join('');
            }
            
            // Update the input value if it was changed
            if (value !== sanitizedValue) {
                $(this).val(sanitizedValue);
            }
            
            const numericValue = parseFloat($(this).val());
            
            // Get specs limits
            const lowerSpecsText = $('#txtLowerSpecsLimit').val();
            const upperSpecsText = $('#txtUpperSpecsLimit').val();
            const lowerSpecs = parseFloat(lowerSpecsText) || 0;
            const upperSpecs = parseFloat(upperSpecsText) || 0;
            
            // Get control limits
            const lowerControlText = $('#txtLowerControlLimit').val();
            const upperControlText = $('#txtUpperControlLimit').val();
            const lowerControl = parseFloat(lowerControlText) || 0;
            const upperControl = parseFloat(upperControlText) || 0;

            // Check specs judgement
            if (!isNaN(numericValue)) {
                // Check if both limits are null/blank
                if ((!lowerSpecsText || lowerSpecsText.trim() === '') && (!upperSpecsText || upperSpecsText.trim() === '')) {
                    $('#txtSpecsJudgement').val('NA').css({
                        'border-color': 'orange',
                        'background-color': '#fff3cd'
                    });
                } else if (numericValue >= lowerSpecs && numericValue <= upperSpecs) {
                    $('#txtSpecsJudgement').val('PASSED').css({
                        'border-color': 'green',
                        'background-color': '#d4edda'
                    });
                } else {
                    $('#txtSpecsJudgement').val('FAILED').css({
                        'border-color': 'red',
                        'background-color': '#f8d7da'
                    });
                }
            } else {
                $('#txtSpecsJudgement').val('').css({
                    'border-color': '',
                    'background-color': ''
                });
            }

            // Check control judgement
            if (!isNaN(numericValue)) {
                // Check if both limits are null/blank
                if ((!lowerControlText || lowerControlText.trim() === '') && (!upperControlText || upperControlText.trim() === '')) {
                    $('#txtControlJudgement').val('NA').css({
                        'border-color': 'orange',
                        'background-color': '#fff3cd'
                    });
                } else if (numericValue >= lowerControl && numericValue <= upperControl) {
                    $('#txtControlJudgement').val('PASSED').css({
                        'border-color': 'green',
                        'background-color': '#d4edda'
                    });
                } else {
                    $('#txtControlJudgement').val('OOC').css({
                        'border-color': 'red',
                        'background-color': '#f8d7da'
                    });
                }
            } else {
                $('#txtControlJudgement').val('').css({
                    'border-color': '',
                    'background-color': ''
                });
            }
        });
    }

    function clearForm() {
      
    }

    // Trial Data Management Functions
    function addTrialValue() {
        const trialValue = $('#txtRawDataTrial').val().trim();
        
        if (!trialValue) {
            ToastrHelper.notification("error", "Please enter a trial value.", "Validation");
            return;
        }
        
        // Add trial value to memory array matching TrialDetail class structure
        trialData.push({
            trial_Counter: trialData.length + 1, // Auto-increment counter starting from 1
            trial_Value: trialValue,
            parameter_Name: $('#ddlParameter option:selected').text(),
            site_Name: $('#txtSite').val()
        });
        
        // Clear input field
        $('#txtRawDataTrial').val('');
        
        // Update trial list display
        updateTrialList();
        
        ToastrHelper.notification("success", "Trial value added.", "Success");
    }

    function updateTrialList() {
        const trialList = $('#trialValuesList');
        trialList.empty();
        
        if (trialData.length === 0) {
            trialList.append('<li class="list-group-item text-muted">No trials added yet</li>');
            // Clear average display
            $('#trialAverage').remove();
            return;
        }
        
        // Display each trial with counter using TrialDetail structure
        trialData.forEach(trial => {
            const trialItem = `
                <li class="list-group-item d-flex justify-content-between align-items-center">
                    <span>
                        <span class="badge badge-primary me-2">${trial.trial_Counter}</span>
                        ${trial.trial_Value}
                    </span>
                    <button class="btn btn-sm btn-outline-danger" onclick="removeTrialValue(${trial.trial_Counter})">
                        <i class="fas fa-times"></i>
                    </button>
                </li>
            `;
            trialList.append(trialItem);
        });
        
        // Calculate and display average
        displayTrialAverage();
    }

    function displayTrialAverage() {
        if (trialData.length === 0) return;
        
        // Calculate average of all trial values using TrialDetail structure
        const numericValues = trialData
            .map(trial => parseFloat(trial.trial_Value))
            .filter(value => !isNaN(value));
        
        let averageText = '';
        if (numericValues.length > 0) {
            const average = numericValues.reduce((sum, val) => sum + val, 0) / numericValues.length;
            averageText = average.toFixed(2);
        } else {
            averageText = 'N/A (non-numeric values)';
        }
        
        // Remove existing average display if any
        $('#trialAverage').remove();
        
        // Add average display after the trial list
        const averageDisplay = `
            <div id="trialAverage" class="mt-3 p-2 bg-light border rounded">
                <strong>Average: ${averageText}</strong>
                <small class="text-muted">(${trialData.length} trials)</small>
            </div>
        `;
        $('#trialValuesList').after(averageDisplay);
    }

    function proceedWithTrialData() {
        if (trialData.length === 0) {
            ToastrHelper.notification("warning", "Please add at least one trial value.", "Warning");
            return;
        }
        
        // Calculate average of all trial values using TrialDetail structure
        const numericValues = trialData
            .map(trial => parseFloat(trial.trial_Value))
            .filter(value => !isNaN(value));
        
        let average = '';
        if (numericValues.length > 0) {
            average = (numericValues.reduce((sum, val) => sum + val, 0) / numericValues.length).toFixed(2);
        } else {
            ToastrHelper.notification("error", "No numeric trial values found to calculate average.", "Error");
            return;
        }
        
        // Store only the average in parameter value field
        $('#txtParameterValue').val(average);
        
        // Trigger judgement calculation by triggering the parameter value change event
        $('#txtParameterValue').trigger('input');
        
        // Prepare trial data for database submission using TrialDetail structure
        const trialDataForSubmit = {
            id: 0,
            material_No: $('#ddlMaterial option:selected').text(),
            lotNumber: $('#txtLotNumber').val(),
            delivery_Date: $('#txtDeliveryDate').val(),
            received_Date: $('#txtReceivedDate').val(),
            toolId: $('#txtToolId').val(),
            job_Number: $('#txtJobNumber').val(),
            
            parameter_Name: $('#ddlParameter option:selected').text(),
            trial_Value: average,
            site_Name: $('#txtSite').val(),
            
            // Trial details array matching TrialDetail class
            trial: trialData.map(trial => ({
                trial_Counter: trial.trial_Counter,
                trial_Value: trial.trial_Value,
                parameter_Name: $('#ddlParameter option:selected').text(),
                site_Name: trial.site_Name
            }))
        };
     
        // Send trial data to database
        $.ajax({
            url: AppUrls.submitTrial,
            type: 'POST',
            data: trialDataForSubmit,
            success: function(response) {
                if (response.success) {
                    // Close modal
                    $('#trialInputModal').modal('hide');
                    
                    // Clear trial data after successful submission
                    clearTrialData();
                    
                    ToastrHelper.notification("success", "Trial data successfully submitted to database.", "Success");
                } else {
                    ToastrHelper.notification("error", response.message || "Failed to submit trial data.", "Error");
                }
            },
            error: function(xhr, status, error) {
                console.error('Error submitting trial data:', error);
                ToastrHelper.notification("error", "Error submitting trial data. Please try again.", "Error");
            }
        });
    }

    // Make removeTrialValue globally accessible
    window.removeTrialValue = function(trialCounter) {
        trialData = trialData.filter(trial => trial.trial_Counter !== trialCounter);
        
        // Re-number the remaining trials to maintain sequential counters
        trialData.forEach((trial, index) => {
            trial.trial_Counter = index + 1;
        });
        
        updateTrialList();
        ToastrHelper.notification("info", "Trial value removed.", "Info");
    };

    function clearTrialData() {
        trialData = [];
        updateTrialList();
    }

    function loadAllDropdowns() {
        return Promise.all([
            loadAccount(),
            loadMaterial()
        ]);
    }

    // Page-specific form preparation for incoming data
    function prepareIncomingDataPage() {
        return GlobalFormPrep.preparePage({
            loadingMessage: 'Loading Incoming Data...',
            loadingSubtitle: 'Please wait while we prepare your form',
            loadDataFunctions: [
                loadAccount(),
                loadMaterial()
            ],
            onAfterLoad: function(results) {
                // Page-specific setup after data loads
                $('#btnSubmit').prop('disabled', true).attr('title', 'No parameters to submit');
                disableParameterControls(); // Disable parameter controls initially
            },
            onError: function(error) {
                console.error('Incoming data preparation failed:', error);
                ToastrHelper.notification("error", "Failed to load initial data. Please refresh the page.", "Error");
            }
        });
    }

    // Backward compatibility - keep existing functions
    function showLoadingOverlay() {
        GlobalLoading.show('Loading Incoming Data...', 'Please wait while we prepare your form');
        GlobalLoading.disableForm();
    }

    function hideLoadingOverlay() {
        GlobalLoading.enableForm();
        GlobalLoading.hide();
    }

    function disableFormInteractions() {
        // Disable all form controls
        $('input, select, button, textarea').prop('disabled', true);
        // Add visual indication that page is loading
        $('body').css('cursor', 'wait');
    }

    function enableFormInteractions() {
        // Enable all form controls except submit button (should remain disabled until data is added)
        $('input, select, textarea').prop('disabled', false);
        $('button').not('#btnSubmit').prop('disabled', false);
        // Reset cursor
        $('body').css('cursor', 'default');
    }

    function disableParameterControls() {
        // Disable parameter-related controls
        $('#ddlParameter').prop('disabled', true).attr('disabled', 'disabled');
        $('#txtParameterValue').prop('disabled', true).attr('disabled', 'disabled');
        $('#btnPreSubmit').prop('disabled', true).attr('disabled', 'disabled');
        $('#btnSubmit').prop('disabled', true).attr('disabled', 'disabled');
        $('#addTrialInput').prop('disabled', true).attr('disabled', 'disabled');
        
        // Disable material-dependent dropdowns and textboxes
        $('#ddlPackaging').prop('disabled', true).attr('disabled', 'disabled');
        $('#ddlVisual').prop('disabled', true).attr('disabled', 'disabled');
        $('#ddlPreQual').prop('disabled', true).attr('disabled', 'disabled');
        $('#ddlReceivedBy').prop('disabled', true).attr('disabled', 'disabled');
        $('#ddlInspectedBy').prop('disabled', true).attr('disabled', 'disabled');
        $('#ddlEncodedBy').prop('disabled', true).attr('disabled', 'disabled');
        
        // Disable inspection date (material-dependent)
        $('#txtInspectionDate').prop('disabled', true).attr('disabled', 'disabled');
        
        // Disable material info fields
        $('#txtLotNumber').prop('disabled', true).attr('disabled', 'disabled');
        $('#txtJobNumber').prop('disabled', true).attr('disabled', 'disabled');
        $('#txtToolId').prop('disabled', true).attr('disabled', 'disabled');
        
        // Disable other text fields
        $('#txtRemarks').prop('disabled', true).attr('disabled', 'disabled');
        
        // Note: Basic date fields (Delivery, Received, Manufacturing, Expiration) remain enabled
        // Only Inspection Date is disabled based on material selection
        
        // Add visual indication that controls are disabled (including inspection date)
        $('#ddlParameter, #txtParameterValue, #ddlPackaging, #ddlVisual, #ddlPreQual, ' +
          '#ddlReceivedBy, #ddlInspectedBy, #ddlEncodedBy, #txtInspectionDate, ' +
          '#txtLotNumber, #txtJobNumber, #txtToolId, #txtRemarks, #btnPreSubmit, #btnSubmit, #addTrialInput')
          .addClass('disabled-control')
          .css({
              'opacity': '0.65',
              'cursor': 'not-allowed',
              'pointer-events': 'none'
          });
    }

    function enableParameterControls() {
        // Enable parameter-related controls
        $('#ddlParameter').prop('disabled', false).removeAttr('disabled');
        $('#txtParameterValue').prop('disabled', false).removeAttr('disabled');
        $('#btnPreSubmit').prop('disabled', false).removeAttr('disabled');
        $('#btnSubmit').prop('disabled', false).removeAttr('disabled');
        $('#addTrialInput').prop('disabled', false).removeAttr('disabled');
        
        // Enable material-dependent dropdowns and textboxes
        $('#ddlPackaging').prop('disabled', false).removeAttr('disabled');
        $('#ddlVisual').prop('disabled', false).removeAttr('disabled');
        $('#ddlPreQual').prop('disabled', false).removeAttr('disabled');
        $('#ddlReceivedBy').prop('disabled', false).removeAttr('disabled');
        $('#ddlInspectedBy').prop('disabled', false).removeAttr('disabled');
        $('#ddlEncodedBy').prop('disabled', false).removeAttr('disabled');
        
        // Enable inspection date (material-dependent)
        $('#txtInspectionDate').prop('disabled', false).removeAttr('disabled');
        
        // Enable material info fields
        $('#txtLotNumber').prop('disabled', false).removeAttr('disabled');
        $('#txtJobNumber').prop('disabled', false).removeAttr('disabled');
        $('#txtToolId').prop('disabled', false).removeAttr('disabled');
        
        // Enable other text fields
        $('#txtRemarks').prop('disabled', false).removeAttr('disabled');
        
        // Note: Basic date fields remain enabled as they are not affected by material selection
        
        // Remove visual indication of disabled state (including inspection date)
        $('#ddlParameter, #txtParameterValue, #ddlPackaging, #ddlVisual, #ddlPreQual, ' +
          '#ddlReceivedBy, #ddlInspectedBy, #ddlEncodedBy, #txtInspectionDate, ' +
          '#txtLotNumber, #txtJobNumber, #txtToolId, #txtRemarks, #btnPreSubmit, #btnSubmit, #addTrialInput')
          .removeClass('disabled-control')
          .css({
              'opacity': '',
              'cursor': '',
              'pointer-events': ''
          });
    }

    return {
        init: function () {
            bindEvents();
            bindSelect();
        }
    };
})();

$(function () {
    IncomingDataModule.init();
});
