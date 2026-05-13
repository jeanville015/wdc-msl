using IDM.Model.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Interface
{
    public interface IManufacturerRepository
    {
        Task<IEnumerable<Manufacturer>> GetAllAsync();
        Task<Manufacturer> GetByIdAsync(int id);
        Task<int> CreateAsync(Manufacturer manufacturer);
        Task<bool> UpdateAsync(Manufacturer manufacturer);
        Task<bool> DeleteAsync(int id);
        Task<Manufacturer> GetByManufacturerName(string name);
    }
}
