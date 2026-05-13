using IDM.Model.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Interface
{
    public interface ISupplierRepository
    {
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<Supplier> GetByIdAsync(int id);
        Task<int> CreateAsync(Supplier supplier);
        Task<bool> UpdateAsync(Supplier supplier);
        Task<bool> DeleteAsync(int id);
        Task<Supplier> GetBySupplierName(string name);
    }
}
