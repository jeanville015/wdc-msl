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
    public class ParameterRepository : IParameterRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ParameterRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Parameter>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_PARAMETER WHERE ActiveFlag = 'Y' ORDER BY StoreTs DESC ";
                return await connection.QueryAsync<Parameter>(sql);
            }
        } 

        public async Task<Parameter> GetByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_PARAMETER WHERE Id = @id";
                return await connection.QueryFirstOrDefaultAsync<Parameter>(sql, new { Id = id });
            }
        }

        public async Task<int> CreateAsync(Parameter parameter)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO MAINT_PARAMETER (Parameter_Name, ActiveFlag, StoredBy, StoreTs)
                          VALUES (@parameter_Name, 'Y', @StoredBy, @StoreTs);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(sql, parameter);
            }
        }

        public async Task<bool> UpdateAsync(Parameter parameter)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE MAINT_PARAMETER 
                            SET Parameter_Name = @parameter_Name, 
                                UpdatedBy = @updatedBy,
                                UpdatedTs = @updatedTs
                            WHERE Id = @id";
                var affectedRows = await connection.ExecuteAsync(sql, parameter);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE MAINT_PARAMETER set ActiveFlag = 'N' WHERE Id = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<Parameter> GetByParameterName(string parameterName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_PARAMETER WHERE Parameter_Name = @parameterName AND ActiveFlag = 'Y'";
                return await connection.QueryFirstOrDefaultAsync<Parameter>(sql, new { parameterName });
            }
        }
    }
}
