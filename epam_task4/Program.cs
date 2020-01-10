using efdb.DataContexts;
using efdb.DataModels;
using epam_task4.ConsoleMenu;
using epam_task4.Threads;
using epam_task4.WorkVersions;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace epam_task4
{
    class Program
    {
        public delegate void method();
        private static object locker = new object();

        [STAThread]
        static void Main(string[] args)
        {
            // INSTALL DATABASE
            Display.Message("Check Database");
            if (!CheckDbAvailability())
            {
                Display.WaitForContinue("DataBase not found. Continuation of work is not possible", ConsoleColor.Red);
                return;
            }

            // MENU
            string[] items = { "Create file", "Use Console", "Use Windows Service", "View Data", "Exit" };
            method[] methods = new method[] { RunOpenFileVertion, RunConsoleVertion, RunServiceVertion, DisplayData, Exit };
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
        /// Display data
        /// </summary>
        private static void DisplayData()
        {
            Console.Clear();
            int viewPositions = 1000;

            Display.DisplayData<Manager>(viewPositions);
            Display.DisplayData<FileName>(viewPositions);
            Display.DisplayData<Client>(viewPositions);
            Display.DisplayData<Product>(viewPositions);

            Display.WaitForContinue();
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
            foreach (var file in files) { Process.StartProcessing(file); Thread.Sleep(150); }

            do
            {
            } while (Process.lstThread.Count() > 0);
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


            Transceiver.PropertyChanged += Transceiver_PropertyChanged;


            Display.Message("Run the CSV file generator in Emulator");
            Console.WriteLine("");
            do
            {
                while (!Console.KeyAvailable)
                {
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            Console.Clear();
            //Transceiver.PropertyChanged -= Transceiver_PropertyChanged;
            eThread?.Close();
            ServiceVersion.Stop(true);
            
        }

        private static void Transceiver_PropertyChanged(object sender, string fileName)
        {
            lock(locker)
            {
                Process.StartProcessing(fileName);
                Transceiver.RemoveItem(fileName);
            }
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

    }
}
