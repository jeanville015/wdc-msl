using Autofac;
using System.Data;
using IDM.Data;

namespace IDM.Web.Helper
{
    public static class AutofacDataAccessModule
    {
        public static void Register(ContainerBuilder builder)
        {
            builder.RegisterType<SqlConnectionFactory>()
                   .As<IDbConnectionFactory>()
                   .InstancePerRequest();

            builder.Register(c =>
            {
                var factory = c.Resolve<IDbConnectionFactory>();
                return factory.CreateConnection();
            }).As<IDbConnection>().InstancePerRequest();


            builder.RegisterType<Db2ConnectionFactory>()
                   .As<IDb2ConnectionFactory>()
                   .InstancePerRequest();

            builder.Register(c =>
            {
                var factory = c.Resolve<IDb2ConnectionFactory>();
                return factory.CreateConnection();
            }).As<IDbConnection>().InstancePerRequest();
        }
    }
}