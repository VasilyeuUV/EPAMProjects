﻿using System;
using System.Threading;
using System.Windows.Forms;

namespace epam_task4.Threads
{
    internal class EmulatorThread
    {
        private Thread _thread = null;
        private Form _form = null;

        public EmulatorThread(Form form)
        {
            this._form = form;
            this._thread = new Thread(this.StartEmulator);
            this._thread.SetApartmentState(ApartmentState.STA);
            this._thread.Start(form);
        }

        private void StartEmulator(object emulator)
        {
            Application.EnableVisualStyles();
            Application.Run((Form)emulator);
        }

        internal void Close()
        {
            if (this._thread.IsAlive)
            {
                Application.Exit();
            }
        }
    }
}
