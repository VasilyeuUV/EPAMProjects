using fwService.Constants;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;

namespace fwService
{
    public partial class FWService : ServiceBase
    {
        private FileWatcherModel _fwModel;
        private string _watchedFolderDefault = ConfigurationManager.AppSettings["WatcherFolder"];
        private string _logFile = ConfigurationManager.AppSettings["LogFile"];

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


            //FWMessage.RecordEntry($"FWService: args.Length = {args.Length}");
            //if (args.Length > 0)
            //{
            //    for (int i = 0; i < args.Length; i++)
            //    {
            //        FWMessage.RecordEntry(i.ToString(), args[i]);
            //    }
            //}

        }


        /// <summary>
        /// Service start
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            string watchedFolder = _watchedFolderDefault;
            FWMessage.LogFile = _logFile;

            //FWMessage.RecordEntry($"FWService: Args.Length = {args.Length}");
            //if (args.Length > 0)
            //{
            //    for (int i = 0; i < args.Length; i++)
            //    {
            //        FWMessage.RecordEntry(i.ToString(), args[i]);
            //    }

            //    watchedFolder = CheckFolder(args[0]);
            //}
            if (string.IsNullOrWhiteSpace(watchedFolder))
            {
                watchedFolder = CheckFolder(_watchedFolderDefault);
            }
            FWMessage.RecordEntry($"Folder: {watchedFolder}");

            if (!string.IsNullOrWhiteSpace(watchedFolder))
            {                
                _fwModel = FileWatcherModel.CreateInstance(watchedFolder);
                _fwModel.NewFileDetectedEvent += _logger_NewFileDetectedEvent;
                FWMessage.RecordEntry($"Service starting. Watching folder: {watchedFolder}");
                Thread fwServiceThread = new Thread(new ThreadStart(_fwModel.Start));
                fwServiceThread.Start();
            }
            else
            {
                FWMessage.RecordEntry($"Service starting Error. Not watching folder.");
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
                return dirInfo?.FullName;
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
            FWMessage.RecordEntry($"Service stopped.");
        }

    }
}
