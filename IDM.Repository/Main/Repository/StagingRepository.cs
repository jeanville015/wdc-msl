using Dapper;
using IDM.Data;
using IDM.Model.Main;
using IDM.Model.User;
using IDM.Repository.Main.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Main.Repository
{
    public class StagingRepository : IStagingRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public StagingRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Staging>> GetByJobAndAnalysisAsync(string table, string amethystJob, string analysis, int analysisTrial)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    var sql = $"SELECT * FROM {table} WHERE AmethystJob = @amethystJob AND Analysis = @analysis AND AnalysisTrial = @analysisTrial";
                    return await connection.QueryAsync<Staging>(sql, new { amethystJob, analysis, analysisTrial });
                }catch(Exception ex)
                { throw; }
            }
        }
    }
}
