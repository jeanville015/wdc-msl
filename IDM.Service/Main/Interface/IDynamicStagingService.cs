using IDM.DTO;
using IDM.DTO.Main;
using IDM.Model.Common;
using IDM.Model.Main;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static IDM.Service.Main.Service.DynamicStagingService;

namespace IDM.Service.Main.Interface
{
    public interface IDynamicStagingService
    {
        Task<IEnumerable<DynamicStagingDTO>> GetByJobAndAnalysisAsync(string table, string amethystJob, string analysis, int analysisTrial);
        Task<IEnumerable<DynamicStagingDTO>> GetDataStagingDefectAsync(string table, string amethystJob, string analysis, int analysisTrial, string area, string subArea);
        Task<IEnumerable<DynamicStagingDTO>> GetDataStagingDefectWithDataStagingWImageParamsAsync(string table, string amethystJob, string analysis, int analysisTrial);
        Task<DataTable> GetByJobAndAnalysisDataTableAsync(string table, string amethystJob, string analysis, int analysisTrial);
        Task<bool> SetApprovalAsync(string table, string amethystJob, string analysis, int analysisTrial, string status, string toolName, object tableData, string updatedBy);
        Task<bool> UpdateStagingDataValueAsync(string Table, DataIdValuePair _DataIdValuePair, DataNameValuePair _DataNameValuePair);
        Task<string> GetCustomerAsync(string amethystJob, string analysis, int analysisTrial);
        Task<string> GetStatusAsync(string amethystJob, string analysis, int analysisTrial);
        Task<bool> MQUploadPreparationParameter(object table, ConfigDTO configDTO, string destinationTable, string approver);
        Task<OperationResult> SetApprovalStagingWImage(string sourceTable, string table, string amethystJob, string analysis, int analysisTrial, string analyzedBy, string status, string tableContent, string userId, string returnUrl = null);
        Task<OperationResult> PrepareMQInputDataTable(DataTable dataTable, string sourceTable, string destinationTable, string userId, ConfigDTO configDTO, string[] excludedFields);
        Task<OperationResult> ProcessApprovalWithNotificationAsync(string sourceTable, string table, string amethystJob, string analysis, int analysisTrial, string analyzedBy, string status, object tableData, string userId, ConfigDTO config, string returnUrl = null);

    }
}
