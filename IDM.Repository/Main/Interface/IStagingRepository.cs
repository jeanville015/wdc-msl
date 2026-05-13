using IDM.Model.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Repository.Main.Interface
{
    public interface IStagingRepository
    {
        Task<IEnumerable<Staging>> GetByJobAndAnalysisAsync(string table, string amethysJob, string analysis, int analysisTrial);

    }
}
