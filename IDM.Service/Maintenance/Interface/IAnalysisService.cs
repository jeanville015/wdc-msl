using IDM.DTO.Maintenance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Interface
{
    public interface IAnalysisService
    {
        Task<IEnumerable<AnalysisDTO>> GetAllAsync();
        Task<AnalysisDTO> GetByIdAsync(int id);
        Task<AnalysisDTO> GetByToolTypeAndAnalysisAsync(int toolTypeId, string analysisName);
        Task<AnalysisDTO> GetByAnalysisAsync(string analysisName);
        Task<int> CreateAsync(AnalysisDTO analysisDTO);
        Task<bool> UpdateAsync(AnalysisDTO analysisDTO);
        Task<bool> DeleteAsync(int id);
        Task<AnalysisDTO> GetByAnalysisName(string name, int toolTypeId);
    }
}
