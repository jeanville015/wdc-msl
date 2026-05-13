using Dapper;
using IDM.Data;
using IDM.Model.User;
using IDM.Repository.Main.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Main.Repository
{
    public class DataRepository : IDataRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IDb2ConnectionFactory _db2ConnectionFactory;

        public DataRepository(IDbConnectionFactory connectionFactory, IDb2ConnectionFactory db2ConnectionFactory)
        {
            _connectionFactory = connectionFactory;
            _db2ConnectionFactory = db2ConnectionFactory;
        }

        public async Task<IEnumerable<Model.Main.Data>> GetAllAsync()
        {
            using (var connection = _db2ConnectionFactory.CreateConnection())
            {
                var sql = "SELECT DISTINCT MATERIALNUMBER, MATERIALNAME, LOTNUMBER, DELIVERYDATE, RECEIVEDDATE, JOBNUMBER, TOOLID FROM IDM.MATERIALINFO  WITH UR ";
                return await connection.QueryAsync<Model.Main.Data>(sql);
            }
        }

        public async Task<IEnumerable<Model.Main.Data>> GetDataAsync(string deliveryDate, string receivedDate, string materialNo, string lotNumber, string jobNumber, string toolId)
        {
            using (var connection = _db2ConnectionFactory.CreateConnection())
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

                    return await connection.QueryAsync<Model.Main.Data>(sql.ToString(), parameters);
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
