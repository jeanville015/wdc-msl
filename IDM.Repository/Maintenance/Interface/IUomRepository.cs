using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDM.Model.Maintenance;

namespace IDM.Repository.Maintenance.Interface
{
    public interface IUomRepository
    {
        Task<IEnumerable<Uom>> GetAllAsync();
        Task<Uom> GetByIdAsync(int id);
        Task<int> CreateAsync(Uom uom);
        Task<bool> UpdateAsync(Uom uom);
        Task<bool> DeleteAsync(int id);
        Task<Uom> GetByUomName(string name);
    }
}
