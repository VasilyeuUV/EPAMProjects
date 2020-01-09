using fwService;
using System;
using System.Configuration;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Threading;

namespace epam_task4.WorkVersions
{
    internal static class ServiceVersion
    {
        private static string _servicePath = string.Empty;


        internal static void StartService()
        {
            var watchFolder = ConfigurationManager.AppSettings["defaultFolder"];
            _servicePath = ConfigurationManager.AppSettings["servicePath"];
            //@"E:\_projects\epam\task4\epam_task4\fwService\bin\Debug\fwService.exe";            


            // INSTALL SERVICE            
            InstallService(_servicePath);

            //START SERVICE
            using (ServiceController svcController = new ServiceController("FWService"))
            {
                try
                {
                    if (svcController.Status != ServiceControllerStatus.Running)
                    {
                        svcController.Start();
                        WaitStatus(svcController, ServiceControllerStatus.Running);
                    }
                    Console.WriteLine("Service is running.");
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to run the service. Run manually.");
                }
                svcController.Close();
            }
        }

        /// <summary>
        /// Stop Service
        /// </summary>
        /// <param name="uninstallService">uninstal service</param>
        internal static void Stop(bool uninstallService)
        {
            using (ServiceController svcController = new ServiceController("FWService"))
            {
                try
                {
                    if (svcController.Status != ServiceControllerStatus.Stopped)
                    {
                        svcController.Stop();
                        WaitStatus(svcController, ServiceControllerStatus.Stopped);
                    }
                    Console.WriteLine("Service is stopped.");
                }
                catch (Exception)
                {
                }
                svcController.Close();
            }
            if (uninstallService) { UninstallService(_servicePath); }
        }


        /// <summary>
        /// Install Service
        /// </summary>
        /// <param name="v"></param>        
        private static void InstallService(string servicePath)
        {
            //string servicePath = path + serviceFile; 
            if (System.IO.File.Exists(servicePath))
            {
                try
                {
                    ManagedInstallerClass.InstallHelper(new[] { servicePath });
                    Console.Clear();
                    Console.WriteLine("Service is installed.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to install the service. Set manually.");
                }
            }
        }

        /// <summary>
        /// Uninstall Service
        /// </summary>
        /// <param name="servicePath"></param>
        private static void UninstallService(string servicePath)
        {
            //string servicePath = path + serviceFile;
            if (System.IO.File.Exists(servicePath))
            {
                try
                {
                    ManagedInstallerClass.InstallHelper(new[] { "/u", servicePath });
                    Console.WriteLine("Service is uninstalled.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to uninstall the service. Uninstall manually.");
                }
            }
        }



        /// <summary>
        /// Wait for change service status
        /// </summary>
        /// <param name="svcController"></param>
        /// <param name="status"></param>
        private static void WaitStatus(ServiceController svcController, ServiceControllerStatus status)
        {
            while (svcController.Status != status)
            {
                Thread.Sleep(100);
                svcController.Refresh();
            }
        }
    }
}
