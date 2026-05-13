using IDM.DTO.Maintenance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Interface
{
    public interface IMaterialSettingsService
    {
        Task<IEnumerable<MaterialSettingsDTO>> GetAllAsync();
        Task<MaterialSettingsDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(MaterialSettingsDTO materialSettingsDTO);
        Task<bool> UpdateAsync(MaterialSettingsDTO materialSettingsDTO);
        Task<bool> DeleteAsync(int id);
        Task<MaterialSettingsDTO> GetBySettingsName(string name, int toolTypeId, int materialId);
    }
}
