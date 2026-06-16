using IDM.Model.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Repository.Main.Interface
{
    public interface IPendingDataRepository
    {
        //GetPendingDataDetailsAsync(string deliveryDate, string receivedDate, string lotNumber, string materialNo, string jobNumber, string toolId, int page, int pageSize)
        Task<(IEnumerable<PendingData> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
        Task<(IEnumerable<PendingData> Items, int TotalCount)> GetPendingDataDetailsPaginatedAsync(string deliveryDate, string receivedDate, string lotNumber, string materialNo, string jobNumber, string toolId, int page, int pageSize);
        Task<IEnumerable<PendingData>> GetPendingDataDetailsAsync(string deliveryDate, string receivedDate, string lotNumber, string materialNo, string jobNumber, string toolId);
        Task<IEnumerable<PendingData>> GetPendingDataAsync();
        Task<int> UpdateDataParameterDetails(string status, string lotNumber, string deliveryDate, string receivedDate, string materialNo, string jobNumber, string toolId);
        Task<IEnumerable<ParameterTrial>> GetTrialAsync(IncomingData incomingData);
    }
}
