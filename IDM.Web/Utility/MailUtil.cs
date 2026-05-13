using IDM.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace IDM.Web.Utility
{
    public static class MailUtil
    {
        public static void SendMail(MailViewModel mailModel)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(mailModel.From);
                    mail.To.Add(mailModel.To);
                    mail.Subject = mailModel.Subject;
                    mail.Body = mailModel.Body;
                    mail.IsBodyHtml = true;

                    //Way to add attachment
                    foreach (string attachment in mailModel.Attachments)
                    {
                        mail.Attachments.Add(new Attachment(attachment));
                    }
                    using (SmtpClient smtp = new SmtpClient(mailModel.Host, mailModel.Port))
                    {
                        smtp.EnableSsl = mailModel.IsSSLEnabled;
                        smtp.Send(mail);
                    }
                }
            }
            catch (SmtpFailedRecipientException exception)
            {
            }
            catch (Exception ex)
            {
            }
        }
    }
}