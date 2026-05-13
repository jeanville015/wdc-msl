using IDM.DTO.Maintenance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Interface
{
    public interface IToolService
    {
        Task<IEnumerable<ToolDTO>> GetAllAsync();
        Task<ToolDTO> GetByIdAsync(int id);
        Task<ToolDTO> GetByToolNameAsync(string toolName);
        Task<int> CreateAsync(ToolDTO toolDTO);
        Task<bool> UpdateAsync(ToolDTO toolDTO);
        Task<bool> DeleteAsync(int id);
        Task<ToolDTO> GetByToolName(string name, int toolTypeId);
    }
}
