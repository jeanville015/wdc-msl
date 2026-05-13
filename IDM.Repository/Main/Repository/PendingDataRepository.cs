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

        public async Task<(IEnumerable<PendingData> Items, int TotalCount)> GetPendingDataDetailsAsync(string deliveryDate, string receivedDate, string lotNumber, string materialNo, string jobNumber, string toolId, int page, int pageSize)
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
    }
}
