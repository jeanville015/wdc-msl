
using System.Configuration;

namespace IDM.Web.Models
{
    public class ConfigViewModel
    {
        public string MQConnectionFile { get; set; }
        public string MQTransaction { get; set; }
        public string MQTransactionTrial { get; set; }
        public string MQVersion { get; set; }
        public string MQExcludeColumn { get; set; }
    }
}