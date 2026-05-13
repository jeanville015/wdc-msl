using IDM.DTO;
using IDM.Service.Common.Interface;
using System.Threading.Tasks;

namespace IDM.Web.Utility
{
    public class MailSender : IMailSender
    {
        public void SendMail(MailViewModel mailModel)
        {
            MailUtil.SendMail(mailModel);
        }

        public Task SendMailAsync(MailViewModel mailModel)
        {
            return Task.Run(() => SendMail(mailModel));
        }
    }
}
