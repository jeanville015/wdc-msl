using IDM.DTO.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Interface
{
    public interface IMaterialParameterService
    {
        Task<IEnumerable<MaterialParameterDTO>> GetAllAsync();
        Task<IEnumerable<MaterialParameterDTO>> GetByIdAsync(int id);
        Task<IEnumerable<MaterialParameterDTO>> GetByMaterialNoAsync(string id);
        Task<int> CreateAsync(MaterialParameterDTO materialParameterDTO);
        Task<bool> UpdateAsync(MaterialParameterDTO materialParameterDTO);
        Task<bool> DeleteAsync(int id);
        Task<MaterialParameterDTO> GetByParameterAndSite(MaterialParameterDTO materialParameterDTO);
    }
}
