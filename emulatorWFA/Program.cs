﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace emulatorWFA
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FormMain form = FormMain.StartForm(args);
            if (form != null)
            {
                Application.Run(form);
            }
            else
            {
                MessageBox.Show("This application cannot be started.");
            }
        }
    }
}
