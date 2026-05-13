using System.Collections.Generic;
using System.Threading.Tasks;
using IDM.DTO.Maintenance;

namespace IDM.Service.Maintenance.Interface
{
    public interface IMaterialService
    {
        Task<IEnumerable<MaterialDTO>> GetAllAsync();
        Task<MaterialDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(MaterialDTO materialDTO);
        Task<bool> UpdateAsync(MaterialDTO materialDTO);
        Task<bool> DeleteAsync(int id);
        Task<MaterialDTO> GetByMaterialName(string name);
    }
}
