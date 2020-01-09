using fwService.Constants;
using System.Configuration.Install;
using System.ServiceProcess;

namespace fwService
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new FWService(args)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
