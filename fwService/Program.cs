using System.Configuration.Install;
using System.ServiceProcess;

namespace fwService
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main(/*string[] args*/)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new FWService(/*args*/)
            };
            ServiceBase.Run(ServicesToRun);



            //if (Environment.UserInteractive == false)
            //{
            //    Console.WriteLine("Run service");

            //    Console.ReadKey();
            //}
            //else
            //{
            //    if (uac(args[0])) {
            //        ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
            //    }                
            //}
        }

        //static bool uac(string data)
        //{
        //    WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
        //    bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
        //    if (hasAdministrativeRight == false)
        //    {
        //        Console.WriteLine("Haven't Administrative right");
        //        //ProcessStartInfo processInfo = new ProcessStartInfo();
        //        //processInfo.Verb = "runas";
        //        //processInfo.FileName = Application.ExecutablePath;
        //        //try
        //        //{
        //        //    processInfo.Arguments = data;
        //        //    Process.Start(processInfo);
        //        //}
        //        //catch ()
        //        //{

        //        //}
        //        //Application.Exit();
        //        return false;
        //    }
        //    else { return true; }
        //}



    }
}
