using AutoMapper;
using IDM.Application.DTOs.Maintenance;
using IDM.Domain.Entities.Maintenance;

namespace IDM.Mapper
{
    public class MaintenanceProfile : Profile
    {
        public MaintenanceProfile()
        {
            #region Maintenance
            //CreateMap<Uom, UomDTO>().ReverseMap();
            //CreateMap<TestingSite, TestingSiteDTO>().ReverseMap();
            //CreateMap<Parameter, ParameterDTO>().ReverseMap();
            CreateMap<Area, AreaDTO>().ReverseMap();
            //CreateMap<Manufacturer, ManufacturerDTO>().ReverseMap();
            //CreateMap<Supplier, SupplierDTO>().ReverseMap();
            //CreateMap<Material, MaterialDTO>().ReverseMap();
            //CreateMap<MaterialParameter, MaterialParameterDTO>().ReverseMap();
            #endregion

        }
    }
}
