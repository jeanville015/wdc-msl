using System.Configuration;
using System.Data;
using System.Data.OleDb;

namespace IDM.Data
{
    public class Db2ConnectionFactory : IDb2ConnectionFactory
    {
        private readonly string _connectionString;
        public Db2ConnectionFactory()
        {
            var connectionSetting = ConfigurationManager.ConnectionStrings["DB2Connection"];

            if (connectionSetting == null || string.IsNullOrWhiteSpace(connectionSetting.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "The 'DB2Connection' connection string is missing. Configure it in IDM.Web/ConnectionStrings.config.");
            }

            _connectionString = connectionSetting.ConnectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new OleDbConnection(_connectionString);
        }
    }
}
