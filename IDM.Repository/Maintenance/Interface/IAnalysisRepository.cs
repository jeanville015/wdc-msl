using IDM.Model.Maintenance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Interface
{
    public interface IAnalysisRepository
    {
        Task<IEnumerable<Analysis>> GetAllAsync();
        Task<Analysis> GetByIdAsync(int id);
        Task<Analysis> GetByToolTypeAndAnalysisAsync(int toolTypeId, string analysisName);
        Task<Analysis> GetByAnalysisAsync(string analysisName);
        Task<int> CreateAsync(Analysis analysis);
        Task<bool> UpdateAsync(Analysis analysis);
        Task<bool> DeleteAsync(int id);
        Task<Analysis> GetByAnalysisName(string name, int toolTypeId);
    }
}
