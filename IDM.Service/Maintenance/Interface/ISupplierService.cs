using IDM.DTO.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Interface
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierDTO>> GetAllAsync();
        Task<SupplierDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(SupplierDTO supplierDTO);
        Task<bool> UpdateAsync(SupplierDTO supplierDTO);
        Task<bool> DeleteAsync(int id);
        Task<SupplierDTO> GetBySupplierName(string name);
    }
}
