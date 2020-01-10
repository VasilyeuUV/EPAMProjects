using fwService;
using System.Configuration;
using System.Threading;

namespace epam_task4.WorkVersions
{
    internal static class ConsoleVersion
    {
        private static FileWatcherModel _fwLogger;

        internal static void StartFileWatcher()
        {
            var watchFolder = ConfigurationManager.AppSettings["defaultFolder"];

            _fwLogger = FileWatcherModel.CreateInstance(watchFolder);
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
            Process.StartProcessing(filePath);
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
