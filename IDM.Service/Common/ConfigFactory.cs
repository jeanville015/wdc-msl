using IDM.DTO;
using IDM.Service.Common.Interface;
using System;
using System.Configuration;
using System.DirectoryServices;
using System.Threading;
using System.Threading.Tasks;

namespace IDM.Service.Common.Service
{
    public static class ConfigFactory
    {
        public static ConfigDTO GetMqConfiguration()
        {
            return new ConfigDTO
            {
                MQConnectionFile = ConfigurationManager.AppSettings["MQConnectionFile"],
                MQTransaction = ConfigurationManager.AppSettings["MQTransaction"],
                MQTransactionTrial = ConfigurationManager.AppSettings["MQTransactionTrial"],
                MQVersion = ConfigurationManager.AppSettings["MQVersion"],
                MQExcludeColumn = ConfigurationManager.AppSettings["MQExcludeColumn"],
                MQAdjustColumn = ConfigurationManager.AppSettings["MQAdjustColumn"],
                SMTPHost = ConfigurationManager.AppSettings["SMTPHost"],
                SMTPPort = ConfigurationManager.AppSettings["SMTPPort"],
                EmailSender = ConfigurationManager.AppSettings["EmailSender"],
                DefaultEmailRecipients = ConfigurationManager.AppSettings["DefaultEmailRecipients"],
                Website = ConfigurationManager.AppSettings["Website"],
                BdpUploadCommonDirectory = ConfigurationManager.AppSettings["BdpUploadCommonDirectory"],
                BdpUploadCommonURL = ConfigurationManager.AppSettings["BdpUploadCommonURL"]
            };
        }
    }

}
