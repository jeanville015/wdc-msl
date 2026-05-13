using IDM.Model.Maintenance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Interface
{
    public interface IToolTypeRepository
    {
        Task<IEnumerable<ToolType>> GetAllAsync();
        Task<ToolType> GetByIdAsync(int id);
        Task<int> CreateAsync(ToolType toolType);
        Task<bool> UpdateAsync(ToolType toolType);
        Task<bool> DeleteAsync(int id);
        Task<ToolType> GetByToolTypeName(string name);
    }
}
