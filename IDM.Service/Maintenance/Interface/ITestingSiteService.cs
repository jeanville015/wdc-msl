using IDM.DTO.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Interface
{
    public interface ITestingSiteService
    {
        Task<IEnumerable<TestingSiteDTO>> GetAllAsync();
        Task<TestingSiteDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(TestingSiteDTO testingSiteDTO);
        Task<bool> UpdateAsync(TestingSiteDTO testingSiteDTO);
        Task<bool> DeleteAsync(int id);
        Task<TestingSiteDTO> GetBySiteName(string name);
    }
}
