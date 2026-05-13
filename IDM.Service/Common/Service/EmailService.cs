using IDM.DTO;
using IDM.DTO.Main;
using IDM.Service.Common.Interface;
using System;
using System.Threading.Tasks;

namespace IDM.Service.Common.Service
{
    public class EmailService : IEmailService
    {
        private readonly IUserService _userService;
        private readonly ConfigDTO _config;
        private readonly IMailSender _mailSender;

        public EmailService(IUserService userService, ConfigDTO config, IMailSender mailSender)
        {
            _userService = userService;
            _config = config;
            _mailSender = mailSender;
        }

        public async Task<bool> SendFailedDataEmailAsync(IncomingDataDTO incomingData)
        {
            try
            {
                string emailContent = BuildEmailHeader("IDM Team", "IDM Data Entry") +
                                        "<p><span class='warning'>FAILED/OOC</span> for the following:</p>" +

                                        // Basic Material Info Table (20% width)
                                        "<table class='material-info'>" +
                                        "<colgroup><col class='col-10'><col class='col-10'></colgroup>" +
                                        "<tr><th>Field</th><th>Value</th></tr>" +
                                        $"<tr><td>Material Number</td><td>{incomingData.Material_No}</td></tr>" +
                                        $"<tr><td>Material Name</td><td>{incomingData.Material_Name}</td></tr>" +
                                        $"<tr><td>Lot Number Name</td><td>{incomingData.LotNumber}</td></tr>" +
                                        $"<tr><td>Delivery Date</td><td>{incomingData.Delivery_Date}</td></tr>" +
                                        $"<tr><td>Received Date</td><td>{incomingData.Received_Date}</td></tr>" +
                                        "</table>" +

                                        // Parameters Table (100% width)
                                        "<p><span class='bold'>Parameter Details:</span></p>" +
                                        "<table class='parameters'>" +
                                        "<colgroup>" +
                                        "<col class='col-10'>" +  // Parameter Name
                                        "<col class='col-10'>" +  // Parameter Value
                                        "<col style='width:5%;'>" +  // UOM
                                        "<col style='width:10%;'>" +  // Specs Limit
                                        "<col style='width:10%;'>" +  // Control Limit
                                        "<col style='width:5%;'>" +  // Site
                                        "<col style='width:10%;'>" +  // Judgement
                                        "<col style='width:10%;'>" +  // Control Judgement
                                        "</colgroup>" +
                                        "<tr>" +
                                        "<th>Parameter Name</th><th>Parameter Value</th><th>UOM</th><th>Specs Limit</th><th>Control Limit</th>" +
                                        "<th>Site</th><th>Judgement</th><th>Control Judgement</th>" +
                                        "</tr>";

                // Loop through parameters
                foreach (var p in incomingData.Parameters)
                {
                    emailContent += $"<tr>" +
                        $"<td>{p.Parameter_Name}</td>" +
                        $"<td>{p.Parameter_Value}</td>" +
                        $"<td>{p.Uom_Name}</td>" +
                        $"<td>{p.Lower_Specs_Limit} : {p.Upper_Specs_Limit}</td>" +
                        $"<td>{p.Lower_Control_Limit} : {p.Lower_Control_Limit}</td>" +
                        $"<td>{p.Site_Name}</td>" +
                        $"<td>{p.Specs_Judgement}</td>" +
                        $"<td>{p.Control_Judgement}</td>" +
                        $"</tr>";
                }

                emailContent += "</table>" + BuildEmailFooter(null);

                var mailModel = CreateMailViewModel(_config.DefaultEmailRecipients, "IDM Data Entry : Failed/OOC", emailContent);
                _mailSender.SendMail(mailModel);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending failed data email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendRejectionEmailAsync(string analyzedBy, string job, string analysis, int analysisTrial, string status, string approver)
        {
            try
            {
                string recipientName = await _userService.GetUserNameAsync(analyzedBy);
                string approverName = await _userService.GetUserNameAsync(approver);
                string recipientEmail = await _userService.GetUserEmailAsync(analyzedBy);

                string emailContent = BuildEmailHeader(recipientName, "MSL Data") +
                                        $"<p>The Analysis {analysis} for Job {job} and Trial count of {analysisTrial} has been <span class='warning'>{status}</span> by Ma'am/Sir {approverName}</p>" +
                                        BuildEmailFooter(null);

                var mailModel = CreateMailViewModel(recipientEmail, "MSL Web System : FAILED", emailContent);
                _mailSender.SendMail(mailModel);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending rejection email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendApprovalEmailAsync(string analyzedBy, string job, string analysis, int analysisTrial, string status, string approver, string customer, string returnUrl)
        {
            try
            {
                string recipientName = await _userService.GetUserNameAsync(analyzedBy);
                string approverName = await _userService.GetUserNameAsync(approver);
                string recipientEmail = await _userService.GetUserEmailAsync(analyzedBy);
                
                // Combine customer email with analyzedBy email
                string allRecipients = string.IsNullOrEmpty(customer) ? recipientEmail : $"{customer}, {recipientEmail}";

                string emailContent = BuildEmailHeader(recipientName, "MSL Data") +
                                        $"<p>The Analysis {analysis} for Job {job} and Trial count of {analysisTrial} has been <span class='success'>{status}</span> by Ma'am/Sir {approverName}</p>" +
                                        BuildEmailFooter(returnUrl);

                var mailModel = CreateMailViewModel(allRecipients, "MSL Web System : APPROVED", emailContent);
                _mailSender.SendMail(mailModel);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending approval email: {ex.Message}");
                return false;
            }
        }

        
        private string BuildEmailHeader(string recipientName, string emailType)
        {
            return "<!DOCTYPE html><html><head>" +
                   "<style>" +
                   "table { border-collapse: collapse; font-size: 12px; margin-bottom: 20px; }" +
                   ".parameters { width: 70%; }" +
                   ".material-info { width: 20%; margin-left: 0; }" +
                   ".col-10 { width: 10%; }" +
                   "th, td { border: 1px solid black; padding: 4px; text-align: left; }" +
                   ".bold { font-weight: bold; }" +
                   ".warning { font-weight: bold; color: red; }" +
                   ".success { font-weight: bold; color: green; }" +
                   "</style></head><body>" +
                   $"<p><span class='bold'>Hi {recipientName},</span></p>" +
                   "<p>Good day,</p>" +
                   $"<p>Here is the {emailType} result.</p>";
        }

        private string BuildEmailFooter( string returnUrl)
        {
            return $"<p>To view details please visit our website {returnUrl}</p>" +
                   //$"<p>To view details please visit our website {_config.Website}</p>" +
                   "<p class='warning'>This mail is auto-generated, please do not reply to this mail.</p>" +
                   "<p>Thank you</p>" +
                   "</body></html>";
        }

        private MailViewModel CreateMailViewModel(string to, string subject, string body)
        {
            return new MailViewModel()
            {
                Host = _config.SMTPHost,
                Port = Convert.ToInt32(_config.SMTPPort),
                IsSSLEnabled = false,
                From = _config.EmailSender,
                To = to,
                Subject = subject,
                Body = body
            };
        }
    }
}
