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
    public class ManufacturerRepository : IManufacturerRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ManufacturerRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Manufacturer>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_MANUFACTURER WHERE ACTIVEFLAG = 'Y' ORDER BY StoreTs DESC ";
                return await connection.QueryAsync<Manufacturer>(sql);
            }
        }

        public async Task<Manufacturer> GetByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_MANUFACTURER WHERE Id = @id";
                return await connection.QueryFirstOrDefaultAsync<Manufacturer>(sql, new { Id = id });
            }
        }

        public async Task<int> CreateAsync(Manufacturer manufacturer)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO MAINT_MANUFACTURER (Manufacturer_Name, ActiveFlag, StoredBy, StoreTs)
                          VALUES (@manufacturer_Name, 'Y', @storedBy, @storeTs);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(sql, manufacturer);
            }
        }

        public async Task<bool> UpdateAsync(Manufacturer manufacturer)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE MAINT_MANUFACTURER 
                            SET Manufacturer_Name = @manufacturer_Name, 
                                UpdatedBy = @updatedBy,
                                UpdatedTs = @updatedTs
                            WHERE Id = @id";
                var affectedRows = await connection.ExecuteAsync(sql, manufacturer);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE MAINT_MANUFACTURER set ACTIVEFLAG = 'N' WHERE Id = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<Manufacturer> GetByManufacturerName(string manufacturerName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_MANUFACTURER WHERE Manufacturer_Name = @manufacturerName AND ACTIVEFLAG = 'Y'";
                return await connection.QueryFirstOrDefaultAsync<Manufacturer>(sql, new { manufacturerName });
            }
        }
    }
}
