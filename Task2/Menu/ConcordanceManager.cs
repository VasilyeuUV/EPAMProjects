using System;
using System.IO;
using System.Text;
using System.Linq;

namespace Task2.Menu
{
    public static class ConcordanceManager
    {
        /// <summary>
        /// Work with concordance
        /// </summary>
        internal static void ConcordanceDoWork()
        {
            DisplayCreateMenu();
        }

        /// <summary>
        /// Return to the menu one level higher
        /// </summary>
        private static void Back()
        {
        }




        #region CONCORDANCE_FIRST_MENU
        //###############################################################################################################################

        /// <summary>
        /// Concordance create menu
        /// </summary>
        private static void DisplayCreateMenu()
        {
            string operation = "СФОРМИРОВАТЬ КОНКОРДАНС";
            string[] items = { "Из файла", "Назад" };
            MenuManager.method[] methods = new MenuManager.method[] { FileDialog, Back };
            MenuManager.SelectMenuItem(operation, items, methods);
        }


        /// <summary>
        /// Create concordsnce from file
        /// </summary>
        private static void FileDialog()
        {
            Stream fileStream = null;
            try
            {
                using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    //openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "txt files (*.txt)|*.txt"; //|All files (*.*)|*.*
                    //openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        fileStream = openFileDialog.OpenFile();     //Read the contents of the file into a stream
                    }
                }
            }
            catch (Exception e)
            {
               // _concordance = null;
            }

           // _concordance = fileStream == null ? null : ConcordanceModel.CreateInstance(fileStream);

        }


        #endregion // CONCORDANCE_FIRST_MENU



        #region CONCORDANCE_SECOND_MENU
        //###############################################################################################################################

        /// <summary>
        /// Concordance view menu
        /// </summary>
        private static void DisplayOperationMenu()
        {
            string operation = "РАБОТА С КОНКОРДАНСОМ";
            string[] items = { "Показать Конкорданс", "Назад" };
            MenuManager.method[] methods = new MenuManager.method[] { ViewConcordance, Back };
            MenuManager.SelectMenuItem(operation, items, methods);
        }

        private static void ViewConcordance()
        {
            Console.WriteLine("Показ конкорданса:");
        }


        #endregion
    }
}
