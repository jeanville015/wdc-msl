using IDM.Model.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Interface
{
    public interface IMaterialRepository
    {
        Task<IEnumerable<Material>> GetAllAsync();
        Task<Material> GetByIdAsync(int id);
        Task<int> CreateAsync(Material material);
        Task<bool> UpdateAsync(Material material);
        Task<bool> DeleteAsync(int id);
        Task<Material> GetByMaterialName(string name);
    }
}
