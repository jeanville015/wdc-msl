using Dapper;
using IDM.Data;
using IDM.Model.Maintenance;
using IDM.Repository.Maintenance.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Repository
{
    public class MaterialSettingsRepository : IMaterialSettingsRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MaterialSettingsRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<MaterialSettings>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT MS.*, TT.ToolTypeName, M.MATERIAL_NO AS MaterialNumber, M.MATERIAL_NAME AS MaterialName FROM MAINT_MaterialSettings AS MS LEFT JOIN MAINT_ToolType TT ON TT.ToolTypeId = MS.ToolTypeId LEFT JOIN MAINT_MATERIAL M ON M.Id = MS.MaterialNumberId WHERE MS.ActiveFlag = 'Y' ORDER BY MS.StoreTs DESC";
                return await connection.QueryAsync<MaterialSettings>(sql);
            }
        }

        public async Task<MaterialSettings> GetByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_Tool WHERE MaterialSettingsId = @id";
                return await connection.QueryFirstOrDefaultAsync<MaterialSettings>(sql, new { Id = id });
            }
        }

        public async Task<int> CreateAsync(MaterialSettings materialSettings)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO MAINT_MaterialSettings (SettingsName, SettingsValue, MaterialNumberId, ToolTypeId, ActiveFlag, StoredBy, StoreTs)
                          VALUES (@settingsName, @settingsValue, @materialNumberId, @toolTypeId, 'Y', @storedBy, @storeTs);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(sql, materialSettings);
            }
        }

        public async Task<bool> UpdateAsync(MaterialSettings materialSettings)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE MAINT_MaterialSettings 
                            SET SettingsName = @settingsName, 
                                SettingsValue = @settingsValue, 
                                ToolTypeId = @toolTypeId,
                                MaterialNumberId = @materialNumberId,
                                UpdatedBy = @updatedBy,
                                UpdatedTs = @updatedTs
                            WHERE MaterialSettingsId = @materialSettingsId";
                var affectedRows = await connection.ExecuteAsync(sql, materialSettings);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE MAINT_MaterialSettings set ActiveFlag = 'N' WHERE MaterialSettingsId = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<MaterialSettings> GetBySettingsName(string settingsName, int toolTypeId, int materialId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_MaterialSettings WHERE SettingsName = @settingsName AND ToolTypeId = @toolTypeId  AND MaterialNumberId = @materialId AND ActiveFlag = 'Y'";
                return await connection.QueryFirstOrDefaultAsync<MaterialSettings>(sql, new { settingsName, toolTypeId, materialId });
            }
        }
    }
}
