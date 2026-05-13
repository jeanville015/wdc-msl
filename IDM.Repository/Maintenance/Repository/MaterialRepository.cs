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
    public class MaterialRepository : IMaterialRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MaterialRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Material>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"SELECT 
	                          MAT.ID
	                        , MAT.Material_Name
	                        , MAT.Material_No
	                        , MAT.Area_Name
	                        , AREA.ID AS AreaId
	                        , MAT.Manufacturer_Name
	                        , MAN.ID AS ManufacturerId
	                        , MAT.Supplier_Name
	                        , SUP.ID AS SupplierId
                            , MAT.ActiveFlag
                        FROM MAINT_MATERIAL MAT
                        INNER JOIN MAINT_AREA AREA ON AREA.Area_Name = MAT.Area_Name
                        INNER JOIN MAINT_MANUFACTURER MAN ON MAN.Manufacturer_Name = MAT.Manufacturer_Name
                        INNER JOIN MAINT_SUPPLIER SUP ON SUP.Supplier_Name = MAT.Supplier_Name
                        WHERE MAT.ActiveFlag = 'Y' AND SUP.ActiveFlag = 'Y' AND MAN.ActiveFlag = 'Y'
                        ORDER BY MAT.STORETS DESC";
                return await connection.QueryAsync<Material>(sql);
            }
        }

        public async Task<Material> GetByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_MATERIAL WHERE Id = @id";
                return await connection.QueryFirstOrDefaultAsync<Material>(sql, new { Id = id });
            }
        }

        public async Task<int> CreateAsync(Material material)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO MAINT_MATERIAL (Material_Name, Material_No, Area_Name, Manufacturer_Name, Supplier_Name, ActiveFlag, StoredBy, StoreTs)
                          VALUES (@material_Name, @material_No, @area_Name, @manufacturer_Name, @Supplier_Name, 'Y', @StoredBy, @StoreTs);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(sql, material);
            }
        }

        public async Task<bool> UpdateAsync(Material material)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE MAINT_MATERIAL 
                            SET Material_Name = @material_Name, 
                                Material_No = @material_No,
                                Area_Name = @area_Name,
                                Manufacturer_Name = @manufacturer_Name,
                                Supplier_Name = @supplier_Name,
                                UpdatedBy = @updatedBy,
                                UpdatedTs = @updatedTs
                            WHERE Id = @id";
                var affectedRows = await connection.ExecuteAsync(sql, material);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE MAINT_MATERIAL set ActiveFlag = 'N' WHERE Id = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<Material> GetByMaterialName(string materialNo)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM MAINT_MATERIAL WHERE Material_No = @materialNo AND ActiveFlag = 'Y'";
                return await connection.QueryFirstOrDefaultAsync<Material>(sql, new { materialNo });
            }
        }
    }
}
