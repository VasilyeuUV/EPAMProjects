using epam_task4.Threads;
using System;
using System.Windows.Forms;

namespace epam_task4
{
    class Program
    {
        static void Main(string[] args)
        {
            EmulatorThread eThread = null;

            // DATABASE
            //InstallDataBase();

            // START EMULATOR
            Form emulator = emulatorWFA.FormMain.StartForm(true);
            if (emulator != null) { eThread = new EmulatorThread(emulator); }



            Console.WriteLine("Press any key to Exit");
            Console.ReadKey();
            eThread?.Close();
        }
    }
}
