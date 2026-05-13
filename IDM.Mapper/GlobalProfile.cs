using AutoMapper;
using IDM.DTO;
using IDM.Model;

namespace IDM.Mapper
{
    public class GlobalProfile : Profile
    {
        public GlobalProfile() {
            #region Main
            CreateMap<Config, ConfigDTO>().ReverseMap();
            #endregion
        }
    }
}
