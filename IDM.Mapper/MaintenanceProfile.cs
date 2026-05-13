using AutoMapper;
using IDM.DTO.Maintenance;
using IDM.Model.Maintenance;

namespace IDM.Mapper
{
    public class MaintenanceProfile : Profile
    {
        public MaintenanceProfile()
        {
            #region Maintenance
            CreateMap<Uom, UomDTO>().ReverseMap();
            CreateMap<TestingSite, TestingSiteDTO>().ReverseMap();
            CreateMap<Parameter, ParameterDTO>().ReverseMap();
            CreateMap<Area, AreaDTO>().ReverseMap();
            CreateMap<Manufacturer, ManufacturerDTO>().ReverseMap();
            CreateMap<Supplier, SupplierDTO>().ReverseMap();
            CreateMap<Material, MaterialDTO>().ReverseMap();
            CreateMap<MaterialParameter, MaterialParameterDTO>().ReverseMap();
            CreateMap<Tool, ToolDTO>().ReverseMap();
            CreateMap<ToolType, ToolTypeDTO>().ReverseMap();
            CreateMap<Analysis, AnalysisDTO>().ReverseMap();
            CreateMap<MaterialSettings, MaterialSettingsDTO>().ReverseMap();
            CreateMap<Defect, DefectDTO>().ReverseMap();
            CreateMap<DefectBulk, DefectBulkDTO>().ReverseMap();
            #endregion

        }
    }
}
