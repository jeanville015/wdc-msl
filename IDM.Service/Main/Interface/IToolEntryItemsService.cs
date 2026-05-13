using IDM.DTO.Main;
using IDM.DTO.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Service.Main.Interface
{
    public interface IToolEntryItemsService
    {
        Task<IEnumerable<ToolEntryItemsDTO>> GetAllAsync();
        Task<IEnumerable<ToolEntryItemsDTO>> GetAllAsyncForApprove();
        Task<IEnumerable<ToolEntryItemsDTO>> GetAllAsyncByAmethystJob(string amethystJob);
        Task<IEnumerable<ToolEntryItemsDTO>> GetDataAsync(string deliveryDate, string receivedDate, string materialNo, string lotNumber, string jobNumber, string toolId);
    }
}
