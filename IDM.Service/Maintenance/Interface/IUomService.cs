using IDM.DTO.Maintenance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Interface
{
    public interface IUomService
    {
        Task<IEnumerable<UomDTO>> GetAllAsync();
        Task<UomDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(UomDTO uomDTO);
        Task<bool> UpdateAsync(UomDTO uomDTO);
        Task<bool> DeleteAsync(int id);
        Task<UomDTO> GetByUomName(string name);
    }
}
