using efdb.DataContexts;
using efdb.DataModels;
using epam_task4.ConsoleMenu;
using epam_task4.Threads;
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
        static object locker = new object();

        [STAThread]
        static void Main(string[] args)
        {
            // INSTALL DATABASE
            if (!CheckDbAvailability())
            {
                DisplayMessage("DataBase not found. Continuation of work is not possible", ConsoleColor.Red);
                WaitForContinue();
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
            DisplayMessage($"CONSOLE WORK");

            using (OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Select files",
                    Multiselect = true,
                    Filter = "CSV files (*.csv)|*.csv",
                    RestoreDirectory = true
                })
            {
                try
                {
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // START PROCESSING!!!
                        lock (locker)
                        {
                            foreach (string filePath in openFileDialog.FileNames)
                            {
                                FileProcessingThread fileHandler = CreateFileHandlerThread(filePath);
                                StartProcessing(fileHandler, filePath);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    DisplayMessage(string.Format("Error opening file:\n" + ex.Message, ConsoleColor.Red));
                    return;
                }
            }
           
            while (_lstThread.Count > 0)
            {
            }
            WaitForContinue();
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
        /// Start processing
        /// </summary>
        /// <param name="fileHandler"></param>
        /// <param name="filePath"></param>
        private static void StartProcessing(FileProcessingThread fileHandler, string filePath)
        {
            try
            {
                if (fileHandler.Start(filePath))
                {
                    DisplayMessage($"{filePath}: Processing of file starting");
                    _lstThread.Add(fileHandler);
                    DisplayMessage($"Number of file handler threads - {_lstThread.Count}", ConsoleColor.Blue);
                }
                else
                {
                    DisplayMessage($"{filePath}: can't starting");
                }
            }
            catch (Exception)
            {
                DisplayMessage($"{filePath}: Error starting");
            }
        }

        /// <summary>
        /// Check database availability
        /// </summary>
        /// <returns></returns>
        private static bool CheckDbAvailability()
        {
            string clientName = "ClientDefault";
            string fileName = "fileNameDefault.csv";
            string managerName = "ManagerDefault";
            string productName = "ProductDefault";

            try
            {
                using (var repo = new Repository())
                {
                    Client client = repo.Select<Client>().FirstOrDefault(x => x.Name.Equals(clientName))
                                  ?? new Client() { Name = clientName };
                    FileName file = repo.Select<FileName>().FirstOrDefault(x => x.Name.Equals(fileName))
                                  ?? new FileName() { Name = fileName, DTG = DateTime.Now };
                    Manager manager = repo.Select<Manager>().FirstOrDefault(x => x.Name.Equals(managerName))
                                  ?? new Manager() { Name = managerName };
                    Product product = repo.Select<Product>().FirstOrDefault(x => x.Name.Equals(productName))
                                  ?? new Product() { Name = productName, Cost = 0 };
                    TmpSale sale = new TmpSale
                    {
                        Client = client,
                        FileName = file,
                        Manager = manager,
                        Product = product,
                        DTG = DateTime.Now,
                        Sum = 0
                    };

                    if (!repo.Insert(sale)) { return false; };
                    repo.Delete(client);
                    repo.Delete(file);
                    repo.Delete(manager);
                    repo.Delete(product);
                    repo.Dispose();
                }
            }
            catch (Exception) { return false; }
            return true;
        }



        #region FILE_PROCESSING_THREAD_EVENTS
        //##################################################################################################

        private static FileProcessingThread CreateFileHandlerThread(string filePath)
        {            
            FileProcessingThread fileHandler = new FileProcessingThread();

            fileHandler.WorkCompleted += FileHandler_WorkCompleted;
            fileHandler.FileContentErrorEvent += FileHandler_FileContentErrorEvent;
            fileHandler.FileNamingErrorEvent += FileHandler_FileNamingErrorEvent;
            fileHandler.SendMessageEvent += FileHandler_SendMessageEvent;

            return fileHandler;
        }


        private static void FileHandler_SendMessageEvent(object sender, string msg)
        {
            DisplayMessage($"Thread {(sender as FileProcessingThread)?.Name}: " + msg, ConsoleColor.DarkGray);
        }


        private static void FileHandler_WrongProductErrorEvent(object sender, EventArgs e)
        {
            DisplayMessage($"{(sender as FileProcessingThread)?.Name}: Error product data", ConsoleColor.Red);
        }

        private static void FileHandler_FileNamingErrorEvent(object sender, bool isSaved)
        {
            if (isSaved) { DisplayMessage($"File {sender.ToString()} was saved earlier", ConsoleColor.Yellow); }
            else { DisplayMessage($"{(sender as FileProcessingThread)?.Name}: Error file name", ConsoleColor.Red);   }            
        }

        private static void FileHandler_FileContentErrorEvent(object sender, EventArgs e)
        {
            DisplayMessage($"{(sender as FileProcessingThread)?.Name}: Error file content", ConsoleColor.Red);
        }

        private static void FileHandler_WorkCompleted(object sender, bool aborted)
        {
            if (aborted) { DisplayMessage($"Processing of file {(sender as FileProcessingThread)?.Name} aborted", ConsoleColor.Red); }
            else { DisplayMessage($"Processing of file {(sender as FileProcessingThread)?.Name} completed", ConsoleColor.Green); }



            var fileHandler = sender as FileProcessingThread;
            if (fileHandler != null)
            {
                lock (locker)
                {
                    fileHandler.WorkCompleted -= FileHandler_WorkCompleted;
                    fileHandler.FileContentErrorEvent -= FileHandler_FileContentErrorEvent;
                    fileHandler.FileNamingErrorEvent -= FileHandler_FileNamingErrorEvent;
                    fileHandler.SendMessageEvent -= FileHandler_SendMessageEvent;

                    _lstThread.Remove(fileHandler);
                    DisplayMessage($"Number of file handler threads - {_lstThread.Count}", ConsoleColor.Blue);
                }

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







        #region DISPLAY
        //##################################################################################################

        /// <summary>
        /// Wait push key 
        /// </summary>
        /// <param name="str"></param>
        private static ConsoleKeyInfo WaitForContinue(string str = "", ConsoleColor color = ConsoleColor.Green)
        {
            if (!String.IsNullOrEmpty(str.Trim()))
            {
                Console.ForegroundColor = color;
                Console.WriteLine(str);
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to continue");
            return Console.ReadKey();
        }

        private static void DisplayMessage(string str, ConsoleColor color = ConsoleColor.Green)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                lock (locker)
                {
                    Console.ForegroundColor = color;
                    Console.WriteLine(str);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }


        #endregion // DISPLAY


    }
}
