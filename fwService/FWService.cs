using System;
using System.ServiceProcess;
using System.Threading;

namespace fwService
{
    public partial class FWService : ServiceBase
    {
        private FileWatcherModel _fwModel;
        private string _watchedFolder = @"D:\epam_temp";

        public event EventHandler<string> SendMessage;
        public event EventHandler<string> NewFileDetectedEvent;

        /// <summary>
        /// CTOR
        /// </summary>
        public FWService(string[] args)
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
            _watchedFolder = CheckFolder(_watchedFolder);

            if (!string.IsNullOrWhiteSpace(_watchedFolder))
            {                
                _fwModel = FileWatcherModel.CreateInstance(_watchedFolder);
                _fwModel.NewFileDetectedEvent += _logger_NewFileDetectedEvent; ;
                SendMessage?.Invoke(this, "Service starting");
                Thread loggerThread = new Thread(new ThreadStart(_fwModel.Start));
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
            _fwModel?.Stop();
            Thread.Sleep(1000);
            SendMessage?.Invoke(this, "Service stopped");
        }

    }
}
