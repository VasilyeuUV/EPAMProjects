using System;
using System.ServiceProcess;
using System.Threading;

namespace fwService
{
    public partial class FWService : ServiceBase
    {
        private FWLogger _logger;
        private string _watchedFolder = @"D:\epam_temp";

        public event EventHandler<string> NewFileDetectedEvent;

        /// <summary>
        /// CTOR
        /// </summary>
        public FWService(string[] args)
        {
            InitializeComponent();

            if (args.Length > 0)
            {
                this._watchedFolder = args[0];
            }

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
            _watchedFolder = CheckFolder(_watchedFolder);

            if (!string.IsNullOrWhiteSpace(_watchedFolder))
            {                
                _logger = FWLogger.CreateInstance(_watchedFolder);
                _logger.NewFileDetectedEvent += _logger_NewFileDetectedEvent; ;
                _logger.RecordEntry("Service starting");
                Thread loggerThread = new Thread(new ThreadStart(_logger.Start));
                loggerThread.Start();
            }
        }

        /// <summary>
        /// Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="filePath"></param>
        private void _logger_NewFileDetectedEvent(object sender, string filePath)
        {
            NewFileDetectedEvent?.Invoke(this, filePath);
        }


        /// <summary>
        /// Checking for a directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string CheckFolder(string path)
        {
            System.IO.DirectoryInfo dirInfo;
            try
            {
                dirInfo = System.IO.Directory.CreateDirectory(path);
                return dirInfo.FullName;
            }
            catch (Exception)
            {
                return "";
            }            
        }

        /// <summary>
        /// Service stop
        /// </summary>
        protected override void OnStop()
        {
            _logger?.Stop();
            Thread.Sleep(1000);
            _logger.RecordEntry("Service stopped");
        }
    }
}
