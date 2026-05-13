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
    public class MaterialParameterRepository : IMaterialParameterRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MaterialParameterRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<MaterialParameter>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"SELECT MATPARM.MaterialParameterId, MATPARM.MaterialId, MATPARM.ParameterId, PARM.ParameterName,
		                            MATPARM.UomId, UOM.UomName, MATPARM.SiteId, Site.SiteName, MATPARM.IsEdcspc, 
		                            MATPARM.LowerSpecsLimit, MATPARM.UpperSpecsLimit, MATPARM.LowerControlLimit, MATPARM.UpperControlLimit
                            FROM MAINT.MaterialParameter AS MATPARM
                            LEFT JOIN MAINT.Material MAT ON MAT.MaterialId = MATPARM.MaterialId
                            LEFT JOIN MAINT.Parameter PARM ON PARM.ParameterId = MATPARM.ParameterId
                            LEFT JOIN MAINT.Uom UOM ON UOM.UomId = MATPARM.UomId
                            LEFT JOIN MAINT.Site Site ON Site.SiteId = MATPARM.SiteId
                        WHERE MATPARM.ActiveFlag = 0
                        ORDER BY MATPARM.STORETS DESC";
                return await connection.QueryAsync<MaterialParameter>(sql);
            }
        }

        public async Task<IEnumerable<MaterialParameter>> GetByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"SELECT MATPARM.MaterialParameterId, MATPARM.MaterialId, MATPARM.ParameterId, PARM.ParameterName,
		                            MATPARM.UomId, UOM.UomName, MATPARM.SiteId, Site.SiteName, MATPARM.IsEdcspc, 
		                            MATPARM.LowerSpecsLimit, MATPARM.UpperSpecsLimit, MATPARM.LowerControlLimit, MATPARM.UpperControlLimit
                            FROM MAINT.MaterialParameter AS MATPARM
                            LEFT JOIN MAINT.Material MAT ON MAT.MaterialId = MATPARM.MaterialId
                            LEFT JOIN MAINT.Parameter PARM ON PARM.ParameterId = MATPARM.ParameterId
                            LEFT JOIN MAINT.Uom UOM ON UOM.UomId = MATPARM.UomId
                            LEFT JOIN MAINT.Site Site ON Site.SiteId = MATPARM.SiteId 
                            WHERE MATPARM.ActiveFlag = 0 AND MATPARM.MaterialId = @Id
                            ORDER BY MATPARM.STORETS DESC";

                return await connection.QueryAsync<MaterialParameter>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<MaterialParameter>> GetByMaterialNoAsync(string id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"SELECT 
                            MATPARM.Id, MATPARM.Material_No,
                            MATPARM.Parameter_Name, PARM.ID AS ParameterId, 
                            MATPARM.Uom_Name, UOM.ID AS UomId, 
                            MATPARM.Site_Name, Site.ID AS SiteId, 
                            MATPARM.EdcSpcFlag, MATPARM.LowerSpecsLimit, MATPARM.UpperSpecsLimit, MATPARM.LowerControlLimit, MATPARM.UpperControlLimit,  MATPARM.ActiveFlag
                            FROM MAINT_PARAMETER_DETAILS AS MATPARM 
                            LEFT JOIN MAINT_MATERIAL MAT ON MAT.MATERIAL_NO = MATPARM.MATERIAL_NO 
                            LEFT JOIN MAINT_PARAMETER PARM ON PARM.PARAMETER_NAME = MATPARM.PARAMETER_NAME 
                            LEFT JOIN MAINT_UOM UOM ON UOM.UOM_NAME = MATPARM.UOM_NAME 
                            LEFT JOIN MAINT_SITE Site ON Site.SITE_NAME = MATPARM.SITE_NAME 
                            WHERE MATPARM.ActiveFlag = 'Y' AND UOM.ActiveFlag = 'Y' AND Site.ActiveFlag = 'Y' AND MAT.ActiveFlag = 'Y' AND MATPARM.MATERIAL_NO = @Id
                            ORDER BY MATPARM.STORETS DESC";

                return await connection.QueryAsync<MaterialParameter>(sql, new { Id = id });
            }
        }

        public async Task<int> CreateAsync(MaterialParameter materialParameter)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO MAINT_PARAMETER_DETAILS 
                            (MATERIAL_NO, PARAMETER_NAME, UOM_NAME, LOWERSPECSLIMIT, UPPERSPECSLIMIT, LOWERCONTROLLIMIT, UPPERCONTROLLIMIT, SITE_NAME, ACTIVEFLAG, STOREDBY, STORETS, EDCSPCFLAG) 
                            VALUES (@Material_No, @Parameter_Name, @Uom_Name, @LowerSpecsLimit, @UpperSpecsLimit, @LowerControlLimit, @UpperControlLimit, @Site_Name, 'Y', @StoredBy, @StoreTs, @EdcSpcFlag);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(sql, materialParameter);
            }
        }

        public async Task<bool> UpdateAsync(MaterialParameter materialParameter)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE MAINT_PARAMETER_DETAILS
                            SET PARAMETER_NAME = @Parameter_Name, 
                                UOM_NAME = @Uom_Name,
                                Site_Name = @Site_Name,
                                EdcSpcFlag = @EdcSpcFlag,
                                LowerSpecsLimit = @LowerSpecsLimit,
                                UpperSpecsLimit = @UpperSpecsLimit,
                                LowerControlLimit = @LowerControlLimit,
                                UpperControlLimit = @UpperControlLimit,
                                UpdatedBy = @UpdatedBy,
                                UpdatedTs = @UpdatedTs
                            WHERE id = @id";
                var affectedRows = await connection.ExecuteAsync(sql, materialParameter);
                return affectedRows > 0;

            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE MAINT_PARAMETER_DETAILS set ActiveFlag = 'N' WHERE id = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<MaterialParameter> GetByParameterAndSite(MaterialParameter materialParameter)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_PARAMETER_DETAILS WHERE Parameter_Name = @Parameter_Name AND Material_No = @Material_No AND Site_Name = @Site_Name AND ActiveFlag = 'Y'";
                return await connection.QueryFirstOrDefaultAsync<MaterialParameter>(sql, materialParameter);
            }
        }
    }
}
