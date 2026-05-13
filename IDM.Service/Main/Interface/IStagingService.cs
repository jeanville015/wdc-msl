using IDM.DTO;
using IDM.DTO.Main;
using IDM.DTO.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Service.Main.Interface
{
    public interface IStagingService
    {
        Task<IEnumerable<StagingDTO>> GetByJobAndAnalysisAsync(string table, string amethysJob, string analysis, int analysisTrial);

    }
}
