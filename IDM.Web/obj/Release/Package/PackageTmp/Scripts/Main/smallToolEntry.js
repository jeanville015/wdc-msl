var IncomingDataModule = (function () {
    var table;
    let materialList = [];
    let parameterList = [];
    let preSubmitData = []; // Array to store pre-submitted parameter data
    let trialData = []; // Array to store trial values in memory

    
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

    function loadAnalysis() {
        return $.get(AppUrls.getAnalysis)
            .done(data => {
                const ddlAnalysis = $('#ddlAnalysis').empty().append(`<option value="0">-- Select Analysis --</option>`);
                $.each(data, (i, item) => {
                    ddlAnalysis.append(`<option value="${item.analysisName}">${item.analysisName}</option>`);
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
        $(document).ready(function () {
            console.log('Select2 loaded:', typeof $.fn.select2);

            // Small delay to ensure all data is loaded
            setTimeout(function () {
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
            const parameterName = $('#ddlParameter option:selected').text().trim();

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
        $('#add-trialInput').on('click', function () {
            addTrialValue();
        });

        // Proceed with trial data
        $('#proceed-trialInput').on('click', function () {
            proceedWithTrialData();
        });

        // Close trial modal
        $('#close-trialInput').on('click', function () {
            $('#trialInputModal').modal('hide');
        });

        // Allow Enter key to add trial value
        $('#txtRawDataTrial').on('keypress', function (e) {
            if (e.which === 13) { // Enter key
                e.preventDefault();
                addTrialValue();
            }
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

        
        // Pre-Submit button functionality
        $('#btnPreSubmit').on('click', function () {
            saveParameterToPreSubmit();
        });

        // Submit to Database button functionality
        $('#btnSubmit').on('click', function () {
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
                amethystJob: $('#txtJobNumber').val(),
                toolName: $('#txtToolId').val(),
                analysis: $('#ddlAnalysis option:selected').text(),
                customer: $('#txtCustomer').val()
                //parameters: preSubmitData
            };

            // Show loading state
            $('#btnSubmit').prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i> Submitting...');

            // Submit to database (you'll need to update this URL)
            $.post(AppUrls.createToolEntry, formData)
                .done(res => {
                    if (res.success) {
                        ToastrHelper.notification("success", "Data submitted successfully to database.", "Success");
                        
                        // Prepare staging data - convert to TableData format like SetApproval
                        const headerFields = {
                            material_No: $('#ddlMaterial option:selected').text(),
                            lotNumber: $('#txtLotNumber').val(),
                            amethystJob: $('#txtJobNumber').val(),
                            toolName: $('#txtToolId').val(),
                            analysis: $('#ddlAnalysis option:selected').text(),
                            customer: $('#txtCustomer').val()
                        };

                        // Find maximum number of trials across all parameters to create consistent columns
                        const maxTrialCount = Math.max(...preSubmitData.map(param => 
                            (param.trialData && param.trialData.length) || 0
                        ), 0);

                        // Define base column headers
                        const baseColumns = [
                            'material_No', 'lotNumber', 'amethystJob', 'toolName', 'analysis', 'customer',
                            'parameter_Name', 'parameter_Value', 'uom_Name', 'site_Name',
                            'Lower_Specs_Limit', 'Upper_Specs_Limit', 'specs_Judgement',
                            'Lower_Control_Limit', 'Upper_Control_Limit', 'control_Judgement', 'edcSpcFlag'
                        ];

                        // Add trial columns dynamically (Trial1, Trial2, ...)
                        const trialColumns = [];
                        for (let i = 1; i <= maxTrialCount; i++) {
                            trialColumns.push(`Trial${i}`);
                        }

                        // Combine all columns
                        const columns = [...baseColumns, ...trialColumns];

                        // Transform data to rows matching column order
                        const data = preSubmitData.map(parameter => {
                            const row = [];
                            
                            // Add header fields (same for all parameters)
                            baseColumns.forEach(column => {
                                if (headerFields.hasOwnProperty(column)) {
                                    row.push(headerFields[column]);
                                } else {
                                    row.push(parameter[column] || '');
                                }
                            });

                            // Add trial values for this parameter
                            if (parameter.trialData && parameter.trialData.length > 0) {
                                for (let i = 0; i < maxTrialCount; i++) {
                                    row.push(parameter.trialData[i] || ''); // Pad with empty string if no trial
                                }
                            } else {
                                // Add empty trial columns if no trial data
                                for (let i = 0; i < maxTrialCount; i++) {
                                    row.push('');
                                }
                            }

                            return row;
                        });

                        // Create TableData object
                        const tableData = {
                            Columns: columns,
                            Data: data
                        };

                        // Convert to JSON string for tableContent parameter
                        const tableContent = JSON.stringify(tableData);

                        // Call CreateSmallToolStaging with tableContent string
                        $.post(AppUrls.createSmallToolStaging, { tableContent: tableContent })
                            .done(stagingRes => {
                                if (stagingRes.success) {
                                    ToastrHelper.notification("success", "Staging data created successfully.", "Success");
                                } else {
                                    ToastrHelper.notification("warning", stagingRes.message || "Staging data creation failed.", "Warning");
                                }
                            })
                            .fail(xhr => {
                                ToastrHelper.notification("warning", xhr.responseJSON?.message || "An error occurred while creating staging data.", "Warning");
                            });

                        clearAllData();
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
            $('#txtJobNumber').val('');
            $('#txtToolId').val('');
            $('#ddlAnalysis').val('0');
            $('#txtMaterialName').val('');
            $('#txtAreaName').val('');
            $('#txtSupplierName').val('');
            $('#txtManufacturerName').val('');
            $('#txtCustomer').val('');
            
            // Clear parameter fields
            clearParameterFields();
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
        window.removePreSubmitItem = function (id) {
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
            analysis: $('#ddlAnalysis option:selected').text(),
            toolName: $('#txtToolName').val(),
            amethystJob: $('#txtJobNumber').val(),
            customer: $('#txtCustomer').val(),

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
            success: function (response) {
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
            error: function (xhr, status, error) {
                console.error('Error submitting trial data:', error);
                ToastrHelper.notification("error", "Error submitting trial data. Please try again.", "Error");
            }
        });
    }

    // Make removeTrialValue globally accessible
    window.removeTrialValue = function (trialCounter) {
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
            loadMaterial(),
            loadAnalysis()
        ]);
    }

    // Page-specific form preparation for incoming data
    function prepareIncomingDataPage() {
        return GlobalFormPrep.preparePage({
            loadingMessage: 'Loading Incoming Data...',
            loadingSubtitle: 'Please wait while we prepare your form',
            loadDataFunctions: [
                loadMaterial(),
                loadAnalysis()
            ],
            onAfterLoad: function (results) {
                // Page-specific setup after data loads
                $('#btnSubmit').prop('disabled', true).attr('title', 'No parameters to submit');
                disableParameterControls(); // Disable parameter controls initially
            },
            onError: function (error) {
                console.error('Incoming data preparation failed:', error);
                ToastrHelper.notification("error", "Failed to load initial data. Please refresh the page.", "Error");
            }
        });
    }

    function disableParameterControls() {
        // Disable parameter-related controls
        $('#ddlParameter').prop('disabled', true).attr('disabled', 'disabled');
        $('#txtParameterValue').prop('disabled', true).attr('disabled', 'disabled');
        $('#btnPreSubmit').prop('disabled', true).attr('disabled', 'disabled');
        $('#btnSubmit').prop('disabled', true).attr('disabled', 'disabled');
        $('#addTrialInput').prop('disabled', true).attr('disabled', 'disabled');

        // Disable material info fields
        $('#txtLotNumber').prop('disabled', true).attr('disabled', 'disabled');
        $('#txtJobNumber').prop('disabled', true).attr('disabled', 'disabled');
        $('#txtToolId').prop('disabled', true).attr('disabled', 'disabled');

        // Add visual indication that controls are disabled
        $('#ddlParameter, #txtParameterValue, #txtLotNumber, #txtJobNumber, #txtToolId, ' +
            '#btnPreSubmit, #btnSubmit, #addTrialInput')
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

        // Enable material info fields
        $('#txtLotNumber').prop('disabled', false).removeAttr('disabled');
        $('#txtJobNumber').prop('disabled', false).removeAttr('disabled');
        $('#txtToolId').prop('disabled', false).removeAttr('disabled');

        
        // Remove visual indication of disabled state
        $('#ddlParameter, #txtParameterValue, #txtLotNumber, #txtJobNumber, #txtToolId, ' +
            '#btnPreSubmit, #btnSubmit, #addTrialInput')
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
