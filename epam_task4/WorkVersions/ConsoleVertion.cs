using fwService;
using System;
using System.Configuration;
using System.Threading;

namespace epam_task4.WorkVersions
{
    internal static class ConsoleVertion
    {
        private static FWLogger _fwLogger;

        internal static void StartFileWatcher()
        {
            var watchFolder = ConfigurationManager.AppSettings["defaultFolder"];

            _fwLogger = FWLogger.CreateInstance(watchFolder);
            _fwLogger.NewFileDetectedEvent += FwLogger_NewFileDetectedEvent; ;
            Thread fwThread = new Thread(new ThreadStart(_fwLogger.Start));
            fwThread.Start();
        }

        /// <summary>
        /// New file detected event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="filePath"></param>
        private static void FwLogger_NewFileDetectedEvent(object sender, string filePath)
        {
            Program.StartProcessing(filePath);
        }


        /// <summary>
        /// Stop file watcher
        /// </summary>
        internal static void StopFileWatcher()
        {
            _fwLogger.NewFileDetectedEvent -= FwLogger_NewFileDetectedEvent;
            _fwLogger.Stop();
        }
    }
}
