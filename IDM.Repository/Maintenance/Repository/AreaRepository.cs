using Dapper;
using IDM.Data;
using IDM.Model.Maintenance;
using IDM.Repository.Maintenance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Repository
{
    public class AreaRepository : IAreaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AreaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Area>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_AREA WHERE ActiveFlag = 'Y' ORDER BY StoreTs DESC ";
                return await connection.QueryAsync<Area>(sql);
            }
        }

        public async Task<Area> GetByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_AREA WHERE Id = @id";
                return await connection.QueryFirstOrDefaultAsync<Area>(sql, new { Id = id });
            }
        }

        public async Task<int> CreateAsync(Area area)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO MAINT_AREA (Area_Name, ActiveFlag, StoredBy, StoreTs)
                          VALUES (@area_Name, 'Y', @storedBy, @storeTs);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(sql, area);
            }
        }

        public async Task<bool> UpdateAsync(Area area)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE MAINT_AREA 
                            SET Area_Name = @area_Name, 
                                UpdatedBy = @updatedBy,
                                UpdatedTs = @updatedTs
                            WHERE Id = @id";
                var affectedRows = await connection.ExecuteAsync(sql, area);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE MAINT_AREA set ActiveFlag = 'N' WHERE Id = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<Area> GetByAreaName(string areaName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_AREA WHERE Area_Name = @areaName AND ActiveFlag = 'Y'";
                return await connection.QueryFirstOrDefaultAsync<Area>(sql, new { areaName });
            }
        }
    }
}
