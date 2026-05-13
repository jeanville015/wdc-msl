using System.Data;

namespace IDM.Data
{
    public interface IDb2ConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
