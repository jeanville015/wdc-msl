using IDM.Model.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Interface
{
    public interface ITestingSiteRepository
    {
        Task<IEnumerable<TestingSite>> GetAllAsync();
        Task<TestingSite> GetByIdAsync(int id);
        Task<int> CreateAsync(TestingSite testingSite);
        Task<bool> UpdateAsync(TestingSite testingSite);
        Task<bool> DeleteAsync(int id);
        Task<TestingSite> GetBySiteName(string name);
    }
}
