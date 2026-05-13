using Dapper;
using IDM.Data;
using IDM.Model.Main;
using IDM.Model.Maintenance;
using IDM.Repository.Maintenance.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Repository
{
    public class DefectRepository : IDefectRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DefectRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Defect>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT [DefectId], [AnalysisApplicable], [DefectType], [Defect] AS DefectName ,[2KxSem] AS _2KxSem, [Talc], [OverAllPhysicalDefect], ActiveFlag, UpdatedBy, UpdatedTs, StoredBy, StoreTs FROM [MAINT_Defect] WHERE ActiveFlag = 'Y' ORDER BY STORETS DESC ";
                return await connection.QueryAsync<Defect>(sql);
            }
        }

        public async Task<Defect> GetByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT [DefectId], [AnalysisApplicable], [DefectType], [Defect] AS DefectName ,[2KxSem] AS _2KxSem, [Talc], [OverAllPhysicalDefect], ActiveFlag, UpdatedBy, UpdatedTs, StoredBy, StoreTs FROM MAINT_Defect WHERE DefectId = @id";
                return await connection.QueryFirstOrDefaultAsync<Defect>(sql, new { Id = id });
            }
        }

        public async Task<int> CreateAsync(Defect defect)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO MAINT_Defect (AnalysisApplicable, DefectType, Defect, [2KxSem], Talc, OverAllPhysicalDefect, ActiveFlag, StoredBy, StoreTs)
                          VALUES (@analysisApplicable, @defectType, @defectName, @_2kxSem, @talc, @overallPhysicalDefect, 'Y', @storedBy, @storeTs);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(sql, defect);
            }
        }

        public async Task<bool> UpdateAsync(Defect defect)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE MAINT_Defect 
                            SET AnalysisApplicable = @analysisApplicable,
                                 DefectType = @defectType,
                                 Defect = @defectName,
                                 [2KxSem] = @_2kxSem,
                                 Talc = @talc,
                                 OverAllPhysicalDefect = @overallPhysicalDefect,
                                UpdatedBy = @updatedBy,
                                UpdatedTs = @updatedTs
                            WHERE DefectId = @defectId";
                var affectedRows = await connection.ExecuteAsync(sql, defect);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE MAINT_Defect set ActiveFlag = 'N' WHERE DefectId = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<Defect> GetByDefectName(string defectName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_Defect WHERE Defect = @defectName AND ActiveFlag = 'Y'";
                return await connection.QueryFirstOrDefaultAsync<Defect>(sql, new { defectName });
            }
        }

        public async Task<DefectBulk> GetByDefectBulkNames(DataTable csvData)
        {
            // Extract only the DefectNames from your DataTable into a list
            var Names = csvData.AsEnumerable()
                                      .Select(r => r.Field<string>("DefectName"))
                                      .ToList();

            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT Defect FROM MAINT_Defect WHERE Defect IN @Names";
                
                var existingNames = await connection.QueryAsync<string>(sql, new { Names });
                return new DefectBulk
                {
                    DuplicatedDefectNames = existingNames.ToList()
                };
            }
        }

        public async Task<bool> CreateBulkAsync(DefectBulk defectBulk)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    var sqlConnection = connection as SqlConnection;

                    if (sqlConnection == null)
                        throw new InvalidOperationException("SqlBulkCopy requires a SQL Server connection.");

                    if (sqlConnection.State != ConnectionState.Open)
                        sqlConnection.Open();

                    using (var bulkCopy = new SqlBulkCopy(sqlConnection))
                    {
                        bulkCopy.DestinationTableName = "[IDM ].[dbo].[MAINT_Defect]";
                        bulkCopy.WriteToServer(defectBulk.DefectDataTable);
                    }
                }
                return true;
            }
            catch (Exception ex) 
            { 
                return false;
            }
        }
    }
}
