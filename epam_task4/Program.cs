using efdb.DataContexts;
using efdb.DataModels;
using epam_task4.ConsoleMenu;
using epam_task4.Threads;
using epam_task4.WorkVersions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace epam_task4
{
    class Program
    {
        public delegate void method();

        private static List<FileProcessingThread> _lstThread = new List<FileProcessingThread>();
        private static object locker = new object();

        private static readonly string[] FILE_NAME_STRUCT = { "Manager", "DTG" };
        private static readonly string[] FILE_DATA_STRUCT = { "DTG", "Client", "Product", "Sum" };


        [STAThread]
        static void Main(string[] args)
        {
            // INSTALL DATABASE
            if (!CheckDbAvailability())
            {
                Display.WaitForContinue("DataBase not found. Continuation of work is not possible", ConsoleColor.Red);
                return;
            }


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
            Console.Clear();
            Display.Message($"CONSOLE WORK", ConsoleColor.Green);
            string[] files = ConsoleVertion.Run();

            if (files == null || files.Length < 1)
            {
                Display.WaitForContinue("Error opening one/several files", ConsoleColor.Red);
                return;
            }
            foreach (var file in files) { StartProcessing(file); }

            while (true) 
            {
                if (_lstThread.Count < 1)
                {
                    Thread.Sleep(500);
                    break;
                }                
            }
            Display.WaitForContinue();
        }
        


        /// <summary>
        /// Start as Service
        /// </summary>
        private static void UseService()
        {
            var watchFolder = ConfigurationManager.AppSettings["defaultFolder"];
            string servicePath = ConfigurationManager.AppSettings["servicePath"];
            //@"E:\_projects\epam\task4\epam_task4\fwService\bin\Debug\fwService.exe";
                       

            // INSTALL SERVICE            
            InstallService(servicePath);

            //START SERVICE
            StartService();

            // START EMULATOR
            EmulatorThread eThread = null;
            Form emulator = emulatorWFA.FormMain.StartForm(true);
            if (emulator != null) { eThread = new EmulatorThread(emulator); }

            Display.WaitForContinue(string.Format("Run the CSV file generator in Emulator"));
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
        /// Check database availability
        /// </summary>
        /// <returns></returns>
        private static bool CheckDbAvailability()
        {
            string managerName = "ManagerDefault";
            try
            {
                using (var repo = new Repository())
                {
                    Manager manager = repo.Select<Manager>()
                                          .FirstOrDefault(x => x.Name.Equals(managerName))
                                                             ?? new Manager() { Name = managerName };
                }
            }
            catch (Exception) { return false; }
            return true;
        }

        /// <summary>
        /// Run file handler
        /// </summary>
        /// <param name="file"></param>
        private static void StartProcessing(string file)
        {
            lock (locker)
            {
                FileProcessingThread fileHandler = CreateFileHandlerThread();
                try
                {
                    if (fileHandler.Start(file))
                    {
                        Display.Message($"{file}: Processing of file starting");
                        _lstThread.Add(fileHandler);
                        Display.Message($"Number of file handler threads - {_lstThread.Count}", ConsoleColor.Blue);
                    }
                    else
                    {
                        Display.Message($"{file}: can't starting");
                    }
                }
                catch (Exception)
                {
                    Display.Message($"{file}: Error starting");
                }
            }
        }

        private static FileProcessingThread CreateFileHandlerThread()
        {
            lock (locker)
            {
                FileProcessingThread fileHandler = new FileProcessingThread(fns: FILE_NAME_STRUCT, fds: FILE_DATA_STRUCT);

                fileHandler.WorkCompleted += FileHandler_WorkCompleted;
                fileHandler.ErrorEvent += FileHandler_ErrorEvent;
                //fileHandler.FileContentErrorEvent += FileHandler_FileContentErrorEvent;
                //fileHandler.FileNamingErrorEvent += FileHandler_FileNamingErrorEvent;
                //fileHandler.SendMessageEvent += FileHandler_SendMessageEvent;

                return fileHandler;
            }
        }





        #region FILE_PROCESSING_THREAD_EVENTS
        //##################################################################################################

        private static void FileHandler_WorkCompleted(object sender, bool aborted)
        {
            lock (locker)
            {
                if (aborted) { Display.Message($"{(sender as FileProcessingThread)?.Name}: Processing aborted", ConsoleColor.Red); }
                else { Display.Message($"{(sender as FileProcessingThread)?.Name}: Processing completed", ConsoleColor.Green); }

                var fileHandler = sender as FileProcessingThread;
                if (fileHandler != null)
                {
                    fileHandler.WorkCompleted -= FileHandler_WorkCompleted;
                    fileHandler.ErrorEvent -= FileHandler_ErrorEvent;

                    _lstThread.Remove(fileHandler);
                    Display.Message($"Number of file handler threads - {_lstThread.Count}", ConsoleColor.Blue);
                }
            }
        }


        private static void FileHandler_ErrorEvent(object sender, string msg)
        {
            lock (locker)
            {
                Display.Message($"{(sender as FileProcessingThread)?.Name}: " + msg, ConsoleColor.Gray);
            }
        }


        #endregion // FILE_PROCESSING_THREAD_EVENTS


     
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


        #endregion // FOR_WINDOWS_SERVICE


    }
}
