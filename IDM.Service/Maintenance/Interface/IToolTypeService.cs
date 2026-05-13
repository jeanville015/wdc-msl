using IDM.DTO.Maintenance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Interface
{
    public interface IToolTypeService
    {
        Task<IEnumerable<ToolTypeDTO>> GetAllAsync();
        Task<ToolTypeDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(ToolTypeDTO toolTypeDTO);
        Task<bool> UpdateAsync(ToolTypeDTO toolTypeDTO);
        Task<bool> DeleteAsync(int id);
        Task<ToolTypeDTO> GetByToolTypeName(string name);
    }
}
