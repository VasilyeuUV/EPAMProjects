using epam_task4.Threads;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace epam_task4
{
    class Program
    {
        static void Main(string[] args)
        {


            // DATABASE
            //InstallDataBase();

            //START SERVICE
            ServiceController svcController = new ServiceController("FWService");
            try
            {
                if (svcController.Status != ServiceControllerStatus.Running) 
                { 
                    StartService(svcController); 
                }
            }
            catch (Exception)
            {
            }


            

            // START EMULATOR
            EmulatorThread eThread = null;
            Form emulator = emulatorWFA.FormMain.StartForm(true);
            if (emulator != null) { eThread = new EmulatorThread(emulator); }



            Console.WriteLine("Press any key to Exit");
            Console.ReadKey();

            eThread?.Close();
            svcController.Stop();
        }

        private static void StartService(ServiceController svcController)
        {
            svcController.Start();
            while (svcController.Status != ServiceControllerStatus.Running)
            {
                Thread.Sleep(100);
                svcController.Refresh();
            }
        }
    }
}