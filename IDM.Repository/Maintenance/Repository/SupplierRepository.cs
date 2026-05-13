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
    public class SupplierRepository : ISupplierRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SupplierRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT Id, Supplier_Name, ActiveFlag, UpdatedBy, UpdatedTs, StoredBy, StoreTs FROM MAINT_SUPPLIER WHERE ActiveFlag = 'Y' ORDER BY StoreTs DESC ";
                return await connection.QueryAsync<Supplier>(sql);
            }
        }

        public async Task<Supplier> GetByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT Id, Supplier_Name, ActiveFlag, UpdatedBy, UpdatedTs, StoredBy, StoreTs FROM MAINT_SUPPLIER WHERE Id = @id";
                return await connection.QueryFirstOrDefaultAsync<Supplier>(sql, new { Id = id });
            }
        }

        public async Task<int> CreateAsync(Supplier supplier)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO MAINT_SUPPLIER (Supplier_Name, ActiveFlag, StoredBy, StoreTs)
                          VALUES (@supplier_Name, 'Y', @storedBy, @storeTs);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(sql, supplier);
            }
        }

        public async Task<bool> UpdateAsync(Supplier supplier)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE MAINT_SUPPLIER 
                            SET Supplier_Name = @supplier_Name, 
                                UpdatedBy = @updatedBy,
                                UpdatedTs = @updatedTs
                            WHERE Id = @id";
                var affectedRows = await connection.ExecuteAsync(sql, supplier);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE MAINT_SUPPLIER set ActiveFlag = 'N' WHERE Id = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<Supplier> GetBySupplierName(string supplierName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_SUPPLIER WHERE Supplier_Name = @supplierName AND ActiveFlag = 'Y'";
                return await connection.QueryFirstOrDefaultAsync<Supplier>(sql, new { supplierName });
            }
        }
    }
}
