using IDM.Model.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Repository.User.Interface
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role> GetByIdAsync(int id);
        Task<int> CreateAsync(Role role);
        Task<bool> UpdateAsync(Role role);
        Task<bool> DeleteAsync(int id);
        Task<Role> GetByRoleName(string name);
    }
}
