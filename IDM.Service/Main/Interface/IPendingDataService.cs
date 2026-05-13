using IDM.DTO.Main;
using IDM.DTO.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Service.Main.Interface
{
    public interface IPendingDataService
    { 
        Task<PagedResultDTO<PendingDataDTO>> GetAllAsync(int page, int pageSize);
        Task<PagedResultDTO<PendingDataDTO>> GetPendingDataDetailsAsync(string deliveryDate, string receivedDate, string lotNumber, string materialNo, string jobNumber, string toolId, int page, int pageSize);
        Task<IEnumerable<PendingDataDTO>> GetPendingDataAsync();
    }
}
