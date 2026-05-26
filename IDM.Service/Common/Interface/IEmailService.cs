using IDM.DTO;
using IDM.DTO.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Service.Common.Interface
{
    public interface IEmailService
    {
        Task<bool> SendFailedDataEmailAsync(IncomingDataDTO incomingData);
        Task<bool> SendRejectionEmailAsync(IEnumerable<string> userList, string analyzedBy, string job, string analysis, int analysisTrial, string status, string approver);
        Task<bool> SendApprovalEmailAsync(IEnumerable<string> userList, string analyzedBy, string job, string analysis, int analysisTrial, string status, string approver, string customer, string returnUrl);
    }
}
