using IDM.Model.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Interface
{
    public interface IMaterialParameterRepository
    {
        Task<IEnumerable<MaterialParameter>> GetAllAsync();
        Task<IEnumerable<MaterialParameter>> GetByIdAsync(int id);
        Task<IEnumerable<MaterialParameter>> GetByMaterialNoAsync(string id);
        Task<int> CreateAsync(MaterialParameter materialParameter);
        Task<bool> UpdateAsync(MaterialParameter materialParameter);
        Task<bool> DeleteAsync(int id);
        Task<MaterialParameter> GetByParameterAndSite(MaterialParameter materialParameter);
    }
}
