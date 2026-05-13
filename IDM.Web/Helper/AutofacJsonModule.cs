using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IDM.Web.Helper
{
    public static class AutofacJsonModule
    {
        public static void Register(ContainerBuilder builder)
        {
            builder.RegisterInstance(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }).SingleInstance();
        }
    }
}