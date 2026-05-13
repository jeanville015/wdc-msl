using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDM.Web.Models
{
    public class MailViewModel
    {
        public MailViewModel()
        {
            Attachments = new List<string>();
        }
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> Attachments { get; set; }
        public bool IsSSLEnabled { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }
    }
}