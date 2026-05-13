using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Repository.Main.Interface
{
    public interface IDataRepository
    {
        Task<IEnumerable<Model.Main.Data>> GetAllAsync();
        Task<IEnumerable<Model.Main.Data>> GetDataAsync(string deliveryDate, string receivedDate, string materialNo, string lotNumber, string jobNumber, string toolId);

    }
}
