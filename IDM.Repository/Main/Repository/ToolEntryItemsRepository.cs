using Dapper;
using IDM.Data;
using IDM.Model.Main;
using IDM.Model.Maintenance;
using IDM.Model.User;
using IDM.Repository.Main.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Main.Repository
{
    public class ToolEntryItemsRepository : IToolEntryItemsRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ToolEntryItemsRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ToolEntryItems>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT DISTINCT AmethystJob FROM DATA_ENTRYITEMS";
                return await connection.QueryAsync<ToolEntryItems>(sql);
            }
        }

        public async Task<IEnumerable<ToolEntryItems>> GetAllAsyncForApprove()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT MIN(EntryItemsId) as EntryItemsId, AmethystJob, MIN(ToolName) as ToolName, MIN(DateAnalyzed) as DateAnalyzed, MIN(AnalyzedBy) as AnalyzedBy, MIN(DateReviewed) as DateReviewed, MIN(ReviewedBy) as ReviewedBy, MIN(Customer) as Customer FROM DATA_ENTRYITEMS WHERE Status = 'PENDING' GROUP BY AmethystJob";
                return await connection.QueryAsync<ToolEntryItems>(sql);
            }
        }

        public async Task<IEnumerable<ToolEntryItems>> GetAllAsyncByAmethystJob(string amethystJob)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM DATA_ENTRYITEMS WHERE AmethystJob = @amethystJob ";
                return await connection.QueryAsync<ToolEntryItems>(sql, new { amethystJob });
            }
        }

        public async Task<IEnumerable<ToolEntryItems>> GetAllAsyncByAmethystJobAndStatus(string amethystJob)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    var sql = "SELECT * FROM DATA_ENTRYITEMS WHERE Status = 'PENDING' AND AmethystJob = @amethystJob ";
                    return await connection.QueryAsync<ToolEntryItems>(sql, new { amethystJob });
                }
            }
            catch (Exception ex)
            { throw; }
        }

        public async Task<IEnumerable<ToolEntryItems>> GetDataAsync(string deliveryDate, string receivedDate, string materialNo, string lotNumber, string jobNumber, string toolId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    var sql = new StringBuilder("SELECT * FROM IDM.MATERIALINFO WHERE 1=1");
                    var parameters = new DynamicParameters();

                    if (!string.IsNullOrEmpty(deliveryDate))
                    {
                        sql.Append(" AND DELIVERYDATE = ?");
                        parameters.Add("@DeliveryDate", deliveryDate);
                    }

                    if (!string.IsNullOrEmpty(receivedDate))
                    {
                        sql.Append(" AND RECEIVEDDATE = ?");
                        parameters.Add("@ReceivedDate", receivedDate);
                    }

                    if (!string.IsNullOrEmpty(materialNo))
                    {
                        sql.Append(" AND MATERIALNUMBER = ?");
                        parameters.Add("@MaterialNo", materialNo);
                    }

                    if (!string.IsNullOrEmpty(lotNumber))
                    {
                        sql.Append(" AND LOTNUMBER = ?");
                        parameters.Add("@LotNumber", lotNumber);
                    }

                    if (!string.IsNullOrEmpty(jobNumber))
                    {
                        sql.Append(" AND JOBNUMBER = ?");
                        parameters.Add("@JobNumber", jobNumber);
                    }

                    if (!string.IsNullOrEmpty(toolId))
                    {
                        sql.Append(" AND TOOLID = ?");
                        parameters.Add("@ToolId", toolId);
                    }

                    sql.Append(" WITH UR");

                    return await connection.QueryAsync<ToolEntryItems>(sql.ToString(), parameters);
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
