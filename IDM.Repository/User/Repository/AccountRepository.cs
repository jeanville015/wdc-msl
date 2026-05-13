using Dapper;
using IDM.Data;
using IDM.Model.User;
using IDM.Repository.User.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.User.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AccountRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"
                            SELECT 
                                [ul].Id, 
                                [ul].User_Id, 
                                [ul].User_Role, 
                                [ul].LastLoginTs, 
                                [ur].ID AS RoleId, 
                                [ul].User_Group,
                                STRING_AGG(ma.AnalysisName, ', ') 
                                    WITHIN GROUP (ORDER BY ma.AnalysisName) AS User_Analysis_string_group
                            FROM [dbo].[USER_LIST] ul
                            LEFT JOIN [dbo].[USER_ROLE] ur
                                ON [ur].[USER_ROLE] =  [ul].[USER_ROLE]
                            LEFT JOIN [dbo].[USER_ANALYSIS] [ua] 
                                ON [ua].[USER_ID] = [ul].[ID]
                            LEFT JOIN [dbo].[MAINT_Analysis] [ma] 
                                ON [ma].AnalysisId = [ua].[MAINT_ANALYSIS_ID]
                            WHERE [ul].[ACTIVEFLAG] = 'Y' 
                            GROUP BY 
                                [ul].[Id],
                                [ul].[User_Id],
                                [ul].[User_Role],
                                [ul].LastLoginTs,
                                [ur].ID,
                                [ul].User_Group,
                                [ul].[STORETS]
                            ORDER BY [ul].[STORETS] DESC;";
                return await connection.QueryAsync<Account>(sql);
            }
        }

        public async Task<IEnumerable<int>> GetAnalysisListByUserIdAsync(int userId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"SELECT [MA].[AnalysisId]
                              FROM [DBO].[MAINT_Analysis] MA
                              INNER JOIN [DBO].[USER_ANALYSIS] [UA] ON [UA].[MAINT_ANALYSIS_ID] = [MA].[AnalysisId]
                              WHERE [UA].[USER_ID] = @UserId";
                return await connection.QueryAsync<int>(sql, new { UserId = userId });
            }
        }

        public async Task<Account> GetByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM [dbo].[USER_LIST] WHERE ID = @id";
                return await connection.QueryFirstOrDefaultAsync<Account>(sql, new { Id = id });
            }
        }

        public async Task<int> CreateAsync(Account account)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Step 1: insert the main user record
                        var sql = @"INSERT INTO [dbo].[USER_LIST] 
                                (USER_ID, USER_ROLE, ACTIVEFLAG, LastLoginTs, STOREDBY, STORETS)
                            VALUES 
                                (@User_Id, @user_Role, 'Y', @storeTs, @storedBy, @storeTs);
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

                        var newId = await connection.ExecuteScalarAsync<int>(sql, account, transaction);

                        // Step 2: insert USER_ANALYSIS child rows if any were selected
                        if (account.User_Analysis != null && account.User_Analysis.Any())
                        {
                            var analysisSql = @"INSERT INTO [dbo].[USER_ANALYSIS] 
                                            (USER_ID, MAINT_ANALYSIS_ID)
                                        VALUES 
                                            (@UserId, @AnalysisId)";

                            var rows = account.User_Analysis.Select(analysisId => new
                            {
                                UserId = newId,       // FK from SCOPE_IDENTITY()
                                AnalysisId = analysisId
                            });

                            await connection.ExecuteAsync(analysisSql, rows, transaction);
                        }

                        transaction.Commit();
                        return newId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public async Task<int> CreateAsync_old(Account account)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO [dbo].[USER_LIST] (USER_ID, USER_ROLE, ACTIVEFLAG, LastLoginTs, STOREDBY, STORETS)
                          VALUES (@user_Id, @user_Role, 'Y', @storeTs, @storedBy, @storeTs);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(sql, account);
            }
        }

        public async Task<bool> UpdateAsync(Account account)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Step 1: update the main user record
                        var sql = @"UPDATE [dbo].[USER_LIST] 
                            SET USER_ID    = @User_Id, 
                                USER_ROLE  = @User_Role,
                                USER_GROUP = @User_Group,
                                UPDATEDBY  = @UpdatedBy,
                                UPDATEDTS  = @UpdatedTs
                            WHERE ID = @ID";

                        var affectedRows = await connection.ExecuteAsync(sql, account, transaction);

                        // Step 2: delete existing USER_ANALYSIS rows for this user
                        var deleteSql = @"DELETE FROM [dbo].[USER_ANALYSIS] 
                                  WHERE USER_ID = @ID";

                        await connection.ExecuteAsync(deleteSql, new { account.Id }, transaction);

                        // Step 3: insert the updated selections
                        if (account.User_Analysis != null && account.User_Analysis.Any())
                        {
                            var analysisSql = @"INSERT INTO [dbo].[USER_ANALYSIS] 
                                              (USER_ID, MAINT_ANALYSIS_ID)
                                              VALUES (@UserId, @AnalysisId)";

                            var rows = account.User_Analysis.Select(analysisId => new
                            {
                                UserId = account.Id,
                                AnalysisId = analysisId
                            });

                            await connection.ExecuteAsync(analysisSql, rows, transaction);
                        }

                        transaction.Commit();
                        return affectedRows > 0;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public async Task<bool> UpdateAsync_old(Account account)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE [dbo].[USER_LIST] 
                            SET USER_ID = @user_Id, 
                                USER_ROLE = @user_Role,
                                UPDATEDBY = @updatedBy,
                                UPDATEDTS = @updatedTs
                            WHERE ID = @ID";
                var affectedRows = await connection.ExecuteAsync(sql, account);
                return affectedRows > 0;
            }
        }

        public async Task<bool> UpdateLastLoginAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE [dbo].[USER_LIST] 
                            SET LastLoginTs = GETDATE() 
                            WHERE ID = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE [dbo].[USER_LIST] set ACTIVEFLAG = 'N' WHERE ID = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<Account> GetByAccountName(string accountName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                    var sql = "SELECT  * FROM [dbo].[USER_LIST] WHERE USER_ID = @accountName AND UPPER(ACTIVEFLAG) = 'Y'";
                    var result = await connection.QueryFirstOrDefaultAsync<Account>(sql, new { accountName });
                    return result;
            }
        }
    }
}
