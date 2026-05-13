using Dapper;
using IDM.Data;
using IDM.Model.User;
using IDM.Repository.User.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Repository.User.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RoleRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT Id, User_Role, ActiveFlag, UpdatedBy, UpdatedTs, StoredBy, StoreTs FROM [dbo].[USER_ROLE] WHERE ACTIVEFLAG = 'Y' ORDER BY STORETS DESC ";
                return await connection.QueryAsync<Role>(sql);
            }
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT Id, User_Role, ActiveFlag, UpdatedBy, UpdatedTs, StoredBy, StoreTs FROM [dbo].[USER_ROLE] WHERE ID = @id";
                return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { Id = id });
            }
        }

        public async Task<int> CreateAsync(Role role)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO [dbo].[USER_ROLE] (USER_ROLE, ActiveFlag, STOREDBY, STORETS)
                          VALUES (@user_Role, 'Y', @storedBy, @storeTs);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(sql, role);
            }
        }

        public async Task<bool> UpdateAsync(Role role)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE [dbo].[USER_ROLE] 
                            SET USER_ROLE = @user_Role, 
                                UPDATEDBY = @updatedBy,
                                UPDATEDTS = @updatedTs
                            WHERE ID = @ID";
                var affectedRows = await connection.ExecuteAsync(sql, role);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE [dbo].[USER_ROLE] set ACTIVEFLAG = 'N' WHERE ID = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<Role> GetByRoleName(string roleName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT Id, User_Role, ActiveFlag, UpdatedBy, UpdatedTs, StoredBy, StoreTs FROM [dbo].[USER_ROLE] WHERE USER_ROLE = @roleName AND ACTIVEFLAG = 'Y'";
                return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { roleName });
            }
        }
    }
}
