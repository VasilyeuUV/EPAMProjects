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
            //FWMessage.RecordEntry($"Main: args.Length = {args.Length}");
            //if (args.Length > 0)
            //{
            //    for (int i = 0; i < args.Length; i++)
            //    {
            //        FWMessage.RecordEntry(i.ToString(), args[i]);
            //    }
            //}

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new FWService(args)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
