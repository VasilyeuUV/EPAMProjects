using emulatorWFA.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace emulatorWFA.Threads
{
    internal class ManagerJobThread
    {
        private Thread _thread = null;
        private string _folder = "";
        private bool _canceled = false;

        internal string Name 
        {
            get
            { 
                return this._thread.Name;
            }
            private set
            {
                try
                {
                    this._thread.Name = value;
                }
                catch (Exception)
                {
                }                
            }
        }

        internal event EventHandler<bool> ThreadCompleted;
        internal event EventHandler<string> FileSended;
        //internal class EventFileSendedEventArgs : EventArgs
        //{
        //    public string FileName { get; private set; }
        //    public EventFileSendedEventArgs(string fileName)
        //    {
        //        FileName = fileName;
        //    }
        //}

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="name"></param>
        /// <param name="products"></param>
        /// <param name="folder"></param>
        internal ManagerJobThread(string name, string folder)
        {
            this._folder = folder;           

            this._thread = new Thread(this.RunProcess);
            this.Name = name;
            this._thread.IsBackground = true;
        }

        /// <summary>
        /// Close this thread
        /// </summary>
        internal void Close()
        {
            this._canceled = true;
        }

        /// <summary>
        /// Start Thread
        /// </summary>
        /// <param name="products"></param>
        internal void Start(Dictionary<string, int> products)
        {
            this._thread.Start(products);
        }


        /// <summary>
        /// Manager job process
        /// </summary>
        /// <param name="obj">product list</param>
        private void RunProcess(object obj)
        {
            Dictionary<string, int> products = (Dictionary<string, int>)obj;
            int day = 20;
            DateTime startData = new DateTime(2019, 12, day, 4, 0, 0);

            while (startData <= DateTime.Now.AddDays(-1) && !this._canceled)
            {
                int reportCount = Const.RND.Next(1, 5);
                for (int i = 0; i < reportCount; i++)
                {
                    startData = startData.AddHours(4);
                    int salesCount = Const.RND.Next(5, 30);
                    string sales = GetSales(startData, salesCount, products);
                    SendReport(startData, sales, i + 1);
                }
                day = ++day > 28 ? 1 : day;
                startData = new DateTime(2019, 12, day, 4, 0, 0);
            }
            ThreadCompleted(this, this._canceled);
        }

        /// <summary>
        /// Manager Report
        /// </summary>
        /// <param name="startData"></param>
        /// <param name="sales"></param>
        private void SendReport(DateTime startData, string sales, int number)
        {
            string reportFileName = $"{this._thread.Name}_{startData.ToString("dd.MM.yyyy")}_{number}.csv";
            string reportFile = _folder + reportFileName;

            using (StreamWriter writer = new StreamWriter(reportFile, true))
            {
                writer.Write(sales);
                writer.Flush();
                writer.Dispose();
            }

            FileSended(this, reportFileName);

            //OnFileSended(this._thread.Name, reportFileName);
            //OnSendFileEvent?.Invoke(reportFile);
            //SendFile.Invoke
        }


        /// <summary>
        /// Sales process
        /// </summary>
        /// <param name="startData">Date of sale</param>
        /// <param name="salesCount">quantity of goods sold in one report</param>
        /// <param name="products">product list</param>
        /// <returns></returns>
        private string GetSales(DateTime startData, int salesCount, Dictionary<string, int> products)
        {
            string delimiter = ",";
            int clientCount = 500;
            StringBuilder soldGoods = new StringBuilder();
            for (int i = 0; i < salesCount; i++)
            {
                startData = startData.AddMinutes(Const.RND.Next(1, 40));
                soldGoods.Append(startData.ToString("dd.MM.yyyy HH:mm:ss") + delimiter);
                soldGoods.Append("Client" + Const.RND.Next(1, clientCount) + delimiter);
                var product = products.ElementAt(Const.RND.Next(0, products.Count));
                soldGoods.Append(product.Key + delimiter);

                // the probability of an error recording the amount
                var dice1 = Const.RND.Next(0, 100);
                var dice2 = Const.RND2.Next(0, 100);
                if (dice1 == dice2) { soldGoods.Append("wrong price"); }
                else { soldGoods.Append(product.Value); }
                soldGoods.Append(string.Format("\r\n"));

                int sleep = Const.RND.Next(200, 700);
                Thread.Sleep(sleep);
            }
            return soldGoods.ToString();
        }
    }
}
