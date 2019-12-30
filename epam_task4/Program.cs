using epam_task4.ConsoleMenu;
using epam_task4.Threads;
using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;


namespace epam_task4
{
    class Program
    {
        delegate void method();

        [STAThread]
        static void Main(string[] args)
        {
            // MENU
            string[] items = { "Create file", "Use Windows Service", "Exit" };
            method[] methods = new method[] { UseConsole, UseService, Exit };
            Task4Menu menu = new Task4Menu(items);
            int menuResult;
            do
            {
                menuResult = menu.PrintMenu();
                Console.WriteLine();
                methods[menuResult]();
            } while (menuResult != items.Length - 1);
        }


        /// <summary>
        /// Start as Console
        /// </summary>
        private static void UseConsole()
        {
            Stream fileStream = null;
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    //openFileDialog.InitialDirectory = "D:\\";
                    openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // START PROCESS!!!
                        //fileStream = openFileDialog.OpenFile();     //Read the contents of the file into a stream
                    }
                    openFileDialog.Dispose();
                }
                WaitForContinue(string.Format("File processed successfully"));
            }
            catch (Exception e)
            {
                WaitForContinue(string.Format("Error opening file:\n" + e.Message));
                return;
            }
        }


        /// <summary>
        /// Start as Service
        /// </summary>
        private static void UseService()
        {

            string servicePath = @"E:\_projects\epam\task4\epam_task4\fwService\bin\Debug\fwService.exe";


            // DATABASE
            //InstallDataBase();

            // INSTALL SERVICE            
            InstallService(servicePath);

            //START SERVICE
            StartService();

            // START EMULATOR
            EmulatorThread eThread = null;
            Form emulator = emulatorWFA.FormMain.StartForm(true);
            if (emulator != null) { eThread = new EmulatorThread(emulator); }

            WaitForContinue(string.Format("Run the CSV file generator in Emulator"));
            Console.WriteLine("");
            Console.WriteLine("Press any key to stop the service.");
            Console.ReadKey();

            Console.Clear();
            eThread?.Close();
            UninstallService(servicePath);
        }



        /// <summary>
        /// Exit
        /// </summary>
        private static void Exit()
        {
            Console.WriteLine("Press any key to Exit");
            Console.ReadKey();
        }



        /// <summary>
        /// Wait push key 
        /// </summary>
        /// <param name="str"></param>
        private static ConsoleKeyInfo WaitForContinue(string str = "")
        {
            if (!String.IsNullOrEmpty(str.Trim()))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(str);
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to continue");
            return Console.ReadKey();
        }


        #region FOR_WINDOWS_SERVICE
        //##################################################################################################
        
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

    #endregion









}
