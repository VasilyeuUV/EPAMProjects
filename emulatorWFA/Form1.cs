using System;
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

        internal static FormMain StartForm(string[] args)
        {
            bool start = true;                      // to production must be false
            if (args.Length > 0)
            {
                start = Convert.ToBoolean(args[0]);
            }
            if (start)
            {
                return new FormMain();
            }
            return null;
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
                        string slash = tbWatchedFolder.Text[tbWatchedFolder.Text.Length - 1] == '\\'
                            ? ""
                            : @"\";                        
                        tbManagerErrorFolder.Text = tbWatchedFolder.Text + slash + @"FileNameErrors";
                        tbProductErrorFolder.Text = tbWatchedFolder.Text + slash + @"ProductErrors";
                        tbContentErrorFolder.Text = tbWatchedFolder.Text + slash + @"ContentErrors";
                        tbLogsFolder.Text = tbWatchedFolder.Text + slash + @"Logs";
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
        /// Install Paths
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPathsInstall_Click(object sender, System.EventArgs e)
        {
            string[] paths =
            {
                tbWatchedFolder.Text,
                tbManagerErrorFolder.Text,
                tbProductErrorFolder.Text,
                tbContentErrorFolder.Text,
                tbLogsFolder.Text
            };
            //InstallPathsDlgt(paths);
        }


    }
}
