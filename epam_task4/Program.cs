using efdb.DataContexts;
using efdb.DataModels;
using epam_task4.ConsoleMenu;
using epam_task4.Threads;
using epam_task4.WorkVersions;
using System;
using System.Collections.Generic;
using System.Linq;
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
            string[] items = { "Create file", "Use Console", "Use Windows Service", "Exit" };
            method[] methods = new method[] { RunOpenFileVertion, RunConsoleVertion, RunServiceVertion, Exit };
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
        /// Start OpenFileVertion
        /// </summary>
        private static void RunOpenFileVertion()
        {
            Console.Clear();
            Display.Message($"OPEN_FILE_VERTION WORK", ConsoleColor.Green);
            string[] files = OpenFileVersion.Run();

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
        /// Start Console Vertion
        /// </summary>
        private static void RunConsoleVertion()
        {
            Console.Clear();
            Display.Message($"CONSOLE_VERTION WORK", ConsoleColor.Green);

            ConsoleVersion.StartFileWatcher();

            // START EMULATOR
            EmulatorThread eThread = null;
            Form emulator = emulatorWFA.FormMain.StartForm(true);
            if (emulator != null) { eThread = new EmulatorThread(emulator); }

            Display.Message("Run the CSV file generator in Emulator");
            Console.WriteLine("");
            do
            {
                while (!Console.KeyAvailable)
                {
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            ConsoleVersion.StopFileWatcher();
            eThread?.Close();

            Display.WaitForContinue("File watcher is stopped", ConsoleColor.Green);            
        }
               

        /// <summary>
        /// Start as Service
        /// </summary>
        private static void RunServiceVertion()
        {
            ServiceVersion.StartService();

            // START EMULATOR
            EmulatorThread eThread = null;
            Form emulator = emulatorWFA.FormMain.StartForm(true);
            if (emulator != null) { eThread = new EmulatorThread(emulator); }

            Display.Message("Run the CSV file generator in Emulator");
            Console.WriteLine("");
            do
            {
                while (!Console.KeyAvailable)
                {
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            Console.Clear();
            eThread?.Close();
            ServiceVersion.Stop(true);
            
        }


        /// <summary>
        /// Exit
        /// </summary>
        private static void Exit()
        {
            //Console.WriteLine("Press any key to Exit");
            //Console.ReadKey();
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
        public static void StartProcessing(string file)
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
                Display.Message($"{(sender as FileProcessingThread)?.Name}: " + msg, ConsoleColor.Yellow);
            }
        }


        #endregion // FILE_PROCESSING_THREAD_EVENTS


     



    }
}
