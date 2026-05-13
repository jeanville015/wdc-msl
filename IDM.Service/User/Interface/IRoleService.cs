using IDM.DTO.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Service.User.Interface
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetAllAsync();
        Task<RoleDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(RoleDTO roleDTO);
        Task<bool> UpdateAsync(RoleDTO roleDTO);
        Task<bool> DeleteAsync(int id);
        Task<RoleDTO> GetByRoleName(string name);
    }
}
