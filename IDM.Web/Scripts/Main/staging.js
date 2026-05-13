var StagingModule = (function () {

    // Helper function to parse URL parameters
    function getUrlParameters() {
        const params = new URLSearchParams(window.location.search);
        const result = {
            analysisTrial: params.get('analysisTrial'),
            analysis: params.get('analysis'),
            amethystJob: params.get('amethystJob'),
            analyzedBy: params.get('analyzedBy'),
            source: params.get('source') || 'direct'
        };
        
        return result;
    }

    // Helper function to control UI based on source
    function controlUIBasedOnSource(source) {
        if (source !== 'wdcx01') {
            // Disable approve and reject buttons
            $('#approve-btn').prop('disabled', true).addClass('disabled');
            $('#reject-btn').prop('disabled', true).addClass('disabled');
            
            // Hide the sidebar
            $('.main-sidebar').hide();
            
            // Adjust content wrapper to take full width
            $('.content-wrapper').css('margin-left', '0');
            
            console.log('UI controls disabled and sidebar hidden for source:', source);
        } else {
            console.log('Full UI controls enabled for source:', source);
        }
    }

    // Helper function to build column list with proper ordering and filtering
    function buildColumnList(firstItem) {
        const dataColumns = Object.keys(firstItem);
        const desiredOrder = ['amethystJob', 'analysis'];
        const excludedColumns = ['storedBy', 'storeTs', 'updatedBy', 'updatedTs', 'storedby', 'storets'];
        let additionalPropsColumns = [];

        // Process additionalProperties
        if (firstItem.additionalProperties) {
            const additionalKeys = Object.keys(firstItem.additionalProperties);
            
            // Add desired columns in specified order
            desiredOrder.forEach(key => {
                if (additionalKeys.includes(key)) {
                    additionalPropsColumns.push(key);
                }
            });
            
            // Add other columns, excluding ID columns and excluded columns
            additionalKeys.forEach(key => {
                if (!additionalPropsColumns.includes(key) && 
                    !excludedColumns.includes(key) && 
                    !key.toLowerCase().endsWith('id')) {
                    additionalPropsColumns.push(key);
                }
            });
        }

        // Add desired columns from main data if not found in additionalProperties
        if (!additionalPropsColumns.includes('amethystJob') && dataColumns.includes('amethystJob')) {
            additionalPropsColumns.unshift('amethystJob');
        }
        if (!additionalPropsColumns.includes('analysis') && dataColumns.includes('analysis')) {
            const insertIndex = additionalPropsColumns.includes('amethystJob') ? 1 : additionalPropsColumns.length;
            additionalPropsColumns.splice(insertIndex, 0, 'analysis');
        }

        // Combine and filter main data columns
        const filteredDataColumns = dataColumns.filter(col => 
            col !== 'additionalProperties' && 
            !excludedColumns.includes(col) && 
            !desiredOrder.includes(col) && 
            !col.toLowerCase().endsWith('id')
        );

        return [...additionalPropsColumns, ...filteredDataColumns];
    }

    // Helper function to render table header
    function renderTableHeader($table, columns) {
        const headerRow = $('<tr></tr>');
        columns.forEach(column => {
            headerRow.append(`<th style="min-width: 120px; white-space: nowrap;">${column.toUpperCase()}</th>`);
        });
        $table.find('thead').append(headerRow);

        // Apply table styling
        $table.css({
            'table-layout': 'auto',
            'width': '100%'
        });
    }

    // Helper function to get cell value from item or additionalProperties
    function getCellValue(item, column) {
        if (item.additionalProperties && item.additionalProperties.hasOwnProperty(column)) {
            return item.additionalProperties[column];
        }
        return item[column];
    }

    // Helper function to format date values
    function formatDateValue(cellValue, column) {
        if (cellValue && (column.toLowerCase().includes('date') || column.toLowerCase().includes('time'))) {
            try {
                const date = new Date(cellValue);
                const options = { month: 'short', day: 'numeric', year: 'numeric' };
                return date.toLocaleDateString('en-US', options);
            } catch (e) {
                return cellValue;
            }
        }
        return cellValue;
    }

    // Helper function to apply badge styling to judgement values
    function formatJudgementValue(cellValue, column) {
        if (column.toLowerCase() === 'judgement' && cellValue) {
            const judgement = cellValue.toString().toLowerCase();
            // Define the common style to ensure consistent width
            const minWidthStyle = 'style="min-width: 70px; display: inline-block;"';
        
            if (judgement === 'passed') {
                return `<span class="badge badge-success" ${minWidthStyle}>${cellValue}</span>`;
            } else if (judgement === 'failed') {
                return `<span class="badge badge-danger" ${minWidthStyle}>${cellValue}</span>`;
            } else { //for [N/A], etc
                return `<span class="badge badge-secondary" ${minWidthStyle}>${cellValue}</span>`;
            }
        }
        return cellValue;
    }

    // Helper function to render table rows
    function renderTableRows($table, dataResponse, columns) {
        dataResponse.forEach(function(item) {
            const row = $('<tr></tr>');
            columns.forEach(column => {
                let cellValue = getCellValue(item, column);
                cellValue = formatDateValue(cellValue, column);
                cellValue = formatJudgementValue(cellValue, column);
                row.append(`<td>${cellValue || ''}</td>`);
            });
            $table.find('tbody').append(row);
        });
    }

    // Main function to get source and destination table and load staging data
    function loadStagingData(params) {
        console.log('before table');

        return new Promise((resolve, reject) => {
            // First, get the source table name by analysis
            $.ajax({
                url: AppUrls.getByAnalysisAsync,
                type: 'GET',
                data: { analysisName: params.analysis },
                success: function (response) {
                    const sourceTable = response.sourceTable || response.tableName || response.table;
                    const destinationTable = response.destinationTable || null;
                    
                    if (sourceTable) {

                        // Second call to get data by job and analysis using the source table
                        $.ajax({
                            url: AppUrls.getByJobAndAnalysisAsync,
                            type: 'GET',
                            data: {
                                table: sourceTable,
                                amethystJob: params.amethystJob,
                                analysis: params.analysis,
                                analysisTrial: params.analysisTrial
                            },
                            success: function (dataResponse) {
                                // Check if redirect is needed
                                if (dataResponse && dataResponse.redirect === true) {
                                    ToastrHelper.notification("info", dataResponse.message, "Information");
                                    setTimeout(function() {
                                        window.location.href = dataResponse.redirectUrl;
                                    }, 2000);
                                    resolve();
                                    return;
                                }

                                loadStagingData_Table(dataResponse);

                                // Clear existing table content
                                const $table = $('#stagingTable');
                                $table.find('thead').empty();
                                $table.find('tbody').empty();

                                // Create dynamic table based on data structure
                                if (dataResponse && dataResponse.data && dataResponse.data.length > 0) {
                                    const columns = buildColumnList(dataResponse.data[0]);

                                    // 2. Extract first 5 columns for the header div
                                    const headerColumns = columns.slice(0, 8);
                                    const firstRow = dataResponse.data[0];
                                    // 3. Populate the Info Div
                                    const $infoContainer = $('#headerInfoContainer');
                                    $infoContainer.empty();

                                    //set the header label
                                    $('#AnalysisName').text(response.analysisName);

                                    let infoHtml = '<div class="row m-2 p-2 border bg-light">';
                                    headerColumns.forEach(col => {
                                        let val = getCellValue(firstRow, col) || 'N/A';
                                        val = formatDateValue(val, col)
                                        infoHtml += `
                                            <div class="col col-auto">
                                                <strong>${col.toUpperCase()}:</strong>
                                                <span>${val}</span>
                                            </div>`;
                                    });
                                    infoHtml += '</div>';
                                    $infoContainer.append(infoHtml);

                                    // 4. REMOVE the first 5 columns from the array so the table doesn't see them
                                    const tableColumns = columns.slice(8);

                                    // 5. Render table with the REMAINING columns
                                    renderTableHeader($table, tableColumns);
                                    renderTableRows($table, dataResponse.data, tableColumns);

                                } else {
                                    // Show no data message
                                    $table.find('thead').append('<tr><th>No Data</th></tr>');
                                    $table.find('tbody').append('<tr><td>No data available</td></tr>');
                                }

                                ////-------------------------------------------------------------------------------
                                //// Clear existing table content
                                //const $table = $('#stagingTable');
                                //$table.find('thead').empty();
                                //$table.find('tbody').empty();

                                //// Create dynamic table based on data structure
                                //if (dataResponse && dataResponse.data && dataResponse.data.length > 0) {
                                //    const columns = buildColumnList(dataResponse.data[0]);
                                //    renderTableHeader($table, columns);
                                //    renderTableRows($table, dataResponse.data, columns);
                                //} else {
                                //    // Show no data message
                                //    $table.find('thead').append('<tr><th>No Data</th></tr>');
                                //    $table.find('tbody').append('<tr><td>No data available</td></tr>');
                                //}
                                ////-------------------------------------------------------------------------------

                                resolve();
                            },
                            error: function (xhr, status, error) {
                                ToastrHelper.notification("error", "Error fetching staging data: " + error, "Error");
                                reject(error);
                            }
                        });
                    } else {
                        ToastrHelper.notification("error", "No source table found for analysis: " + params.analysis, "Error");
                        reject("No source table found");
                    }
                },
                error: function (xhr, status, error) {
                    ToastrHelper.notification("error", "Error fetching analysis data: " + error, "Error");
                    reject(error);
                }
            });
        });
    }

    // Initialize staging data loading
    function loadStagingData_Table(dataResponse) { 
        // Clear existing table content
        const $table_lsdt = $('#stagingTablecontainer');
        $table_lsdt.find('thead').empty();
        $table_lsdt.find('tbody').empty();

        // Create dynamic table based on data structure
        if (dataResponse && dataResponse.data && dataResponse.data.length > 0) {
            const columns = buildColumnList(dataResponse.data[0]);
            renderTableHeader($table_lsdt, columns);
            renderTableRows($table_lsdt, dataResponse.data, columns);
        } else {
            // Show no data message
            $table_lsdt.find('thead').append('<tr><th>No Data</th></tr>');
            $table_lsdt.find('tbody').append('<tr><td>No data available</td></tr>');
        } 
    }


    // Initialize staging data loading
    function initStagingData() {
        return new Promise((resolve, reject) => {
            const params = getUrlParameters();

            // Apply UI controls based on source
            controlUIBasedOnSource(params.source);

            if (params.analysis && params.amethystJob) {
                loadStagingData(params)
                    .then(() => resolve())
                    .catch((error) => reject(error));
            } else {
                ToastrHelper.notification("error", "Missing required parameters", "Error");
                reject("Missing parameters");
            }
        });
    }

    // Function to capture staging table content (columns and data)
    function captureStagingTableData() {
        const $table = $('#stagingTablecontainer');
        const columns = [];
        const data = [];
        
        // Get column headers
        $table.find('thead th').each(function () {
            const columnName = $(this).text().trim();
            columns.push(columnName);
            //console.log(columnName);
        });

        
        // Get table rows data
        $table.find('tbody tr').each(function () {
            const rowData = [];

            $(this).find('td').each(function () {
                rowData.push($(this).text().trim());
            });

            //console.log(rowData);   // logs one row at a time
            data.push(rowData);
        });

        
        return {
            columns: columns,
            data: data
        };
    }

    function approvalFunction(status) {
        console.log('Approval');
        const params = getUrlParameters();
        return new Promise((resolve, reject) => {
            // First, get the destination table name by analysis
            $.ajax({
                url: AppUrls.getByAnalysisAsync,
                type: 'GET',
                data: { analysisName: params.analysis },
                success: function (response) {

                    const sourceTable_ = response.sourceTable || response.tableName || response.table;
                    const destinationTable = response.destinationTable || null; 

                    if (destinationTable) {
                        // Capture staging table content
                        const tableData = captureStagingTableData();
                        console.log(tableData);
                        $.ajax({
                            url: AppUrls.setApproval,
                            type: 'POST',
                            data: {
                                sourceTable: sourceTable_,
                                table: destinationTable,
                                amethystJob: params.amethystJob,
                                analysis: params.analysis,
                                analysisTrial: params.analysisTrial,
                                analyzedBy: params.analyzedBy,
                                status: status,
                                tableContent: JSON.stringify(tableData)
                            },
                            success: function (dataResponse) {
                                if (dataResponse.success) {
                                    ToastrHelper.notification("info", `Status ${status} applied successfully`, "Information");
                                    $('#approve-btn').prop('disabled', true).addClass('disabled');
                                    $('#reject-btn').prop('disabled', true).addClass('disabled');
                                    resolve();
                                } else {
                                    ToastrHelper.notification("error", dataResponse.message || "Error occurred", "Error");
                                    reject(dataResponse.message);
                                }
                            },
                            error: function (xhr, status, error) {
                                console.log(dataResponse); 
                                ToastrHelper.notification("error", "Error fetching staging data: " + error, "Error");
                                reject(error);
                            }
                        });
                    } else {
                        ToastrHelper.notification("error", "No source table found for analysis: " + params.analysis, "Error");
                        reject("No source table found");
                    }
                },
                error: function (xhr, status, error) {
                    ToastrHelper.notification("error", "Error fetching analysis data: " + error, "Error");
                    reject(error);
                }
            });
        });
    }

    function bindEvents() {
        $(document).on('click', '#reject-btn', function (e) {
            e.preventDefault();

            const status = 'REJECTED';
            approvalFunction(status)
        });

        $(document).on('click', '#approve-btn', function (e) {
            e.preventDefault();

            const status = 'PASSED';
            approvalFunction(status)
        });
    }

    return {
        init: function () {
            // Use global form preparation for staging page
            GlobalFormPrep.standardPage([
                // Include staging data initialization as a loading function
                initStagingData()
            ], {
                loadingMessage: 'Loading Staging data...',
                loadingSubtitle: 'Please wait while we load Staging data and prepare the interface',
                onAfterLoad: function (results) {
                    // Bind events after data is fully loaded
                    bindEvents();
                },
                onError: function (error) {
                    console.error('Failed to prepare Staging data page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    StagingModule.init();
});
