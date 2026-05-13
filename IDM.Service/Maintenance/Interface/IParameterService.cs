using IDM.DTO.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Interface
{
    public interface IParameterService
    {
        Task<IEnumerable<ParameterDTO>> GetAllAsync();
        Task<ParameterDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(ParameterDTO parameterDTO);
        Task<bool> UpdateAsync(ParameterDTO parameterDTO);
        Task<bool> DeleteAsync(int id);
        Task<ParameterDTO> GetByParameterName(string name);
    }
}
