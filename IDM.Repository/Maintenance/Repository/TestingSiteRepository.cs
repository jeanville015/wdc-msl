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
    public class TestingSiteRepository : ITestingSiteRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TestingSiteRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<TestingSite>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT Id, Site_Name, ActiveFlag, UpdatedBy, UpdatedTs, StoredBy, StoreTs FROM MAINT_SITE WHERE ActiveFlag = 'Y' ORDER BY StoreTs DESC ";
                return await connection.QueryAsync<TestingSite>(sql);
            }
        }

        public async Task<TestingSite> GetByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT Id, Site_Name, ActiveFlag, UpdatedBy, UpdatedTs, StoredBy, StoreTs FROM MAINT_SITE WHERE Id = @id";
                return await connection.QueryFirstOrDefaultAsync<TestingSite>(sql, new { Id = id });
            }
        }

        public async Task<int> CreateAsync(TestingSite testingSite)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO MAINT_SITE (Site_Name, ActiveFlag, StoredBy, StoreTs)
                          VALUES (@site_Name, 'Y', @storedBy, @storeTs);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(sql, testingSite);
            }
        }

        public async Task<bool> UpdateAsync(TestingSite testingSite)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE MAINT_SITE 
                            SET Site_Name = @site_Name, 
                                UpdatedBy = @updatedBy,
                                UpdatedTs = @updatedTs
                            WHERE Id = @id";
                var affectedRows = await connection.ExecuteAsync(sql, testingSite);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE MAINT_SITE set ActiveFlag = 'N' WHERE Id = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<TestingSite> GetBySiteName(string siteName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT Id, Site_Name, ActiveFlag, UpdatedBy, UpdatedTs, StoredBy, StoreTs  FROM MAINT_SITE WHERE Site_Name = @siteName AND ActiveFlag = 'Y'";
                return await connection.QueryFirstOrDefaultAsync<TestingSite>(sql, new { siteName });
            }
        }
    }
}
