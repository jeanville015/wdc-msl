using AutoMapper;
using IDM.DTO;
using IDM.DTO.Main;
using IDM.DTO.Maintenance;
using IDM.DTO.User;
using IDM.Model;
using IDM.Model.Common;
using IDM.Model.Main;
using IDM.Model.Maintenance;
using IDM.Repository.Main.Interface;
using IDM.Repository.Main.Repository;
using IDM.Repository.User.Interface;
using IDM.Repository.User.Repository;
using IDM.Service.Main.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using IDM.Service.DataAccess;

namespace IDM.Service.Main.Service
{
    public class PendingDataService : IPendingDataService
    {
        private readonly EDCSPC _edcspc = new EDCSPC();
        private readonly IPendingDataRepository _pendingDataRepository;
        private readonly IMapper _Mapper;

        public PendingDataService(IPendingDataRepository pendingDataRepository, IMapper mapper)
        {
            _pendingDataRepository = pendingDataRepository;
            _Mapper = mapper;
        }
         
        public async Task<PagedResultDTO<PendingDataDTO>> GetAllAsync(int page, int pageSize)
        {

            try
            {
                var result = await _pendingDataRepository.GetAllAsync(page, pageSize);

                var mappedItems = result.Items
                    .Select(entity => _Mapper.Map<PendingDataDTO>(entity))
                    .ToList();

                return new PagedResultDTO<PendingDataDTO>
                {
                    Items = mappedItems,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
                    PageSize = pageSize,
                    TotalCount = result.TotalCount
                };
            }
            catch (Exception ex)
            {
                throw;
            } 
        }
         
        public async Task<IEnumerable<PendingDataDTO>> GetPendingDataDetailsAsync(string deliveryDate, string receivedDate, string lotNumber, string materialNo, string jobNumber, string toolId)
        {  
            var data = await _pendingDataRepository.GetPendingDataDetailsAsync(deliveryDate, receivedDate, lotNumber, materialNo, jobNumber, toolId);
            return _Mapper.Map<IEnumerable<PendingDataDTO>>(data);
        }
        public async Task<PagedResultDTO<PendingDataDTO>> GetPendingDataDetailsPaginatedAsync(string deliveryDate, string receivedDate, string lotNumber, string materialNo, string jobNumber, string toolId, int page, int pageSize)
        {

            try
            {
                var result = await _pendingDataRepository.GetPendingDataDetailsPaginatedAsync(deliveryDate, receivedDate, lotNumber, materialNo, jobNumber, toolId, page, pageSize);

                var mappedItems = result.Items
                    .Select(entity => _Mapper.Map<PendingDataDTO>(entity))
                    .ToList();

                return new PagedResultDTO<PendingDataDTO>
                {
                    Items = mappedItems,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
                    PageSize = pageSize,
                    TotalCount = result.TotalCount
                };
            }
            catch (Exception ex)
            {
                throw;
            } 
        }

        public async Task<IEnumerable<PendingDataDTO>> GetPendingDataAsync()
        {
            var data = await _pendingDataRepository.GetPendingDataAsync();
            return _Mapper.Map<IEnumerable<PendingDataDTO>>(data);
        }

        public async Task<int> UpdateDataParameterDetails(string status, string lotNumber, string deliveryDate, string receivedDate, string materialNo, string jobNumber, string toolId)
        {
            return await _pendingDataRepository.UpdateDataParameterDetails(status, lotNumber, deliveryDate, receivedDate, materialNo, jobNumber, toolId);
        }

        public async Task<int> MQUploadPreparationParameter(ConfigDTO configDTO, IEnumerable<PendingDataDTO> pendingDataDTO)
        {
            return await CreateAndUploadMQTable(pendingDataDTO, configDTO, configDTO.MQTransaction, includeMainData: true, useParameters: true);
        }

        public async Task<int> MQUploadPreparationTrial(ConfigDTO configDTO, IEnumerable<PendingDataDTO> pendingDataDTO)
        {
            return await CreateAndUploadMQTable(pendingDataDTO, configDTO, configDTO.MQTransactionTrial, includeMainData: false, useParameters: false);
        }
        private async Task<int> CreateAndUploadMQTable(IEnumerable<PendingDataDTO> pendingDataDTOs, ConfigDTO configDTO, string transaction, bool includeMainData, bool useParameters)
        {
            var dataTable = new DataTable();

            // Add columns based on configuration.
            AddColumnsToTable(
                dataTable,
                configDTO,
                includeMainData,
                useParameters);

            var pendingRows = pendingDataDTOs?.ToList()
                ?? new List<PendingDataDTO>();

            if (useParameters)
            {
                // AddParameterRows already loops through all pending rows,
                // so call it only once.
                AddParameterRows(dataTable, pendingRows,configDTO);
            }
            else
            {
                // Trial retrieval must still be performed for each pending row.
                foreach (var item in pendingRows)
                {
                    var incomingData = new IncomingData
                    {
                        LotNumber = item.LotNumber,
                        Delivery_Date = item.Delivery_Date,
                        Received_Date = item.Received_Date,
                        Material_No = item.Material_No,
                        Job_Number = item.Job_Number,
                        ToolId = item.ToolId
                    };

                    var trialData =
                        await _pendingDataRepository.GetTrialAsync(incomingData);

                    if (trialData?.Any() != true)
                    {
                        return 0;
                    }

                    AddTrialRows( dataTable, trialData, configDTO);
                }
            }

            return await MQUpload( dataTable, configDTO, transaction);
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
            // Trial
            else
            {
                var trialProperties = typeof(ParameterTrial)
                    .GetProperties()
                    .Where(p => p.PropertyType == typeof(string) || p.PropertyType.IsValueType);

                foreach (var prop in trialProperties)
                {
                    var columnName = FormatColumnName(prop.Name, configDTO);
                    if (!configDTO.MQExcludeColumn.Contains(columnName))
                        table.Columns.Add(columnName, typeof(string));
                }
            }
        }
        private static bool IsConfiguredColumn(string configuredColumns, string columnName)
        {
            if (string.IsNullOrWhiteSpace(configuredColumns) ||
                string.IsNullOrWhiteSpace(columnName))
            {
                return false;
            }

            return configuredColumns
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .Any(value => string.Equals(
                    value,
                    columnName,
                    StringComparison.OrdinalIgnoreCase));
        }

        private void AddParameterRows(DataTable table, IEnumerable<PendingDataDTO> pendingDataDTOs, ConfigDTO configDTO)
        {
            var pendingRows = pendingDataDTOs?.ToList()
                ?? new List<PendingDataDTO>();

            if (!pendingRows.Any())
            {
                table.Rows.Add(table.NewRow());
                return;
            }

            var properties = typeof(PendingDataDTO)
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string) ||
                            p.PropertyType.IsValueType);

            foreach (var pendingData in pendingRows)
            {
                var row = table.NewRow();

                foreach (var property in properties)
                {
                    var columnName = GetPendingMqColumnName(
                        property.Name,
                        configDTO);

                    // A null mapping means this property is intentionally
                    // not included in the current MQ schema.
                    if (string.IsNullOrEmpty(columnName))
                        continue;

                    if (IsConfiguredColumn(
                            configDTO.MQExcludeColumn,
                            columnName))
                    {
                        continue;
                    }

                    // Fail with a useful error instead of DataRow's generic
                    // "does not belong to table" exception.
                    if (!table.Columns.Contains(columnName))
                    {
                        throw new InvalidOperationException(
                            $"MQ table column '{columnName}' was not created " +
                            $"for PendingDataDTO property '{property.Name}'.");
                    }

                    var value = property.GetValue(pendingData);

                    row[columnName] =
                        value?.ToString() ?? string.Empty;
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
                    if (!IsConfiguredColumn(configDTO.MQExcludeColumn, columnName))
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

        private string GetPendingMqColumnName(string propertyName, ConfigDTO configDTO)
        {
            switch (propertyName)
            {
                case nameof(PendingDataDTO.Visual_Appearance_Check):
                    // The current MQ schema is generated from
                    // IncomingDataDTO.View_Appearance_Check.
                    return FormatColumnName(
                        nameof(IncomingDataDTO.View_Appearance_Check),
                        configDTO);

                case nameof(PendingDataDTO.Judgement):
                    // Equivalent property in ParameterDetailDTO.
                    // Currently excluded through MQExcludeColumn.
                    return FormatColumnName(
                        nameof(ParameterDetailDTO.Specs_Judgement),
                        configDTO);

                case nameof(PendingDataDTO.InspectionValue):
                    // No equivalent column exists in the current MQ schema.
                    // Keep this excluded unless MQ defines a destination field.
                    return null;

                default:
                    return FormatColumnName(propertyName, configDTO);
            }
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

        public SubmitResult SendEDCSPC(IEnumerable<PendingDataDTO> pendingData, string operatorId)
        {
            if (string.IsNullOrWhiteSpace(operatorId))
            {
                return SubmitResult.Fail(
                    "Operator ID is required for EDCSPC submission.");
            }
            var rows = pendingData?.ToList() ?? new List<PendingDataDTO>();

            if (!rows.Any())
                return SubmitResult.Fail("No pending data found for EDCSPC submission.");

            var first = rows.First();

            var table = new DataTable();

            table.Columns.Add("PROCTOOLINFO");
            table.Columns.Add("LOT NUMBER");
            table.Columns.Add("JOB#");
            table.Columns.Add("MATERIAL#");

            foreach (var item in rows)
            {
                var columnName = BuildEDCSPCParameterColumnName(item);

                if (!table.Columns.Contains(columnName))
                    table.Columns.Add(columnName);
            }

            var row = table.NewRow();

            row["PROCTOOLINFO"] = first.Material_Name;
            row["LOT NUMBER"] = string.IsNullOrWhiteSpace(first.LotNumber) ? "-" : first.LotNumber;
            row["JOB#"] = string.IsNullOrWhiteSpace(first.Job_Number) ? "-" : first.Job_Number;
            row["MATERIAL#"] = first.Material_No;

            foreach (var item in rows)
            {
                row[BuildEDCSPCParameterColumnName(item)] = item.Parameter_Value;
            }

            table.Rows.Add(row);

            var edcspcConfig = new EDCSPCDataSending
            {
                Operator = operatorId,
                Operation = "7000",
                WaferLot = "PQ-7000-*",
                Product = first.Material_Name,
                TimePrepared = DateTime.Now
            };

            edcspcConfig.ChartName.Add(first.Material_Name);
            edcspcConfig.SourceEntity.Add(first.Material_No);

            return _edcspc.Submit(table, edcspcConfig);
        }

        private static string BuildEDCSPCParameterColumnName(PendingDataDTO item)
        {
            return $"{item.Site_Name?.ToUpperInvariant()}_{item.Parameter_Name}";
        }

    }
}
