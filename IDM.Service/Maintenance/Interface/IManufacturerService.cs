using IDM.DTO.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Interface
{
    public interface IManufacturerService
    {
        Task<IEnumerable<ManufacturerDTO>> GetAllAsync();
        Task<ManufacturerDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(ManufacturerDTO manufacturerDTO);
        Task<bool> UpdateAsync(ManufacturerDTO manufacturerDTO);
        Task<bool> DeleteAsync(int id);
        Task<ManufacturerDTO> GetByManufacturerName(string name);
    }
}
