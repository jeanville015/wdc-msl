using Autofac;
using Autofac.Integration.Mvc;
using System.Reflection;
using System.Web.Mvc;
using IDM.Web.Helper;

namespace IDM.Web
{
    public class AutofacConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            // Register Controllers
            //builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterControllers(typeof(MvcApplication).Assembly);


            // Modular Registration
            AutofacDataAccessModule.Register(builder);
            AutofacAutoMapperModule.Register(builder);
            AutofacJsonModule.Register(builder);
            AutofacServiceModule.Register(builder);
            AutofacRepositoryModule.Register(builder);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}