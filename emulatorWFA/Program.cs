using System;
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

            bool start = false;
            if (args.Length > 0)
            {
                start = Convert.ToBoolean(args[0]);
            }

            FormMain form = FormMain.StartForm(start);
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
