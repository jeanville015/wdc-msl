using System.Collections.Generic;

namespace IDM.DTO
{
    public class MailViewModel
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool IsSSLEnabled { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Attachments { get; set; } = new List<string>();
    }
}
