using IDM.DTO.Main;
using System.Threading.Tasks;

namespace IDM.Service.Main.Interface
{
    public interface ISmallToolEntryService
    {
        Task<int> CreateAsync(SmallToolEntryDTO smallToolEntryDTO);
        Task<int> GetAnalysisTrial(SmallToolEntryDTO smallToolEntryDTO);
        Task<int> CreateTrial(SmallToolEntryDTO smallToolEntryDTO);
    }
}