using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTO.Framework.Shared.Configuration;
using System.IO;

namespace SynMusicRightsValidation.Entities
{
    public static class Lib
    {
        public static void LogService(string content)
        {
            ApplicationConfiguration applicationConfiguration = new ApplicationConfiguration();
            string rootLogFolderPath = applicationConfiguration.GetConfigurationValue("RootLogFolderPath");
            string rootAppName = applicationConfiguration.GetConfigurationValue("RootAppName");
            string appName = applicationConfiguration.GetConfigurationValue("AppName");

            string FileName = rootLogFolderPath + rootAppName + "\\" + appName + "\\" + appName + "_" + DateTime.Now.Date.ToString("dd-MMM-yyyy") + "_" + "Log.txt";

            FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine(content);
            sw.Flush();
            sw.Close();
        }
    }
}
