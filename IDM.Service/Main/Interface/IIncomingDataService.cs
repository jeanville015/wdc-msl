using IDM.DTO;
using IDM.DTO.Main;
using IDM.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Service.Main.Interface
{
    public interface IIncomingDataService
    {
        Task<int> CreateAsync(IncomingDataDTO incomingDataDTO);
        Task<int> CreateTrial(IncomingDataDTO incomingDataDTO);
        Task<int> MQUploadPreparationParameter(IncomingDataDTO incomingDataDTO, ConfigDTO configDTO);
        Task<int> MQUploadPreparationTrial(IncomingDataDTO incomingDataDTO, ConfigDTO configDTO);
    }
}
