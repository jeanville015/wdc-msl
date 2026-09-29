using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace IDM.Data
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        public SqlConnectionFactory()
        {
            var connectionSetting = ConfigurationManager.ConnectionStrings["DBConnection"];

            if (connectionSetting == null || string.IsNullOrWhiteSpace(connectionSetting.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "The 'DBConnection' connection string is missing. Configure it in IDM.Web/ConnectionStrings.config.");
            }

            _connectionString = connectionSetting.ConnectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
