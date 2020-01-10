using System;
using System.Configuration;
using System.IO;

namespace fwService.Constants
{
    internal static class FWMessage
    {
        private static object obj = new object();          // some object for lock
        internal static string LogFile { get; set; } = ConfigurationManager.AppSettings["LogFile"] ?? @"d:\fwLogFile.txt";


        /// <summary>
        /// Save event result
        /// </summary>
        /// <param name="fileEvent"></param>
        /// <param name="filePath"></param>
        internal static void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(LogFile, true))
                {
                    writer.WriteLine(string.Format("{0}: File {1} was {2}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// Save event result
        /// </summary>
        /// <param name="logEvent"></param>
        internal static void RecordEntry(string logEvent)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(LogFile, true))
                {
                    writer.WriteLine(string.Format("{0}: {1}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), logEvent));
                    writer.Flush();
                }
            }
        }
    }
}
