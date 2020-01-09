using emulatorWFA.Threads;
using fwService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace emulatorWFA
{
    public delegate void InstallPathsDlgt(string[] paths);


    public partial class FormMain : Form
    {
        private List<ManagerJobThread> _threadPool = null;
        private object obj = new object();          // some object for lock


        /// <summary>
        /// CTOR
        /// </summary>
        private FormMain()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Create Form
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static FormMain StartForm(bool go = false)
        {
            bool start = go;                      // to production must be false
            if (start)
            {
                return new FormMain();
            }
            return null;
        }


        /// <summary>
        /// Run the process simulator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartProcess_Click(object sender, EventArgs e)
        {
            btnStartProcess.Enabled = false;
            tbLog.Text = "";

            var managers = lbManagers.Items.Cast<string>().ToList();
            managers.AddRange(lbNotManager.Items.Cast<string>().ToList());

            Dictionary<string, int> goods = GetProduct(lvProduct.Items);
            Dictionary<string, int> products = GetProduct(lvNotProducts.Items);
            foreach (var good in goods) { products.Add(good.Key, good.Value); }

            this._threadPool = new List<ManagerJobThread>();

            bool flag = false;
            foreach (var manager in managers)
            {
                flag = !flag;
                Dictionary<string, int> usingProducts = flag ? goods : products;

                ManagerJobThread managerJobThread = new ManagerJobThread(manager, tbWatchedFolder.Text, dtpStartData.Value.Date);
                managerJobThread.FileSended += ManagerJobThread_FileSended;
                managerJobThread.ThreadCompleted += ManagerJobThread_ThreadCompleted;
                managerJobThread.Start(usingProducts);

                lbManagerThreads.Items.Add(managerJobThread.Name);
                this._threadPool.Add(managerJobThread);
                tbLog.Text += $"{managerJobThread.Name} started work" + Environment.NewLine;
            }
        }

        /// <summary>
        /// Manager file send event sender
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fileName"></param>
        private void ManagerJobThread_FileSended(object sender, string fileName)
        {
            Action action = () =>
            {
                lock (obj)
                {
                    tbLog.Text += $"{(sender as ManagerJobThread)?.Name} send {fileName}" + Environment.NewLine;
                }
            };
            Invoke(action);
        }

        /// <summary>
        /// Manager job completed event sender
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="canceled"></param>
        private void ManagerJobThread_ThreadCompleted(object sender, bool canceled)
        {
            ManagerJobThread thread = sender as ManagerJobThread;
            if (thread == null) { return; }

            Action action = () =>
            {
                lock (obj)
                {
                    string log = !canceled ? $"{thread.Name} work completed"
                                          : $"{thread.Name} work breaked";
                    tbLog.Text += log + Environment.NewLine;  
                    
                    lbManagerThreads.Items.Remove(thread.Name);
                    _threadPool.Remove(thread);
                    if (_threadPool?.Count() == 0)
                    {
                        btnStartProcess.Enabled = true;
                    }
                }
            };
            Invoke(action);         
        }



        /// <summary>
        /// Stop threads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopProcess_Click(object sender, EventArgs e)
        {
            foreach (var thread in this._threadPool)
            {
                //lbManagerThreads.Items.Remove(thread.Name);
                thread.Close();
            }
        }
            
       
        private Dictionary<string, int> GetProduct(ListView.ListViewItemCollection products)
        {
            return products.Cast<ListViewItem>()
                           .ToDictionary(
                                item => item.Text,
                                item => Convert.ToInt32(item.SubItems[1].Text)
                           );
        }



        /// <summary>
        /// Select Folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWatchedFolderSelect_Click(object sender, System.EventArgs e)
        {
            FolderBrowserDialog DirDialog = new FolderBrowserDialog();
            DirDialog.Description = "Выбор директории";
            DirDialog.SelectedPath = @"C:\";

            if (DirDialog.ShowDialog() == DialogResult.OK)
            {
                switch( (sender as Button)?.Name)
                {
                    case "btnWatchedFolderSelect":
                        tbWatchedFolder.Text = DirDialog.SelectedPath;
                        if (tbWatchedFolder.Text[tbWatchedFolder.Text.Length - 1] != '\\')
                        {
                            tbWatchedFolder.Text += @"\";
                        }                       
                        tbManagerErrorFolder.Text = $"{tbWatchedFolder.Text}FileNameErrors\\";
                        tbProductErrorFolder.Text = $"{tbWatchedFolder.Text}ProductErrors\\";
                        tbContentErrorFolder.Text = $"{tbWatchedFolder.Text}ContentErrors\\";
                        tbLogsFolder.Text = $"{tbWatchedFolder.Text}Logs\\";
                        break;
                    case "btnManagerErrorFolder":
                        tbManagerErrorFolder.Text = DirDialog.SelectedPath;
                        break;
                    case "btnProductErrorFolder":
                        tbProductErrorFolder.Text = DirDialog.SelectedPath;
                        break;
                    case "btnContentErrorFolder":
                        tbContentErrorFolder.Text = DirDialog.SelectedPath;
                        break;
                    case "btnLogsFolderSelect":
                        tbLogsFolder.Text = DirDialog.SelectedPath;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Install process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAllInstall_Click(object sender, System.EventArgs e)
        {

        }


        /// <summary>
        /// Install Paths event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPathsInstall_Click(object sender, System.EventArgs e)
        {
            InstallPaths();
            MessageBox.Show("Folders installation completed");            
        }

        /// <summary>
        /// Install Paths
        /// </summary>
        private void InstallPaths()
        {
            string[] paths =
            {
                tbWatchedFolder.Text,
                tbManagerErrorFolder.Text,
                tbProductErrorFolder.Text,
                tbContentErrorFolder.Text,
                tbLogsFolder.Text
            };
            for (int i = 0; i < paths.Length; i++)
            {
                try
                {
                    if (Directory.Exists(paths[i])) { continue; }
                    Directory.CreateDirectory(paths[i]);
                }
                catch (Exception)
                {
                    continue;
                }
            }



            // Set the paths to dispatcher and reinstall
        }


    }
}

