using System;
using System.Windows.Forms;

namespace epam_task4
{
    class Program
    {
        static void Main(string[] args)
        {
            //InstallDataBase();

            // START EMULATOR
            Form emulator = GetEmulator();
            if (emulator != null)
            {
                Application.EnableVisualStyles();
                Application.Run(emulator);
            }



            Console.WriteLine("Press any key to Exit");
            Console.ReadKey();
        }


        /// <summary>
        /// Run emulator
        /// </summary>
        /// <returns></returns>
        private static Form GetEmulator()
        {
            return emulatorWFA.FormMain.StartForm(true);
        }
    }
}
