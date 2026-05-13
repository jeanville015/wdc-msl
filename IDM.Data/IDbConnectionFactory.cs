using System.Data;

namespace IDM.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
