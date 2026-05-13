using AutoMapper;
using IDM.DTO.Main;
using IDM.Model.Main;

namespace IDM.Mapper
{
    public class MainProfile : Profile
    {
        public MainProfile() {
            #region Main
            CreateMap<IncomingData, IncomingDataDTO>().ReverseMap();
            CreateMap<ParameterDetail, ParameterDetailDTO>().ReverseMap();
            CreateMap<TrialDetail, TrialDetailDTO>().ReverseMap();
            CreateMap<ParameterTrial, ParameterTrialDTO>().ReverseMap();
            CreateMap<Data, DataDTO>().ReverseMap();
            CreateMap<ToolEntryItems, ToolEntryItemsDTO>().ReverseMap();
            CreateMap<Staging, StagingDTO>().ReverseMap();
            CreateMap<DynamicStaging, DynamicStagingDTO>().ReverseMap();
            CreateMap<SmallToolEntry, SmallToolEntryDTO>().ReverseMap();
            CreateMap<PendingData, PendingDataDTO>().ReverseMap();
            #endregion
        }
    }
}
