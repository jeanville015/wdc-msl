using IDM.Model.Maintenance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Interface
{
    public interface IToolRepository
    {
        Task<IEnumerable<Tool>> GetAllAsync();
        Task<Tool> GetByIdAsync(int id);
        Task<Tool> GetByToolNameAsync(string toolName);
        Task<int> CreateAsync(Tool tool);
        Task<bool> UpdateAsync(Tool tool);
        Task<bool> DeleteAsync(int id);
        Task<Tool> GetByToolName(string name, int toolTypeId);
    }
}
