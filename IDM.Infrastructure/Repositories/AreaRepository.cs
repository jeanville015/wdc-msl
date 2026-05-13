using IDM.Domain.Entities.Maintenance;
using IDM.Infrastructure.Database;
using IDM.Infrastructure.Repositories.Interfaces;
using Dapper;

namespace IDM.Infrastructure.Repositories
{
    public sealed class AreaRepository : BaseRepository, IAreaRepository
    {
        public AreaRepository(ConnectionFactory factory, IUserContext userContext)
        : base(factory, userContext) { }

        public async Task<IEnumerable<Area>> GetAllAsync()
        {
            var sql = "SELECT * FROM MAINT.Area WHERE ActiveFlag = 0 ORDER BY StoreTs DESC ";
            using var connection = CreateConnection();
            return await connection.QueryAsync<Area>(sql);
        }

        public async Task<Area> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM MAINT.Area WHERE AreaId = @id";
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Area>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(Area area)
        {
            var sql = @"INSERT INTO MAINT.Area (AreaName, StoredBy, StoreTs)
                          VALUES (@areaName, @StoredBy, @StoreTs);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
            //return await connection.ExecuteScalarAsync<int>(sql, area);
            return await InsertAsync(sql, area);
        }

        public async Task<bool> UpdateAsync(Area area)
        {
            var sql = @"UPDATE MAINT.Area 
                            SET AreaName = @areaName, 
                                UpdateBy = @UpdateBy,
                                UpdateTs = @UpdateTs
                            WHERE AreaId = @areaId";
            return await UpdateAsync(sql, area);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "UPDATE MAINT.Area set ActiveFlag = 1 WHERE AreaId = @Id";
            return await UpdateAsync(sql, id);
        }

        public async Task<Area> GetByAreaName(string areaName)
        {
                var sql = "SELECT * FROM MAINT.Area WHERE AreaName = @areaName AND ActiveFlag = 0";
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Area>(sql, new { areaName });
        }
    }
}
