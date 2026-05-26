var ToolEntryItemsModule = (function () {
    var table;
    var tableAmethyst;
    var toolTypeData;

    function initToolEntryItemsData() {
        return new Promise((resolve, reject) => {
            table = $('#toolEntryItemsTable').DataTable({
                ajax: {
                    url: AppUrls.getData,
                    type: 'GET',
                    dataSrc: '',
                    "error": function (xhr) {
                        ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                        reject(xhr);
                    }
                },
                columns: [
                    { data: 'amethystJob' },
                    {
                        data: null,
                        render: function (data) {
                            return `
                                <a href='#' class="view-btn" 
                                   data-amethyst-job="${data.amethystJob}"
                                   title="View Details">
                                     <i class="fas fa-eye text-info"></i>
                                </a>
                            `;
                        }
                    }
                ],
                processing: false,
                serverSide: false,
                autoWidth: false,
                initComplete: function () {
                    $('#toolEntryItemsTable').fadeIn(300);
                    resolve();
                }
            });
        });
    }

    function tableEntryItemsByAmethyst(params) {
        return new Promise((resolve, reject) => {
            // Register custom sorting function for status column
            jQuery.fn.dataTableExt.oSort['status-custom-asc'] = function(a, b) {
                const statusOrder = { 'pending': 0, 'rejected': 1, 'passed': 2 };
                const aStatus = (a || '').toString().toLowerCase();
                const bStatus = (b || '').toString().toLowerCase();
                
                const aOrder = statusOrder.hasOwnProperty(aStatus) ? statusOrder[aStatus] : 999;
                const bOrder = statusOrder.hasOwnProperty(bStatus) ? statusOrder[bStatus] : 999;
                
                return aOrder - bOrder;
            };
            
            jQuery.fn.dataTableExt.oSort['status-custom-desc'] = function(a, b) {
                const statusOrder = { 'pending': 0, 'rejected': 1, 'passed': 2 };
                const aStatus = (a || '').toString().toLowerCase();
                const bStatus = (b || '').toString().toLowerCase();
                
                const aOrder = statusOrder.hasOwnProperty(aStatus) ? statusOrder[aStatus] : 999;
                const bOrder = statusOrder.hasOwnProperty(bStatus) ? statusOrder[bStatus] : 999;
                
                return bOrder - aOrder;
            };
            
            // Destroy existing DataTable instance if it exists
            if ($.fn.DataTable.isDataTable('#byAmethystTable')) {
                $('#byAmethystTable').DataTable().destroy();
            }
            
            tableAmethyst = $('#byAmethystTable').DataTable({
                ajax: {
                    url: AppUrls.getAllAsyncByAmethystJob, 
                    type: 'GET',
                    data: params,
                    dataSrc: '',
                    "error": function (xhr) {
                        ToastrHelper.notification("error", xhr.responseJSON?.message || "An unexpected error occurred.", "Error");
                        reject(xhr);
                    }
                },
                columnDefs: [
                    {
                        targets: 7, // Status column (index 7)
                        type: 'status-custom',
                        orderSequence: ['asc', 'desc']
                    }
                ],
                columns: [
                    { data: 'amethystJob', className: 'all' },
                    { data: 'analysis', className: 'min-tablet' },
                    { data: 'analysisTrial', className: 'min-tablet' },
                    { data: 'toolName', className: 'min-tablet' },
                    { 
                        data: 'dateAnalyzed',
                        className: 'min-tablet',
                        render: function (data) {
                            if (!data) return '';
                            const date = new Date(data);
                            const options = { month: 'short', day: 'numeric', year: 'numeric' };
                            return date.toLocaleDateString('en-US', options);
                        }
                    },
                    { data: 'analyzedBy', className: 'desktop' },

                    {
                        data: 'dateReviewed',
                        className: 'desktop',
                        render: function (data) {
                            if (!data) return '';
                            const date = new Date(data);
                            const options = { month: 'short', day: 'numeric', year: 'numeric' };
                            return date.toLocaleDateString('en-US', options);
                        }
                    },

                    { data: 'reviewedBy', className: 'desktop' },
                    { data: 'customer', className: 'desktop' },
                    { 
                        data: 'image', 
                        className: 'desktop',
                        render: function(data) {
                            if (data === '1') {
                                return '<i class="fas fa-check text-success"></i>';
                            } else {
                                return '<i class="fas fa-times text-danger"></i>';
                            }
                        }
                    },
                    { data: 'status', className: 'min-tablet',
                    render: function (data) {
                        if (!data) return '';
                        
                        let badgeClass = '';
                        let statusText = data.toString().toLowerCase();
                        
                        switch(statusText) {
                            case 'pending':
                                badgeClass = 'badge badge-warning';
                                break;
                            case 'rejected':
                                badgeClass = 'badge badge-danger';
                                break;
                            case 'passed':
                                badgeClass = 'badge badge-success';
                                break;
                            default:
                                badgeClass = 'badge badge-secondary';
                        }
                        
                        return `<span class="${badgeClass}" style="min-width: 80px; display: inline-block; text-align: center;">${data}</span>`;
                    }
                },
                    { 
                        data: null,
                        className: 'all',
                        render: function (data) {
                            let approvalButton = '';
                            if (data.status && data.status.toString().toLowerCase() === 'pending') {
                                approvalButton = `
                                    <a href='#' class="approval-btn ms-2"
                                       data-id="${data.analysisTrial}"
                                       data-analysis="${data.analysis}"
                                       data-amethyst-job="${data.amethystJob}"
                                       data-analyzed-by="${data.analyzedBy}"
                                       data-image="${data.image}"
                                       title="Approval">
                                        <i class="fa-solid fa-share"></i>
                                    </a>
                                `;
                            }
                            
                            return `
                                <a href='#' class="view-detailed-btn"
                                   data-id="${data.analysisTrial}"
                                   data-analysis="${data.analysis}"
                                   data-amethyst-job="${data.amethystJob}"
                                   title="View Details">
                                     <i class="fas fa-eye text-info"></i>
                                </a>
                                ${approvalButton}
                            `;
                        }
                    }
                ],
                responsive: {
                    details: {
                        type: 'column'
                    }
                },
                autoWidth: false,
                processing: false,
                serverSide: false,
                order: [[7, 'asc']], // Sort by status column (index 7) ascending to prioritize PENDING
                initComplete: function () {
                    $('#byAmethystTable').fadeIn(300);
                    resolve();
                }
            });
        });
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
            if (judgement === 'passed') {
                return '<span class="badge badge-success">' + cellValue + '</span>';
            } else if (judgement === 'failed') {
                return '<span class="badge badge-danger">' + cellValue + '</span>';
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

    function getSourceTableByAnalysis(params) {
        // First, get the source table name by analysis
        $.ajax({
            url: AppUrls.getByAnalysisAsync,
            type: 'GET',
            data: { analysisName: params.analysis },
            success: function (response) {
                // Extract source table name from response
                const sourceTable = response.sourceTable || response.tableName || response.table;
                
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
                            // Clear existing table content
                            const $table = $('#detailedModal #byAmethystTable');
                            $table.find('thead').empty();
                            $table.find('tbody').empty();
                            
                            // Create dynamic table based on data structure
                            if (dataResponse && dataResponse.length > 0) {
                                // Destroy existing DataTable instance if it exists
                                if ($.fn.DataTable.isDataTable('#byAmethystTable')) {
                                    $('#byAmethystTable').DataTable().destroy();
                                }

                                const columns = buildColumnList(dataResponse[0]);
                                renderTableHeader($table, columns);
                                renderTableRows($table, dataResponse, columns);
                            } else {
                                // Show no data message
                                $table.find('thead').append('<tr><th>No Data</th></tr>');
                                $table.find('tbody').append('<tr><td>No data available</td></tr>');
                            }
                            
                            $('#viewModal').modal('hide');
                            $('#detailedModal').modal('show');
                        },
                        error: function (xhr, status, error) {
                            ToastrHelper.notification("error", "Error fetching job and analysis data: " + error, "Error");
                        }
                    });
                } else {
                    ToastrHelper.notification("error", "No source table found for analysis: " + params.analysis, "Error");
                }
            },
            error: function (xhr, status, error) {
                ToastrHelper.notification("error", "Error fetching analysis data: " + error, "Error");
            }
        });
    }

    function bindEvents() {
        $(document).on('click', '.view-btn', function(e) {
            e.preventDefault();
            const btn = $(this);
            
            
            const params = {
                amethystJob: btn.data('amethyst-job')
            };

            tableEntryItemsByAmethyst(params)
            $('#viewModal').modal('show');
        });

        $(document).on('click', '.view-detailed-btn', function (e) {
            e.preventDefault();
            const btn = $(this);


            const params = {
                analysisTrial: btn.data('id'),
                analysis: btn.data('analysis'),
                amethystJob: btn.data('amethyst-job')
            };

            getSourceTableByAnalysis(params);

            
        });

        $(document).on('click', '.approval-btn', function (e) {
            e.preventDefault();
            const btn = $(this);

            const params = {
                analysisTrial: btn.data('id'),
                analysis: btn.data('analysis'),
                amethystJob: btn.data('amethyst-job'),
                analyzedBy: btn.data('analyzed-by'),
                source: 'wdcx01' 
            };
      
            const queryString = $.param(params);

            //make a decision here whether to load 2KX Staging or not
            if (btn.data('image') === 1) {
                window.location.href = 'Staging/WImage?' + queryString;
                //console.log('logging: ' + 'Staging/WImage?' + queryString + '; image:' + btn.data('image'));
            } else {
                window.location.href = 'Staging?' + queryString;
                //console.log('logging: ' + 'Staging?' + queryString + '; image:' + btn.data('image'));
            }
        });


        $('#close-view, #close-view-btn').click(() => {
            $('#viewModal').modal('hide');
        });

        $('#close-view-detailed, #close-view-detailed-btn').click(() => {
            $('#detailedModal').modal('hide');
        });
    }


    return {
        init: function () {
            // Use global form preparation for role page
            GlobalFormPrep.standardPage([
                // Include DataTable initialization as a loading function
                initToolEntryItemsData()
            ], {
                loadingMessage: 'Loading Tool Entry Items list...',
                loadingSubtitle: 'Please wait while we load Tool Entry Items and prepare the interface',
                onAfterLoad: function (results) {
                    // Bind events after DataTable is fully loaded
                    bindEvents();
                },
                onError: function (error) {
                    console.error('Failed to prepare Tool Entry Items page:', error);
                    // Still try to initialize the page even if loading fails
                    bindEvents();
                }
            });
        }
    };
})();



$(document).ready(function () {
    ToolEntryItemsModule.init();
});
