using System.Threading;
using System.Windows.Forms;

namespace epam_task4.Threads
{
    internal class EmulatorThread
    {
        Thread thread;

        public EmulatorThread(Form form)
        {
            thread = new Thread(this.StartEmulator);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(form);
        }

        private void StartEmulator(object emulator)
        {
            Application.EnableVisualStyles();
            Application.Run((Form)emulator);
        }

    }
}
