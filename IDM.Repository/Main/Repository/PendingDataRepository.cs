using Dapper;
using IDM.Data;
using IDM.Model.Main;
using IDM.Model.Maintenance;
using IDM.Model.User;
using IDM.Repository.Main.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace IDM.Repository.Main.Repository
{
    public class PendingDataRepository : IPendingDataRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        //private readonly IDb2ConnectionFactory _db2ConnectionFactory;

        public PendingDataRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory; 
        }

        //public async Task<IEnumerable<PendingData>> GetAllAsync(int page = 1, int pageSize = 10)
        //{
        //    using (var connection = _connectionFactory.CreateConnection())
        //    {
        //        //var sql = "SELECT DISTINCT MATERIALNUMBER, MATERIALNAME, LOTNUMBER, DELIVERYDATE, RECEIVEDDATE, JOBNUMBER, TOOLID FROM IDM.MATERIALINFO  WITH UR ";
        //        try
        //        {
        //            var sql = "SELECT DISTINCT [MATERIAL_NO], [MATERIAL_NAME], [DELIVERY_DATE], [RECEIVED_DATE], [JOB_NUMBER], [TOOLID]   FROM [IDM ].[dbo].[DATA_PARAMETER_DETAILS] WHERE [STATUS]='PENDING' ";
        //            return await connection.QueryAsync<PendingData>(sql);
        //        }
        //        catch (Exception ex)
        //        { throw; }
        //    }
        //}

        public async Task<IEnumerable<PendingData>> GetPendingDataDetailsAsync(string deliveryDate, string receivedDate, string lotNumber, string materialNo, string jobNumber, string toolId)
        {
            // Parse date strings to DateTime objects
            DateTime dtDeliveryDate = DateTime.ParseExact(deliveryDate, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime dtReceivedDate = DateTime.ParseExact(receivedDate, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            string strDeliveryDate = dtDeliveryDate.ToString("yyyy-MM-dd");
            string strReceivedDate = dtReceivedDate.ToString("yyyy-MM-dd");

            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                { 
                    var sql = new StringBuilder("SELECT * FROM [IDM].[DBO].[DATA_PARAMETER_DETAILS] WHERE [STATUS]='PENDING'");
                    var parameters = new DynamicParameters();


                    sql.Append(" AND [DELIVERY_DATE] = @DeliveryDate");
                    parameters.Add("DeliveryDate", strDeliveryDate, DbType.String);

                    sql.Append(" AND [RECEIVED_DATE] = @ReceivedDate");
                    parameters.Add("ReceivedDate", strReceivedDate, DbType.String);

                    // Append string filters
                    sql.Append(" AND [LOT_NUMBER] = @LotNumber");
                    parameters.Add("LotNumber", lotNumber, DbType.String);

                    sql.Append(" AND [MATERIAL_NO] = @MaterialNo");
                    parameters.Add("MaterialNo", materialNo, DbType.String);

                    sql.Append(" AND [JOB_NUMBER] = @JobNumber");
                    parameters.Add("JobNumber", jobNumber, DbType.String);

                    sql.Append(" AND [TOOL_ID] = @ToolId");
                    parameters.Add("ToolId", toolId, DbType.String);

                    return await connection.QueryAsync<PendingData>(sql.ToString(), parameters);
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it appropriately
                    throw;
                }
            }
        }

        public async Task<(IEnumerable<PendingData> Items, int TotalCount)> GetPendingDataDetailsPaginatedAsync(string deliveryDate, string receivedDate, string lotNumber, string materialNo, string jobNumber, string toolId, int page, int pageSize)
        {
            //parse date string to correct format yyyy-mm-dd
            DateTime dtDeliveryDate = DateTime.ParseExact(deliveryDate, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime dtReceivedDate = DateTime.ParseExact(receivedDate, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            string strDeliveryDate = dtDeliveryDate.ToString("yyyy-MM-dd");
            string strReceivedDate = dtReceivedDate.ToString("yyyy-MM-dd");

            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    var sql = @"
                                SELECT 
                                    [MATERIAL_NO], [MATERIAL_NAME], [DELIVERY_DATE], 
                                    [RECEIVED_DATE], [JOB_NUMBER], [TOOLID], [LOTNUMBER]
                                FROM [IDM].[dbo].[DATA_PARAMETER_DETAILS]
                                WHERE [STATUS]       = 'PENDING'
                                  AND (@DeliveryDate = '' OR [DELIVERY_DATE] = @DeliveryDate)
                                  AND (@ReceivedDate = '' OR [RECEIVED_DATE] = @ReceivedDate)
                                  AND (@LotNumber    = '' OR [LOTNUMBER]     = @LotNumber)
                                  AND (@MaterialNo   = '' OR [MATERIAL_NO]    = @MaterialNo)
                                  AND (@JobNumber    = '' OR [JOB_NUMBER]     = @JobNumber)
                                  AND (@ToolId       = '' OR [TOOLID]         = @ToolId)
                                ORDER BY [DELIVERY_DATE] DESC
                                OFFSET (@Page - 1) * @PageSize ROWS
                                FETCH NEXT @PageSize ROWS ONLY;

                                SELECT COUNT(*) 
                                FROM [IDM].[dbo].[DATA_PARAMETER_DETAILS]
                                WHERE [STATUS]       = 'PENDING'
                                  AND (@DeliveryDate = '' OR [DELIVERY_DATE] = @DeliveryDate)
                                  AND (@ReceivedDate = '' OR [RECEIVED_DATE] = @ReceivedDate)
                                  AND (@LotNumber    = '' OR [LOTNUMBER]     = @LotNumber)
                                  AND (@MaterialNo   = '' OR [MATERIAL_NO]    = @MaterialNo)
                                  AND (@JobNumber    = '' OR [JOB_NUMBER]     = @JobNumber)
                                  AND (@ToolId       = '' OR [TOOLID]         = @ToolId);";

                    using (var multi = await connection.QueryMultipleAsync(sql, new 
                    { 
                        DeliveryDate = strDeliveryDate,
                        ReceivedDate = strReceivedDate,
                        LotNumber = lotNumber,
                        MaterialNo = materialNo,
                        JobNumber = jobNumber,
                        ToolId = toolId,
                        Page = page, 
                        PageSize = pageSize
                    }))
                    {
                        var items = await multi.ReadAsync<PendingData>();
                        var totalCount = await multi.ReadFirstAsync<int>();
                        return (items, totalCount);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<(IEnumerable<PendingData> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    var sql = @"
                                SELECT DISTINCT
                                    [MATERIAL_NO], [MATERIAL_NAME], [DELIVERY_DATE], 
                                    [RECEIVED_DATE], [JOB_NUMBER], [TOOLID], [LOTNUMBER]
                                FROM [IDM].[dbo].[DATA_PARAMETER_DETAILS]
                                WHERE [STATUS] = 'PENDING'
                                ORDER BY [DELIVERY_DATE] DESC
                                OFFSET (@Page - 1) * @PageSize ROWS
                                FETCH NEXT @PageSize ROWS ONLY;

                                SELECT COUNT(*) FROM (
                                    SELECT DISTINCT
                                        [MATERIAL_NO], [MATERIAL_NAME], [DELIVERY_DATE], 
                                        [RECEIVED_DATE], [JOB_NUMBER], [TOOLID], [LOTNUMBER]
                                    FROM [IDM].[dbo].[DATA_PARAMETER_DETAILS]
                                    WHERE [STATUS] = 'PENDING'
                                ) AS DistinctRows;";

                    using (var multi = await connection.QueryMultipleAsync(sql, new { Page = page, PageSize = pageSize }))
                    {
                        var items = await multi.ReadAsync<PendingData>();
                        var totalCount = await multi.ReadFirstAsync<int>();
                        return (items, totalCount);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<PendingData>> GetPendingDataAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    var sql = new StringBuilder("SELECT * [IDM].[DBO].[DATA_PARAMETER_DETAILS] WHERE [STATUS]='PENDING'");
                    var parameters = new DynamicParameters(); 

                    return await connection.QueryAsync<PendingData>(sql.ToString(), parameters);
                }
                catch(Exception ex)
                {
                    // Log the exception or handle it appropriately
                    throw; // Re-throw to maintain the original exception behavior
                }
            }
        }

        public async Task<int> UpdateDataParameterDetails(string status, string lotNumber, string deliveryDate, string receivedDate, string materialNo, string jobNumber, string toolId)
        //(string deliveryDate, string receivedDate, string lotNumber, string materialNo, string jobNumber, string toolId,
        { 
            //parse date string to correct format yyyy-mm-dd
            DateTime dtDeliveryDate = DateTime.ParseExact(deliveryDate, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime dtReceivedDate = DateTime.ParseExact(receivedDate, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            string strDeliveryDate = dtDeliveryDate.ToString("yyyy-MM-dd");
            string strReceivedDate = dtReceivedDate.ToString("yyyy-MM-dd");

            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"
                            UPDATE [IDM].[dbo].[DATA_PARAMETER_DETAILS]
                            SET [STATUS] = @Status
                            WHERE [STATUS] = 'PENDING'
                                  AND (@DeliveryDate = '' OR [DELIVERY_DATE] = @DeliveryDate)
                                  AND (@ReceivedDate = '' OR [RECEIVED_DATE] = @ReceivedDate)
                                  AND (@LotNumber    = '' OR [LOTNUMBER]     = @LotNumber)
                                  AND (@MaterialNo   = '' OR [MATERIAL_NO]    = @MaterialNo)
                                  AND (@JobNumber    = '' OR [JOB_NUMBER]     = @JobNumber)
                                  AND (@ToolId       = '' OR [TOOLID]         = @ToolId);";

                return await connection.ExecuteAsync(sql, new
                {
                    Status = status,
                    LotNumber = lotNumber,
                    DeliveryDate = strDeliveryDate,
                    ReceivedDate = strReceivedDate,
                    MaterialNo = materialNo,
                    JobNumber = jobNumber,
                    ToolId = toolId
                });
            }
        }

        public async Task<IEnumerable<ParameterTrial>> GetTrialAsync(IncomingData incomingData)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"SELECT * FROM DATA_PARAMETER_TRIALS  
                            WHERE 
                                DELIVERYDATE = @delivery_Date 
                                AND ACTIVEFLAG = 'Y' 
                                AND RECEIVEDDATE = @received_Date 
                                AND MATERIAL_NO = @material_No 
                                AND 
                                (
                                    LOTNUMBER = @lotNumber 
                                    OR (@lotNumber IS NULL AND LOTNUMBER IS NULL) 
                                    OR (@lotNumber = '' AND LOTNUMBER = '')
                                )
                                AND 
                                (
                                    JOB_NUMBER = @job_Number 
                                    OR (@job_Number IS NULL AND JOB_NUMBER IS NULL)
                                    OR (@job_Number = '' AND JOB_NUMBER = '')
                                )
                                AND 
                                (
                                    TOOLID = @toolId 
                                    OR (@toolId IS NULL AND TOOLID IS NULL)
                                    OR (@toolId = '' AND TOOLID = '')
                                ); ";
                return await connection.QueryAsync<ParameterTrial>(sql, incomingData);
            }
        }


    }
}
