using epam_task4.Threads;
using System;
using System.Configuration.Install;
using System.Diagnostics;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace epam_task4
{
    class Program
    {
        static void Main(string[] args)
        {
            string servicePath = @"E:\_projects\epam\task4\epam_task4\fwService\bin\Debug\fwService.exe";


            // DATABASE
            //InstallDataBase();

            // INSTALL SERVICE            
            InstallService(servicePath);
            //ManagedInstallerClass.InstallHelper(new[] {"FWService.exe"});

            //START SERVICE
            StartService();




            //ServiceController svcController = new ServiceController("FWService");
            //try
            //{
            //    if (svcController.Status != ServiceControllerStatus.Running) 
            //    { 
            //        StartService(svcController); 
            //    }
            //}
            //catch (Exception)
            //{
            //}




            // START EMULATOR
            EmulatorThread eThread = null;
            Form emulator = emulatorWFA.FormMain.StartForm(true);
            if (emulator != null) { eThread = new EmulatorThread(emulator); }


            Console.WriteLine("");
            Console.WriteLine("Run the CSV file generator in Window Form Application (emulatorWFA)");
            Console.WriteLine("");
            Console.WriteLine("Press any key to Stop");
            Console.ReadKey();

            Console.Clear();
            eThread?.Close();
            UninstallService(servicePath);

            Console.WriteLine("Press any key to Exit");
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
        /// Start Service
        /// </summary>
        private static void StartService()
        {
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
        private static void StopService()
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
