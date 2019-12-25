﻿using epam_task4.Threads;
using System;
using System.Threading;
using System.Windows.Forms;

namespace epam_task4
{
    class Program
    {
        static void Main(string[] args)
        {
            EmulatorThread eThread = null;


            //InstallDataBase();

            // START EMULATOR
            Form emulator = emulatorWFA.FormMain.StartForm(true);
            if (emulator != null)
            {
                eThread = new EmulatorThread(emulator);

                //Thread emulatorThread = new Thread(new ThreadStart(StartEmulator));
                //emulatorThread.Start();
            }



            Console.WriteLine("Press any key to Exit");
            Console.ReadKey();
            eThread?.Close();


            //myThread.Abort();
        }

        //private static void StartEmulator(Form emulator)
        //{
        //    Application.EnableVisualStyles();
        //    Application.Run(emulator);
        //}

    }
}
