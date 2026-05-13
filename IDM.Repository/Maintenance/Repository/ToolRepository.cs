using Dapper;
using IDM.Data;
using IDM.Model.Maintenance;
using IDM.Repository.Maintenance.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Repository
{
    public class ToolRepository : IToolRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ToolRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Tool>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT T.*, TT.ToolTypeName  FROM MAINT_Tool AS T LEFT JOIN MAINT_ToolType TT ON TT.ToolTypeId = T.ToolTypeId WHERE T.ActiveFlag = 'Y' ORDER BY T.StoreTs DESC ";
                return await connection.QueryAsync<Tool>(sql);
            }
        }

        public async Task<Tool> GetByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_Tool WHERE ToolId = @id";
                return await connection.QueryFirstOrDefaultAsync<Tool>(sql, new { Id = id });
            }
        }

        public async Task<Tool> GetByToolNameAsync(string toolName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_Tool WHERE ToolName = @toolName";
                return await connection.QueryFirstOrDefaultAsync<Tool>(sql, new { toolName });
            }
        }

        public async Task<int> CreateAsync(Tool tool)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO MAINT_Tool (ToolName, ToolTypeId, ActiveFlag, StoredBy, StoreTs)
                          VALUES (@toolName, @toolTypeId, 'Y', @storedBy, @storeTs);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(sql, tool);
            }
        }

        public async Task<bool> UpdateAsync(Tool tool)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE MAINT_Tool 
                            SET ToolName = @toolName, 
                                ToolTypeId = @toolTypeId,
                                UpdatedBy = @updatedBy,
                                UpdatedTs = @updatedTs
                            WHERE ToolId = @toolId";
                var affectedRows = await connection.ExecuteAsync(sql, tool);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE MAINT_Tool set ActiveFlag = 'N' WHERE ToolId = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<Tool> GetByToolName(string toolName, int toolTypeId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_Tool WHERE ToolName = @toolName AND ToolTypeId = @toolTypeId AND ActiveFlag = 'Y'";
                return await connection.QueryFirstOrDefaultAsync<Tool>(sql, new { toolName, toolTypeId });
            }
        }
    }
}
