using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace IDM.Web.DataAccess
{
    public class Message
    {
        public void Log(string message)
        {
            try
            {
                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                string appPath = HttpContext.Current.Server.MapPath($"~/Logs ");
                string logFile = appPath + @"\" + fileName;

                if (!Directory.Exists(appPath))
                {
                    Directory.CreateDirectory(appPath);
                }
                if (!File.Exists(logFile))
                {
                    File.Create(logFile).Close();
                }

                using (StreamWriter writer = File.AppendText(logFile))
                {
                    writer.WriteLine(DateTime.Now.ToString() + "-----" + message);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}