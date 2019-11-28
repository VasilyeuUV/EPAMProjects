using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Task2.Models;
using Task2.Tools;

namespace Task2.Menu
{
    internal static class MenuManager
    {       

        internal delegate void method();
        
        private static string _originalFilePath = string.Empty;

        internal static void DisplayMainMenu()
        {
            string operation = "ИСХОДНЫЕ ДАННЫЕ:";
            string[] items = { "Выбрать файл для обработки", "Выход" };
            method[] methods = new MenuManager.method[] { FileDialog, Exit };
            SelectMenuItem(operation, items, methods);
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
                ToDisplay.WaitForContinue(string.Format("Ошибка открытия файла:\n" + e.Message));
                return;
            }

            TextModel textModel = CreateTextModel(fileStream);

            if (TextHandler.IsEmptyTextModel(textModel))
            {
                ToDisplay.WaitForContinue("При обработке текста возникли ошибки. Текст не обработан.");
                return;
            }
            ToDisplay.WaitForContinue("Текс обработан успешно.");

            ConcordanceManager.DisplayOperationMenuAsync(textModel);
        }

        /// <summary>
        /// Create TextModel new instance
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        private static TextModel CreateTextModel(Stream fileStream)
        {
            if (fileStream == null) { return null; }

            FileStream fs = fileStream as FileStream;
            _originalFilePath = fs == null ? string.Empty : fs.Name;  //Get the path of opened file
            string fileContent = FileReadWriter.ReadFile(fileStream);

            fileContent = TextHandler.OptimizeText(fileContent);
            return TextModel.NewInstance(fileContent);
        }


        /// <summary>
        /// Back to the up menu
        /// </summary>
        private static void Exit()
        {
            Console.WriteLine("Работа завершена.");
        }


        /// <summary>
        /// Select menu item
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="items"></param>
        /// <param name="methods"></param>
        internal static void SelectMenuItem(string operation, string[] items, method[] methods)
        {
            ConsoleMenu menu = new ConsoleMenu(items);
            int menuResult;
            do
            {
                menuResult = menu.PrintMenu(operation);
                Console.WriteLine();
                methods[menuResult]();
            } while (menuResult != items.Length - 1);
        }
    }
}
