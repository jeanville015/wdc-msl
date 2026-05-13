using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace IDM.Infrastructure.Database
{
    public class ConnectionFactory
    {
        private readonly string _connectionString;

        public ConnectionFactory(IOptions<DatabaseOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        public IDbConnection Create()
        {
            return new SqlConnection(_connectionString);
        }
    }
}