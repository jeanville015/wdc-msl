using Autofac;
using IDM.Repository.Main.Interface;
using IDM.Repository.Main.Repository;
using IDM.Repository.Maintenance.Interface;
using IDM.Repository.Maintenance.Repository;
using IDM.Repository.User.Interface;
using IDM.Repository.User.Repository;


namespace IDM.Web.Helper
{
    public static class AutofacRepositoryModule
    {
        public static void Register(ContainerBuilder builder)
        {
            #region Maintenance
            builder.RegisterType<UomRepository>()
                   .As<IUomRepository>()
                   .InstancePerRequest();

            builder.RegisterType<TestingSiteRepository>()
                   .As<ITestingSiteRepository>()
                   .InstancePerRequest();

            builder.RegisterType<ParameterRepository>()
                   .As<IParameterRepository>()
                   .InstancePerRequest();

            builder.RegisterType<AreaRepository>()
                   .As<IAreaRepository>()
                   .InstancePerRequest();

            builder.RegisterType<ManufacturerRepository>()
                   .As<IManufacturerRepository>()
                   .InstancePerRequest();

            builder.RegisterType<SupplierRepository>()
                   .As<ISupplierRepository>()
                   .InstancePerRequest();

            builder.RegisterType<MaterialRepository>()
                   .As<IMaterialRepository>()
                   .InstancePerRequest();

            builder.RegisterType<MaterialParameterRepository>()
                   .As<IMaterialParameterRepository>()
                   .InstancePerRequest();

            builder.RegisterType<ToolRepository>()
                   .As<IToolRepository>()
                   .InstancePerRequest();

            builder.RegisterType<ToolTypeRepository>()
                    .As<IToolTypeRepository>()
                    .InstancePerRequest();

            builder.RegisterType<AnalysisRepository>()
                    .As<IAnalysisRepository>()
                    .InstancePerRequest();

            builder.RegisterType<MaterialSettingsRepository>()
                    .As<IMaterialSettingsRepository>()
                    .InstancePerRequest();

            builder.RegisterType<DefectRepository>()
                 .As<IDefectRepository>()
                 .InstancePerRequest();
            #endregion

            #region User
            builder.RegisterType<RoleRepository>()
                   .As<IRoleRepository>()
                   .InstancePerRequest();

            builder.RegisterType<AccountRepository>()
                   .As<IAccountRepository>()
                   .InstancePerRequest();

            #endregion

            #region Main
            builder.RegisterType<IncomingDataRepository>()
                   .As<IIncomingDataRepository>()
                   .InstancePerRequest();

            builder.RegisterType<DataRepository>()
                   .As<IDataRepository>()
                   .InstancePerRequest();

            builder.RegisterType<PendingDataRepository>()
                   .As<IPendingDataRepository>()
                   .InstancePerRequest();

            builder.RegisterType<ToolEntryItemsRepository>()
                   .As<IToolEntryItemsRepository>()
                   .InstancePerRequest();

            builder.RegisterType<StagingRepository>()
                   .As<IStagingRepository>()
                   .InstancePerRequest();

            builder.RegisterType<DynamicStagingRepository>()
                   .As<IDynamicStagingRepository>()
                   .InstancePerRequest();

            builder.RegisterType<SmallToolEntryRepository>()
                   .As<ISmallToolEntryRepository>()
                   .InstancePerRequest();

            #endregion

        }
    }
}