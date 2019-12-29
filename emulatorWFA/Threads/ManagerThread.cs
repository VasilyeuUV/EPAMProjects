using emulatorWFA.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace emulatorWFA.Threads
{
    internal class ManagerThread
    {
        //internal delegate void SendFileHandler(string file);
        //internal event SendFileHandler OnSendFileEvent;

        public event EventHandler<EventFileSendedEventArgs> FileSended;

        public class EventFileSendedEventArgs : EventArgs
        {
            public string Manager { get; private set; }
            public string FileName { get; private set; }
            public EventFileSendedEventArgs(string manager, string fileName)
            {
                Manager = manager;
                FileName = fileName;
            }
        }

        void OnFileSended(string manager, string fileName)
        {
            FileSended?.Invoke(this, new EventFileSendedEventArgs(manager, fileName));
        }




        private Thread _thread = null;
        private string _folder = "";        

        internal string Name { get; private set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public ManagerThread(string name, Dictionary<string, int> products, string folder)
        {
            this._folder = folder;
            this.Name = name;

            this._thread = new Thread(this.RunProcess);
            this._thread.Name = name;            
            this._thread.IsBackground = true;
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
            
            while (startData <= DateTime.Now.AddDays(-1))
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
            }

            OnFileSended(this._thread.Name, reportFileName);
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

                int sleep = Const.RND.Next(500, 1000);
                Thread.Sleep(sleep);
            }
            return soldGoods.ToString();
        }
    }
}
