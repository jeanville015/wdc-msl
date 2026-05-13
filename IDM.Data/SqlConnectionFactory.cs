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
            //_connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            _connectionString = "Data Source=PBT-DD-SQLMFG01.AD.SHARED;Initial Catalog=IDM;User ID=idmuser;Password=idM@U$er65894;";
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
