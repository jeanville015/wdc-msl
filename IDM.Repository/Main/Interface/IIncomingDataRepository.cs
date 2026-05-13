using IDM.Model.Main;
using IDM.Model.Maintenance;
using IDM.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Main.Interface
{
    public interface IIncomingDataRepository
    {
        Task<int> CreateAsync(IncomingData incomingData);
        Task<int> CreateTrial(IncomingData incomingData);
        Task<int> RetireTrial(IncomingData incomingData);
        Task<IEnumerable<ParameterTrial>> GetTrialAsync(IncomingData incomingData);
    }
}
