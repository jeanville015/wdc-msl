using IDM.Model.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Repository.Main.Interface
{
    public interface IToolEntryItemsRepository
    {
        Task<IEnumerable<ToolEntryItems>> GetAllAsync();
        Task<IEnumerable<ToolEntryItems>> GetAllAsyncForApprove();
        Task<IEnumerable<ToolEntryItems>> GetAllAsyncByAmethystJob(string amethystJob);
        Task<IEnumerable<ToolEntryItems>> GetDataAsync(string deliveryDate, string receivedDate, string materialNo, string lotNumber, string jobNumber, string toolId);

    }
}
