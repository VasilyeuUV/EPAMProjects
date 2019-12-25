using System;
using System.ServiceProcess;
using System.Threading;

namespace fwService
{
    public partial class FWService : ServiceBase
    {

        private FWLogger _logger;
        private string _watchedFolder = @"D:\epam_temp";

        /// <summary>
        /// CTOR
        /// </summary>
        public FWService()
        {
            InitializeComponent();

            this.CanStop = true;                    // service can be stopped
            this.CanPauseAndContinue = true;        // service can be paused and then continued
            this.AutoLog = true;                    // service can record to the log            
        }


        /// <summary>
        /// Service start
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            _watchedFolder = CheckFolder(_watchedFolder).FullName;

            _logger = FWLogger.CreateInstance(_watchedFolder);
            Thread loggerThread = new Thread(new ThreadStart(_logger.Start));
            loggerThread.Start();


            //if (_logger == null) { _logger = FWLogger.CreateInstance(_watchedFolder); }
            //if (_logger == null)
            //{
            //    _logger.RecordEntry("Service Startup Error. Wathcher do not exist");
            //}
            //else
            //{
            //    Thread loggerThread = new Thread(new ThreadStart(_logger.Start));
            //    try
            //    {
            //        loggerThread.Start();
            //        _logger.RecordEntry("Whatcher started");
            //    }
            //    catch (System.Exception)
            //    {
            //        _logger.RecordEntry("Service Startup Error");
            //    }
            //}
        }

        private System.IO.DirectoryInfo CheckFolder(string path)
        {
            return System.IO.Directory.CreateDirectory(path);
        }


        /// <summary>
        /// Service stop
        /// </summary>
        protected override void OnStop()
        {
            _logger?.Stop();
            Thread.Sleep(1000);
            _logger.RecordEntry("Watcher stopped");
        }
    }
}
