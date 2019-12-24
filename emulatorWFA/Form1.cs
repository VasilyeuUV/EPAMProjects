using System.Windows.Forms;

namespace emulatorWFA
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
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
    }
}
