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
            //_connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            _connectionString = "Provider=IBMDADB2;Database=SNJWWDB;Hostname=hadrnode02.hgst.com;Protocol=TCPIP;Connect Timeout=180;Port=9900;Uid=sifeats;Pwd=welcome123;";
        }

        public IDbConnection CreateConnection()
        {
            return new OleDbConnection(_connectionString);
        }
    }
}
