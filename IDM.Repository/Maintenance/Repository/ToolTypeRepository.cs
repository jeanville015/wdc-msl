using Dapper;
using IDM.Data;
using IDM.Model.Maintenance;
using IDM.Repository.Maintenance.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Repository
{
    public class ToolTypeRepository : IToolTypeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ToolTypeRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ToolType>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_ToolType WHERE ActiveFlag = 'Y' ORDER BY StoreTs DESC ";
                return await connection.QueryAsync<ToolType>(sql);
            }
        }

        public async Task<ToolType> GetByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_ToolType WHERE ToolTypeId = @id";
                return await connection.QueryFirstOrDefaultAsync<ToolType>(sql, new { Id = id });
            }
        }

        public async Task<int> CreateAsync(ToolType toolType)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO MAINT_ToolType (ToolTypeName, RequireApproval, ActiveFlag, StoredBy, StoreTs)
                          VALUES (@toolTypeName, @requireApproval, 'Y', @storedBy, @storeTs);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(sql, toolType);
            }
        }

        public async Task<bool> UpdateAsync(ToolType toolType)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE MAINT_ToolType 
                            SET ToolTypeName = @toolTypeName, 
                                RequireApproval = @requireApproval, 
                                UpdatedBy = @updatedBy,
                                UpdatedTs = @updatedTs
                            WHERE ToolTypeId = @toolTypeId";
                var affectedRows = await connection.ExecuteAsync(sql, toolType);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE MAINT_ToolType set ActiveFlag = 'N' WHERE ToolTypeId = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<ToolType> GetByToolTypeName(string toolTypeName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_ToolType WHERE ToolTypeName = @toolTypeName AND ActiveFlag = 'Y'";
                return await connection.QueryFirstOrDefaultAsync<ToolType>(sql, new { toolTypeName });
            }
        }
    }
}
