using IDM.DTO.Main;
using IDM.DTO.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Service.Main.Interface
{
    public interface IDataService
    {
        Task<IEnumerable<DataDTO>> GetAllAsync();
        Task<IEnumerable<DataDTO>> GetDataAsync(string deliveryDate, string receivedDate, string materialNo, string lotNumber, string jobNumber, string toolId);
    }
}
