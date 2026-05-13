using IDM.Model.Maintenance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Interface
{
    public interface IMaterialSettingsRepository
    {
        Task<IEnumerable<MaterialSettings>> GetAllAsync();
        Task<MaterialSettings> GetByIdAsync(int id);
        Task<int> CreateAsync(MaterialSettings materialSettings);
        Task<bool> UpdateAsync(MaterialSettings materialSettings);
        Task<bool> DeleteAsync(int id);
        Task<MaterialSettings> GetBySettingsName(string name, int toolTypeId, int materialId);
    }
}
