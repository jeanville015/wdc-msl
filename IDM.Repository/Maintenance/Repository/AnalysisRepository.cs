using Dapper;
using IDM.Data;
using IDM.Model.Maintenance;
using IDM.Repository.Maintenance.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Repository
{
    public class AnalysisRepository : IAnalysisRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AnalysisRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Analysis>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT A.*, TT.ToolTypeName  FROM MAINT_Analysis AS A LEFT JOIN MAINT_ToolType TT ON TT.ToolTypeId = A.ToolTypeId WHERE A.ActiveFlag = 'Y' ORDER BY A.StoreTs DESC ";
                return await connection.QueryAsync<Analysis>(sql);
            }
        }

        public async Task<Analysis> GetByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_Analysis WHERE AnalysisId = @id";
                return await connection.QueryFirstOrDefaultAsync<Analysis>(sql, new { Id = id });
            }
        }

        public async Task<Analysis> GetByToolTypeAndAnalysisAsync(int toolTypeId, string analysisName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_Analysis WHERE ToolTypeId = @toolTypeId AND AnalysisName = @analysisName";
                return await connection.QueryFirstOrDefaultAsync<Analysis>(sql, new { toolTypeId, analysisName });
            }
        }

        public async Task<Analysis> GetByAnalysisAsync(string analysisName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_Analysis WHERE AnalysisName = @analysisName";
                return await connection.QueryFirstOrDefaultAsync<Analysis>(sql, new { analysisName });
            }
        }

        public async Task<int> CreateAsync(Analysis analysis)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO MAINT_Analysis (AnalysisName, ToolTypeId, SourceTable, DestinationTable, ActiveFlag, StoredBy, StoreTs)
                          VALUES (@analysisName, @toolTypeId, @sourceTable, @destinationTable, 'Y', @storedBy, @storeTs);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(sql, analysis);
            }
        }

        public async Task<bool> UpdateAsync(Analysis analysis)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE MAINT_Analysis 
                            SET AnalysisName = @analysisName, 
                                ToolTypeId = @toolTypeId,
                                sourceTable = @sourceTable,
                                destinationTable = @destinationTable,
                                UpdatedBy = @updatedBy,
                                UpdatedTs = @updatedTs
                            WHERE AnalysisId = @analysisId";
                var affectedRows = await connection.ExecuteAsync(sql, analysis);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE MAINT_Analysis set ActiveFlag = 'N' WHERE AnalysisId = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<Analysis> GetByAnalysisName(string analysisName, int toolTypeId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_Analysis WHERE AnalysisName = @analysisName AND ToolTypeId = @toolTypeId AND ActiveFlag = 'Y'";
                return await connection.QueryFirstOrDefaultAsync<Analysis>(sql, new { analysisName, toolTypeId });
            }
        }
    }
}
