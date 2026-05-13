using IDM.Model.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Interface
{
    public interface IAreaRepository
    {
        Task<IEnumerable<Area>> GetAllAsync();
        Task<Area> GetByIdAsync(int id);
        Task<int> CreateAsync(Area area);
        Task<bool> UpdateAsync(Area area);
        Task<bool> DeleteAsync(int id);
        Task<Area> GetByAreaName(string name);
    }
}
