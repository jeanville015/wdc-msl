namespace IDM.DTO
{
    public class ConfigDTO
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
        public string BdpUploadCommonDirectory { get; set; }
        public string BdpUploadCommonURL { get; set; }
    }
}
