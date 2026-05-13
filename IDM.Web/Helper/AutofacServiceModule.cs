using Autofac;
using IDM.Model.Main;
using IDM.Service.Common.Interface;
using IDM.Service.Common.Service;
using IDM.Service.Main.Interface;
using IDM.Service.Main.Service;
using IDM.Service.Maintenance.Interface;
using IDM.Service.Maintenance.Service;
using IDM.Service.User.Interface;
using IDM.Service.User.Service;

namespace IDM.Web.Helper
{
    public static class AutofacServiceModule
    {
        public static void Register(ContainerBuilder builder)
        {
            #region Maintenance
            builder.RegisterType<UomService>()
                   .As<IUomService>()
                   .InstancePerRequest();

            builder.RegisterType<TestingSiteService>()
                   .As<ITestingSiteService>()
                   .InstancePerRequest();

            builder.RegisterType<ParameterService>()
                   .As<IParameterService>()
                   .InstancePerRequest();

            builder.RegisterType<AreaService>()
                   .As<IAreaService>()
                   .InstancePerRequest();

            builder.RegisterType<ManufacturerService>()
                   .As<IManufacturerService>()
                   .InstancePerRequest();

            builder.RegisterType<SupplierService>()
                   .As<ISupplierService>()
                   .InstancePerRequest();

            builder.RegisterType<MaterialService>()
                   .As<IMaterialService>()
                   .InstancePerRequest();

            builder.RegisterType<MaterialParameterService>()
                   .As<IMaterialParameterService>()
                   .InstancePerRequest();

            builder.RegisterType<ToolService>()
                   .As<IToolService>()
                   .InstancePerRequest();

            builder.RegisterType<ToolTypeService>()
                   .As<IToolTypeService>()
                   .InstancePerRequest();

            builder.RegisterType<AnalysisService>()
                  .As<IAnalysisService>()
                  .InstancePerRequest();

            builder.RegisterType<MaterialSettingsService>()
                  .As<IMaterialSettingsService>()
                  .InstancePerRequest();

            builder.RegisterType<DefectService>()
                  .As<IDefectService>()
                  .InstancePerRequest();
            #endregion

            #region User
            builder.RegisterType<RoleService>()
                   .As<IRoleService>()
                   .InstancePerRequest();

            builder.RegisterType<AccountService>()
                   .As<IAccountService>()
                   .InstancePerRequest();

            #endregion

            #region Main
            builder.RegisterType<IncomingDataService>()
                   .As<IIncomingDataService>()
                   .InstancePerRequest();

            builder.RegisterType<DataService>()
                    .As<IDataService>()
                    .InstancePerRequest();

            builder.RegisterType<PendingDataService>()
                    .As<IPendingDataService>()
                    .InstancePerRequest();

            builder.RegisterType<ToolEntryItemsService>()
                     .As<IToolEntryItemsService>()
                     .InstancePerRequest();

            builder.RegisterType<StagingService>()
                     .As<IStagingService>()
                     .InstancePerRequest();

            builder.RegisterType<DynamicStagingService>()
                     .As<IDynamicStagingService>()
                     .InstancePerRequest();

            builder.RegisterType<SmallToolEntryService>()
                     .As<ISmallToolEntryService>()
                     .InstancePerRequest();

            #endregion

            builder.RegisterType<UserService>()
                     .As<IUserService>()
                     .InstancePerRequest();

            builder.RegisterType<EmailService>()
                     .As<IEmailService>()
                     .InstancePerRequest();

        }
    }
}