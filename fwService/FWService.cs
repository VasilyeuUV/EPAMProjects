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
        private string _watchedFolderDefault = @"d:\epam_temp\";
        private string _logFile = @"d:\fwLogFile.txt";

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

            //SaveToAppConfig("WatcherFolder", @"d:\epam\");

        }


        /// <summary>
        /// Service start
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            string watchedFolder = CheckFolder(ConfigurationManager.AppSettings["WatcherFolder"] ?? _watchedFolderDefault);
            FWMessage.LogFile = ConfigurationManager.AppSettings["LogFile"] ?? _logFile;

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
            Transceiver.AddItem(filePath);
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


        private static void SaveToAppConfig(string key, string value)
        {
            Configuration configuration =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
