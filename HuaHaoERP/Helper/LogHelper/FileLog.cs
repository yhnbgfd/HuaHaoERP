﻿using System;
using System.IO;

namespace HuaHaoERP.Helper.LogHelper
{
    static class FileLog
    {
        private static string path = AppDomain.CurrentDomain.BaseDirectory + "Logs\\";

        internal static void ErrorLog(string log)
        {
            LogToFile(log, "Error.log");
        }
        internal static void WarnLog(string log)
        {
            LogToFile(log, "Warn.log");
        }
        internal static void Log(string log)
        {
            LogToFile(log, "Log.log");
        }

        private static void LogToFile(string log, string file)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            FileStream fs = new FileStream(path + file, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            log = DateTime.Now + " \t| " + log + "\n";
            sw.Write(log);
            sw.Flush();//清空缓冲区  
            sw.Close();//关闭流  
            fs.Close();
        }
    }
}
