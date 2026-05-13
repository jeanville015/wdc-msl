using IDM.DTO;
using System.Threading.Tasks;

namespace IDM.Service.Common.Interface
{
    public interface IMailSender
    {
        Task SendMailAsync(MailViewModel mailModel);
        void SendMail(MailViewModel mailModel);
    }
}
