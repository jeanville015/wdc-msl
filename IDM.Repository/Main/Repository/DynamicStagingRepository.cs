using Dapper;
using IDM.Data;
using IDM.Model.Common;
using IDM.Model.Main;
using IDM.Model.Maintenance;
using IDM.Model.User;
using IDM.Repository.Main.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Main.Repository
{
    public class DynamicStagingRepository : IDynamicStagingRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DynamicStagingRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<DynamicStaging>> GetByJobAndAnalysisAsync(string table, string amethystJob, string analysis, int analysisTrial)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    var safeTable = FormatSqlIdentifier(table, nameof(table));
                    var sql = $"SELECT * FROM {safeTable} WHERE AmethystJob = @amethystJob AND Analysis = @analysis AND AnalysisTrial = @analysisTrial";
                    
                    // Get dynamic data first
                    var dynamicData = await connection.QueryAsync(sql, new { amethystJob, analysis, analysisTrial });
                    
                    // Convert to DynamicStaging objects
                    var result = new List<DynamicStaging>();
                    foreach (var row in dynamicData)
                    {
                        result.Add(ConvertToDynamicStaging(row));
                    }
                    return result;
                }
                catch(Exception ex)
                { 
                    throw; 
                }
            }
        }

        public async Task<IEnumerable<DynamicStaging>> GetDataStagingDefectAsync(string table, string amethystJob, string analysis, int analysisTrial, string area, string subArea)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    var safeTable = FormatSqlIdentifier(table, nameof(table));
                    var sql = $"SELECT * FROM {safeTable} " +
                        $"WHERE AmethystJob = @amethystJob " +
                        $"AND Analysis = @analysis " +
                        $"AND AnalysisTrial = @analysisTrial " +
                        $"AND Area = @area " +
                        $"AND SubArea = @subArea";

                    // Get dynamic data first
                    var dynamicData = await connection.QueryAsync(sql, new { amethystJob, analysis, analysisTrial, area, subArea });

                    // Convert to DynamicStaging objects
                    var result = new List<DynamicStaging>();
                    foreach (var row in dynamicData)
                    {
                        result.Add(ConvertToDynamicStaging(row));
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<DataTable> GetDataStagingDefectDataTableAsync(string table, string amethystJob, string analysis, int analysisTrial, string area = null, string subArea = null)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    var safeTable = FormatSqlIdentifier(table, nameof(table));
                    var sql = $"SELECT * FROM {safeTable} " +
                        $"WHERE AmethystJob = @amethystJob " +
                        $"AND Analysis = @analysis " +
                        $"AND AnalysisTrial = @analysisTrial " +
                        $"AND (Area = @area OR (@area IS NULL)) " +
                        $"AND (SubArea = @subArea OR (@subArea IS NULL))";

                    // Get dynamic data first
                    var dynamicData = await connection.QueryAsync(sql, new { amethystJob, analysis, analysisTrial, area, subArea });

                    // Execute the query and get a data reader
                    using (var reader = await connection.ExecuteReaderAsync(sql, new { amethystJob, analysis, analysisTrial, area, subArea }))
                    {
                        var dataTable = new DataTable();
                        // Load the schema and data directly into the DataTable
                        dataTable.Load(reader);
                        return dataTable;
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<DynamicStaging>> GetDataStagingDefectWithDataStagingWImageParamsAsync(string table, string amethystJob, string analysis, int analysisTrial)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    var safeTable = FormatSqlIdentifier(table, nameof(table));
                    var sql = $"SELECT * FROM {safeTable} WHERE AmethystJob = @amethystJob AND Analysis = @analysis AND AnalysisTrial = @analysisTrial";

                    // Get dynamic data first
                    var dynamicData = await connection.QueryAsync(sql, new { amethystJob, analysis, analysisTrial });

                    // Convert to DynamicStaging objects
                    var result = new List<DynamicStaging>();
                    foreach (var row in dynamicData)
                    {
                        result.Add(ConvertToDynamicStaging(row));
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<DataTable> GetByJobAndAnalysisDataTableAsync(string table, string amethystJob, string analysis, int analysisTrial)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    var safeTable = FormatSqlIdentifier(table, nameof(table));
                    var sql = $"SELECT * FROM {safeTable} WHERE AmethystJob = @amethystJob AND Analysis = @analysis AND AnalysisTrial = @analysisTrial";

                    // Get dynamic data first
                    var dynamicData = await connection.QueryAsync(sql, new { amethystJob, analysis, analysisTrial });

                    // Execute the query and get a data reader
                    using (var reader = await connection.ExecuteReaderAsync(sql, new { amethystJob, analysis, analysisTrial }))
                    {
                        var dataTable = new DataTable();
                        // Load the schema and data directly into the DataTable
                        dataTable.Load(reader);
                        return dataTable;
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private DynamicStaging ConvertToDynamicStaging(dynamic row)
        {
            var staging = new DynamicStaging();
            
            // Get all properties from the dynamic row
            var rowDict = (IDictionary<string, object>)row;
            
            // Core properties that map directly
            var coreProperties = new[]
            {
                "AmethystJob", "Analysis"
            };

            foreach (var prop in coreProperties)
            {
                if (rowDict.ContainsKey(prop) && rowDict[prop] != null)
                {
                    var property = typeof(DynamicStaging).GetProperty(prop);
                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(staging, rowDict[prop]?.ToString());
                    }
                }
            }

            // Add remaining properties to AdditionalProperties
            foreach (var kvp in rowDict)
            {
                if (!coreProperties.Contains(kvp.Key) && kvp.Value != null)
                {
                    staging.AdditionalProperties[kvp.Key] = kvp.Value;
                }
            }

            return staging;
        }

        public async Task<bool> UpdateStagingDataValueAsync(string Table, DataIdValuePair _DataIdValuePair, DataNameValuePair _DataNameValuePair)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    var safeTable = FormatSqlIdentifier(Table, nameof(Table));
                    var safeDataName = FormatSqlIdentifier(_DataNameValuePair.DataName, nameof(_DataNameValuePair.DataName));
                    var safeIdName = FormatSqlIdentifier(_DataIdValuePair.IdName, nameof(_DataIdValuePair.IdName));

                    var updateSql = $@"
                        UPDATE {safeTable}
                        SET {safeDataName} = @DataValue
                        WHERE {safeIdName} = @IdValue";

                    var updateResult = await connection.ExecuteAsync(updateSql, new
                    {
                        Table,
                        _DataNameValuePair.DataValue,
                        _DataIdValuePair.IdValue
                    });

                    // If tableData contains additional data, you might want to insert/update destination table
                    // This would depend on your specific business logic
                    // For now, we'll just return true if the update was successful
                    return updateResult > 0;
                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    throw new Exception($"Error table updating: {ex.Message}", ex);
                }
            }
        }

        public async Task<bool> SetApprovalAsync(string table, string amethystJob, string analysis, int analysisTrial, string status, string toolName, object tableData, string updatedBy)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    // Update existing records with approval status
                    var updateSql = $@"
                        UPDATE DATA_ENTRYITEMS 
                        SET Status = @status,
                            DateReviewed = GETDATE(),
                            ReviewedBy = @updatedBy,
                            UpdatedTs = GETDATE(),
                            UpdatedBy = @updatedBy
                        WHERE AmethystJob = @amethystJob 
                        AND Analysis = @analysis 
                        AND AnalysisTrial = @analysisTrial";

                    var updateResult = await connection.ExecuteAsync(updateSql, new 
                    { 
                        status, 
                        amethystJob, 
                        analysis, 
                        analysisTrial,
                        updatedBy 
                    });

                    // If tableData contains additional data, you might want to insert/update destination table
                    // This would depend on your specific business logic
                    // For now, we'll just return true if the update was successful
                    return updateResult > 0;
                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    throw new Exception($"Error setting approval status: {ex.Message}", ex);
                }
            }
        }

        public async Task<string> GetCustomerAsync(string amethystJob, string analysis, int analysisTrial)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    var sql = $"SELECT TOP 1 Customer FROM DATA_ENTRYITEMS WHERE AmethystJob = @amethystJob AND Analysis = @analysis AND AnalysisTrial = @analysisTrial";
                    
                    var customer = await connection.QuerySingleOrDefaultAsync<string>(sql, new { amethystJob, analysis, analysisTrial });
                    
                    return customer ?? string.Empty;
                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    throw new Exception($"Error getting customer: {ex.Message}", ex);
                }
            }
        }

        public async Task<string> GetStatusAsync(string amethystJob, string analysis, int analysisTrial)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    var sql = $"SELECT TOP 1 Status FROM DATA_ENTRYITEMS WHERE AmethystJob = @amethystJob AND Analysis = @analysis AND AnalysisTrial = @analysisTrial";
                    
                    var status = await connection.QuerySingleOrDefaultAsync<string>(sql, new { amethystJob, analysis, analysisTrial });
                    
                    return status ?? "PENDING";
                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    throw new Exception($"Error getting status: {ex.Message}", ex);
                }
            }
        }

        public async Task<IEnumerable<string>> GetUserListByAnalysisAsync(string analysisName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    var sql = @"
                        SELECT [U_List].[USER_ID]
                        FROM [USER_ANALYSIS] AS [U_Analysis]
                        INNER JOIN [USER_LIST] AS [U_List] ON [U_Analysis].[USER_ID] = [U_List].[ID]
                        INNER JOIN [MAINT_Analysis] AS [M_Analysis] ON [U_Analysis].[MAINT_ANALYSIS_ID] = [M_Analysis].[AnalysisId]
                        WHERE [M_Analysis].[AnalysisName] = @analysisName";

                    return (await connection.QueryAsync<string>(sql, new { analysisName })).ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private static string FormatSqlIdentifier(string identifier, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("SQL identifier cannot be empty.", argumentName);
            }

            var parts = identifier.Split('.');
            if (parts.Length > 2)
            {
                throw new ArgumentException("SQL identifier cannot contain more than one schema separator.", argumentName);
            }

            return string.Join(".", parts.Select(part =>
            {
                var trimmed = part.Trim();
                if (trimmed.StartsWith("[") && trimmed.EndsWith("]") && trimmed.Length > 2)
                {
                    trimmed = trimmed.Substring(1, trimmed.Length - 2);
                }

                if (trimmed.Length == 0 || trimmed.Any(c => !(char.IsLetterOrDigit(c) || c == '_')))
                {
                    throw new ArgumentException($"Invalid SQL identifier: {identifier}", argumentName);
                }

                return $"[{trimmed}]";
            }));
        }

    }
}
