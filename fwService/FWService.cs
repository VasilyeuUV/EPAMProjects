using System.ServiceProcess;
using System.Threading;

namespace fwService
{
    public partial class FWService : ServiceBase
    {

        FWLogger logger = null;
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
            if (logger == null)
            {
                logger = FWLogger.CreateInstance(_watchedFolder);
            }
            Thread loggerThread = new Thread(new ThreadStart(logger.Start));
            loggerThread.Start();
        }


        /// <summary>
        /// Service stop
        /// </summary>
        protected override void OnStop()
        {
            logger?.Stop();
            Thread.Sleep(1000);
            logger = null;            
        }
    }
}
