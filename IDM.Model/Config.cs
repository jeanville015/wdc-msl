using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Model
{
    public class Config
    {
        public string MQConnectionFile { get; set; }
        public string MQTransaction { get; set; }
        public string MQTransactionTrial { get; set; }
        public string MQVersion { get; set; }
        public string MQExcludeColumn { get; set; }
        public string MQAdjustColumn { get; set; }

        public string SMTPHost { get; set; }
        public string SMTPPort { get; set; }
        public string EmailSender { get; set; }
        public string DefaultEmailRecipients { get; set; }
        public string Website { get; set; }
    }
}
