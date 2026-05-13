using AutoMapper;
using IDM.DTO;
using IDM.DTO.Main;
using IDM.DTO.Maintenance;
using IDM.Model.Main;
using IDM.Repository.Main.Interface;
using IDM.Service.Main.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IDM.Service.Main.Service
{
    public class IncomingDataService : IIncomingDataService
    {
        private readonly IIncomingDataRepository _incomingDataRepository;
        private readonly IMapper _Mapper;

        public IncomingDataService(IIncomingDataRepository incomingDataRepository, IMapper mapper)
        {
            _incomingDataRepository = incomingDataRepository;
            _Mapper = mapper;
        }

        public async Task<int> CreateAsync(IncomingDataDTO incomingDataDTO)
        {
            var incomingData = _Mapper.Map<IncomingData>(incomingDataDTO);
            return await _incomingDataRepository.CreateAsync(incomingData);
        }

        public async Task<int> CreateTrial(IncomingDataDTO incomingDataDTO)
        {
            var incomingData = _Mapper.Map<IncomingData>(incomingDataDTO);

            await _incomingDataRepository.RetireTrial(incomingData);
            return await _incomingDataRepository.CreateTrial(incomingData);
        }

        public async Task<int> MQUploadPreparationParameter(IncomingDataDTO incomingDataDTO, ConfigDTO configDTO)
        {
            return await CreateAndUploadMQTable(incomingDataDTO, configDTO, configDTO.MQTransaction, includeMainData: true, useParameters: true);
        }

        public async Task<int> MQUploadPreparationTrial(IncomingDataDTO incomingDataDTO, ConfigDTO configDTO)
        {
            return await CreateAndUploadMQTable(incomingDataDTO, configDTO, configDTO.MQTransactionTrial, includeMainData: false, useParameters: false);
        }

        private async Task<int> CreateAndUploadMQTable(IncomingDataDTO incomingDataDTO, ConfigDTO configDTO, string transaction, bool includeMainData, bool useParameters)
        {
            var incomingData = _Mapper.Map<IncomingData>(incomingDataDTO);
            var dataTable = new DataTable();

            // Add columns based on configuration
            AddColumnsToTable(dataTable, configDTO, includeMainData, useParameters);

            // Add rows based on data source
            if (useParameters && incomingDataDTO.Parameters?.Any() == true)
            {
                AddParameterRows(dataTable, incomingDataDTO, configDTO);
            }
            else if (!useParameters)
            {
                var trialData = await _incomingDataRepository.GetTrialAsync(incomingData);
                
                // End process if no trial data returned
                if (trialData?.Any() != true)
                {
                    return 0; // Return error code to indicate no trial data
                }
                
                AddTrialRows(dataTable, trialData, configDTO);
            }
            else
            {
                // Create empty row if no data
                dataTable.Rows.Add(dataTable.NewRow());
            }

            return await MQUpload(dataTable, configDTO, transaction);
        }

        private void AddColumnsToTable(DataTable table, ConfigDTO configDTO, bool includeMainData, bool useParameters)
        {
            // Add main IncomingDataDTO columns if needed
            if (includeMainData)
            {
                var mainProperties = typeof(IncomingDataDTO)
                    .GetProperties()
                    .Where(p => p.PropertyType == typeof(string) || p.PropertyType.IsValueType);

                foreach (var prop in mainProperties)
                {
                    var columnName = FormatColumnName(prop.Name, configDTO);
                    if (!configDTO.MQExcludeColumn.Contains(columnName))
                        table.Columns.Add(columnName, typeof(string));
                }
            }

            // Add parameter or trial columns
            Type dataType = useParameters ? typeof(ParameterDetailDTO) : null;
            if (useParameters)
            {
                var paramProperties = typeof(ParameterDetailDTO)
                    .GetProperties()
                    .Where(p => p.PropertyType == typeof(string) || p.PropertyType.IsValueType);

                foreach (var prop in paramProperties)
                {
                    var columnName = FormatColumnName(prop.Name, configDTO);
                    if (!configDTO.MQExcludeColumn.Contains(columnName))
                        table.Columns.Add(columnName, typeof(string));
                }
            }
        }

        private void AddParameterRows(DataTable table, IncomingDataDTO incomingDataDTO, ConfigDTO configDTO)
        {
            var mainProperties = typeof(IncomingDataDTO)
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string) || p.PropertyType.IsValueType);

            var paramProperties = typeof(ParameterDetailDTO)
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string) || p.PropertyType.IsValueType);

            foreach (var parameter in incomingDataDTO.Parameters)
            {
                var row = table.NewRow();

                // Add main data
                foreach (var prop in mainProperties)
                {
                    var columnName = FormatColumnName(prop.Name, configDTO);
                    if (!configDTO.MQExcludeColumn.Contains(columnName))
                    {
                        var value = prop.GetValue(incomingDataDTO);
                        row[columnName] = value?.ToString() ?? string.Empty;
                    }
                }

                // Add parameter data
                foreach (var prop in paramProperties)
                {
                    var columnName = FormatColumnName(prop.Name, configDTO);
                    if (!configDTO.MQExcludeColumn.Contains(columnName))
                    {
                        var value = prop.GetValue(parameter);
                        row[columnName] = value?.ToString() ?? string.Empty;
                    }
                }

                table.Rows.Add(row);
            }
        }

        private void AddTrialRows(DataTable table, IEnumerable<object> trialData, ConfigDTO configDTO)
        {
            if (trialData?.Any() != true)
            {
                // Create empty row if no trial data
                table.Rows.Add(table.NewRow());
                return;
            }

            var trialProperties = trialData.First().GetType()
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string) || p.PropertyType.IsValueType);

            foreach (var trial in trialData)
            {
                var row = table.NewRow();

                foreach (var prop in trialProperties)
                {
                    var columnName = FormatColumnName(prop.Name, configDTO);
                    if (!configDTO.MQExcludeColumn.Contains(columnName))
                    {
                        var value = prop.GetValue(trial);

                        // Format date fields
                        if (columnName.Contains("DELIVERYDATE") || columnName.Contains("RECEIVEDDATE"))
                        {
                            if (value != null && DateTime.TryParse(value.ToString(), out DateTime dateValue))
                            {
                                row[columnName] = dateValue.ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                row[columnName] = value?.ToString() ?? string.Empty;
                            }
                        }
                        else
                        {
                            row[columnName] = value?.ToString() ?? string.Empty;
                        }
                    }
                }

                table.Rows.Add(row);
            }
        }

        private string FormatColumnName(string propertyName, ConfigDTO configDTO)
        {
            var columnName = propertyName.Replace("_", "").ToUpper();
            if (columnName.Equals("MATERIALNO"))
                columnName = "MATERIALNUMBER";
            if (configDTO.MQAdjustColumn.Contains(columnName.Replace("NAME", "")))
                columnName = columnName.Replace("NAME", "");
            return columnName;
        }

        public async Task<int> MQUpload(DataTable MQDataTable, ConfigDTO configDTO, string transaction)
        {
            try
            {
                foreach (DataRow row in MQDataTable.Rows)
                {
                    PDBAXLib.PdbClass PDB = new PDBAXLib.PdbClass();
                    PDB.init(transaction, configDTO.MQVersion, configDTO.MQConnectionFile);
                    while (PDB.reupload()) ;
                    PDB.format("Detail");

                    foreach (DataColumn col in MQDataTable.Columns)
                    {
                        if (!string.IsNullOrEmpty(row[col].ToString()))
                            PDB.field(col.ColumnName, row[col].ToString());
                        else if (row[col].ToString().Equals(" "))
                            PDB.field(col.ColumnName, row[col].ToString());
                    }
                    PDB.formatEnd("Detail");
                    PDB.transmit(null);
                }
            }
            catch (Exception ex)
            {
                string strerr = ex.Message;
            }
            return 1;
        }
    }
}
