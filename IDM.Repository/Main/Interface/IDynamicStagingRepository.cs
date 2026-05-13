using IDM.Model.Main;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using IDM.Model.Common;

namespace IDM.Repository.Main.Interface
{
    public interface IDynamicStagingRepository
    {
        Task<IEnumerable<DynamicStaging>> GetByJobAndAnalysisAsync(string table, string amethystJob, string analysis, int analysisTrial);
        Task<IEnumerable<DynamicStaging>> GetDataStagingDefectAsync(string table, string amethystJob, string analysis, int analysisTrial, string area, string subArea);
        Task<IEnumerable<DynamicStaging>> GetDataStagingDefectWithDataStagingWImageParamsAsync(string table, string amethystJob, string analysis, int analysisTrial);
        Task<DataTable> GetByJobAndAnalysisDataTableAsync(string table, string amethystJob, string analysis, int analysisTrial);
        Task<DataTable> GetDataStagingDefectDataTableAsync(string table, string amethystJob, string analysis, int analysisTrial, string area = null, string subArea = null);
        Task<bool> SetApprovalAsync(string table, string amethystJob, string analysis, int analysisTrial, string status, string toolName, object tableData, string updatedBy);
        Task<bool> UpdateStagingDataValueAsync(string Table, DataIdValuePair _DataIdValuePair, DataNameValuePair _DataNameValuePair);
        Task<string> GetCustomerAsync(string amethystJob, string analysis, int analysisTrial);
        Task<string> GetStatusAsync(string amethystJob, string analysis, int analysisTrial);

    }
}
