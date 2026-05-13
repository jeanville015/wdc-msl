using IDM.Model.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Main.Interface
{
    public interface ISmallToolEntryRepository
    {
        Task<int> CreateAsync(SmallToolEntry smallToolEntry);
        Task<int> GetAnalysisTrial(SmallToolEntry smallToolEntry);
        Task<int> CreateTrial(SmallToolEntry smallToolEntry);
        Task<int> RetireTrial(SmallToolEntry smallToolEntry);
    }
}
