using AutoMapper;
using IDM.DTO.User;
using IDM.Model.User;

namespace IDM.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            #region User
            CreateMap<Account, AccountDTO>().ReverseMap();
            CreateMap<Role, RoleDTO>().ReverseMap();
            #endregion
        }
    }
}
