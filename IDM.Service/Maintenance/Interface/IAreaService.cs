using IDM.DTO.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Interface
{
    public interface IAreaService
    {
        Task<IEnumerable<AreaDTO>> GetAllAsync();
        Task<AreaDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(AreaDTO areaDTO);
        Task<bool> UpdateAsync(AreaDTO areaDTO);
        Task<bool> DeleteAsync(int id);
        Task<AreaDTO> GetByAreaName(string name);
    }
}
