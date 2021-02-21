using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Logging
{
    public static class Log
    {
        // 1 = Log everything!
        // 2 = Log important and errors
        // 3 = Log errors only
        public static int logLevel = 2;
        private static object lockObj = new object();
        private static IReporter _reporter;

        private static string path = AppDomain.CurrentDomain.BaseDirectory;

        public static void EnableReporting(IReporter reporter)
        {
            _reporter = reporter;
        }

        public static void LogMessage(string message)
        {
            LogMessage(message, LogType.General);
        }

        public static void LogMessage(string message, LogType type)
        {
            if(type == LogType.Verbose && logLevel != 1) { return; }
            LogMessage(message, type, "");
        }

        public static void LogMessage(string message, LogType type, string additionalInfo)
        {
            if(type == LogType.Verbose && logLevel != 1) { return; }
            Write(message);
            if(type == LogType.Error)
            {
                if(_reporter != null)
                    _reporter.ReportError(message + " Additional: " + additionalInfo);
            }
        }

        private static void Write(string message)
        {
            lock (lockObj)
            {
                try {
                    string line = DateTime.Now + ": " + message;
                    Console.WriteLine(line);
                    if (!File.Exists(path + "/log.txt"))
                    {
                        File.Create(path + "/log.txt");
                        TextWriter tw = new StreamWriter(path + "/log.txt");
                        tw.WriteLine(line);
                        tw.Close();
                    }
                    else if (File.Exists(path + "/log.txt"))
                    {
                        using (var tw = new StreamWriter(path + "/log.txt", true))
                        {
                            tw.WriteLine(line);
                        }
                    }
                } catch (Exception) { /*Who cares*/ }
            }
        }
    }
}
