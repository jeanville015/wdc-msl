using Autofac;
using AutoMapper;

namespace IDM.Web.Helper
{
    public static class AutofacAutoMapperModule
    {
        public static void Register(ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddMaps("IDM.Mapper");
  
                 });

                return config;
            }).AsSelf().SingleInstance();

            builder.Register(ctx =>
            {
                var context = ctx.Resolve<MapperConfiguration>();
                return context.CreateMapper();
            }).As<IMapper>().InstancePerLifetimeScope();
        }
    }
}