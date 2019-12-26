using emulatorWFA.Threads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace emulatorWFA
{
    public delegate void InstallPathsDlgt(string[] paths);


    public partial class FormMain : Form
    {
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
            var managers = lbManagers.Items.Cast<string>().ToList();
            managers.AddRange(lbNotManager.Items.Cast<string>().ToList());

            Dictionary<string, int> goods = GetProduct(lvProduct.Items);
            Dictionary<string, int> products = GetProduct(lvNotProducts.Items);
            foreach (var good in goods) { products.Add(good.Key, good.Value); }


            bool flag = false;            
            foreach (var manager in managers)
            {
                flag = !flag;
                Dictionary<string, int> usingProducts = flag ? goods : products;
                ManagerThread managerThread = new ManagerThread(manager, usingProducts, tbWatchedFolder.Text);
                lbManagerThreads.Items.Add(managerThread.Name);
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

