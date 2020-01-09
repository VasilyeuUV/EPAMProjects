using System;
using System.Windows.Forms;

namespace epam_task4.WorkVersions
{
    internal static class OpenFileVersion
    {
        /// <summary>
        /// Run console vertion
        /// </summary>
        /// <param name="fns">file name structure</param>
        /// <param name="fds">file data structure</param>
        internal static string[] Run()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select files",
                Multiselect = true,
                Filter = "CSV files (*.csv)|*.csv",
                RestoreDirectory = true
            })
            {
                try
                {
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        return openFileDialog.FileNames;
                    }
                    else { return null; }
                }
                catch (Exception)
                {                    
                    return null;
                }
            }
        }
    }
}
