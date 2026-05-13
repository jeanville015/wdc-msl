using Dapper;
using IDM.Data;
using IDM.Model.Maintenance;
using IDM.Repository.Maintenance.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Repository
{
    public class UomRepository : IUomRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UomRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Uom>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT Id, Uom_Name, ActiveFlag, UpdatedBy, UpdatedTs, StoredBy, StoreTs FROM MAINT_UOM WHERE ActiveFlag = 'Y' ORDER BY STORETS DESC ";
                return await connection.QueryAsync<Uom>(sql);
            }
        }

        public async Task<Uom> GetByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT Id, Uom_Name, ActiveFlag, UpdatedBy, UpdatedTs, StoredBy, StoreTs FROM MAINT_UOM WHERE Id = @id";
                return await connection.QueryFirstOrDefaultAsync<Uom>(sql, new { Id = id });
            }
        }

        public async Task<int> CreateAsync(Uom uom)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO MAINT_UOM (Uom_Name, ActiveFlag, StoredBy, StoreTs)
                          VALUES (@uom_Name, 'Y', @storedBy, @storeTs);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(sql, uom);
            }
        }

        public async Task<bool> UpdateAsync(Uom uom)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE MAINT_UOM 
                            SET Uom_Name = @uom_Name, 
                                UpdatedBy = @updatedBy,
                                UpdatedTs = @updatedTs
                            WHERE Id = @id";
                var affectedRows = await connection.ExecuteAsync(sql, uom);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE MAINT_UOM set ActiveFlag = 'N' WHERE Id = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<Uom> GetByUomName(string uomName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_UOM WHERE uom_Name = @uomName AND ActiveFlag = 'Y'";
                return await connection.QueryFirstOrDefaultAsync<Uom>(sql, new { uomName });
            }
        }
    }
}
