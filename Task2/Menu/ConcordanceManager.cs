using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Task2.Models;
using Task2.Tools;

namespace Task2.Menu
{
    public static class ConcordanceManager
    {
        private static string _originalFilePath = string.Empty;
        private static TextModel _textModel = null;
        private static IEnumerable<string> _tmWords = null;


        /// <summary>
        /// Work Process
        /// </summary>
        /// <param name="fileContent"></param>
        private static async void DoWorkAsync(string fileContent)
        {
            if (string.IsNullOrWhiteSpace(fileContent))
            {
                MenuManager.WaitForContinue("Текст не получен.");
                return;
            }

            fileContent = TextHandler.OptimizeText(fileContent);
            _textModel = TextModel.NewInstance(fileContent);

            _tmWords = await GetTMWords(_textModel);
            //var pages = TextLayoutModel.NewInstance(_textModel.Content);  // работает

            if (IsTMEmpty())
            {
                MenuManager.WaitForContinue("При обработке текста возникли ошибки. Текст не обработан.");
                return;
            }

            MenuManager.WaitForContinue("Текст обработан успешно");
            DisplayOperationMenu();
        }

        /// <summary>
        /// Check TextModel object for null or empty
        /// </summary>
        /// <returns></returns>
        private static bool IsTMEmpty()
        {
            return _textModel == null || _textModel.Paragraphs == null
                                               || _textModel.Paragraphs.Count() < 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textModel"></param>
        /// <returns></returns>
        private static Task<IEnumerable<string>> GetTMWords(TextModel textModel)
        {
            if (IsTMEmpty()) { return null; }

            var result = Task.Run(() =>
                _textModel.Paragraphs.SelectMany(p =>
                            p.Sentences.SelectMany(s =>
                                s.Words.SelectMany(w =>
                                    w.WordParts.Select(wp =>
                                    {
                                        ref string rwp = ref wp;
                                        return rwp;
                                    }).ToList())))
            );
            //var result = Task.Run(() =>
            //    _textModel.Paragraphs.SelectMany(p =>
            //                p.Sentences.SelectMany(s =>
            //                    s.Words.SelectMany(w =>
            //                        w.WordParts.ToList())))
            //);
            return result;
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
        internal static void DisplayCreateMenu()
        {
            string operation = "ИСХОДНЫЕ ДАННЫЕ:";
            string[] items = { "Выбрать файл для обработки", "Назад" };
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
                MenuManager.WaitForContinue(string.Format("Ошибка открытия файла:\n" + e.Message));
                return;
            }

            if (fileStream != null)
            {
                FileStream fs = fileStream as FileStream;
                _originalFilePath = fs == null ? string.Empty : fs.Name;  //Get the path of opened file
                string fileContent = ReadFile(fileStream);
                DoWorkAsync(fileContent);
            }            
        }




        #endregion // CONCORDANCE_FIRST_MENU



        #region CONCORDANCE_SECOND_MENU
        //###############################################################################################################################

        /// <summary>
        /// Concordance view menu
        /// </summary>
        private static void DisplayOperationMenu()
        {
            string operation = "ОПЕРАЦИИ:";
            string[] items = { "Структура текста",
                               "Показать Конкорданс",
                               "Предметный указатель слов для А4",
                               "Предметный указатель слов для А5",
                               "Показать текст сортированный по количеству слов в предложении",
                               "Показать слова вопросительных предложений",
                               "Заменить слова заданной длины в предложении",
                               "Удалить слова заданной длины в тексте",
                               "Назад" };
            MenuManager.method[] methods = new MenuManager.method[] {
                               ViewTextModel,               // "Структура текста"
                               ViewConcordance,             // "Показать Конкорданс",
                               ViewConcordanceA4,           // "Предметный указатель слов для А4"
                               ViewConcordanceA5,           // "Предметный указатель слов для А5"
                               SortTextBySentenceLength,    // "Показать текст сортированный по количеству слов в предложении
                               DisplayWordsQuestion,        // "Показать слова вопросительных предложений"
                               ReplaceWords,                // "Заменить слова заданной длины в предложении"
                               DeleteWords,                 // "Удалить слова заданной длины в тексте"
                               Back };                      // "Назад"
            MenuManager.SelectMenuItem(operation, items, methods);
        }

        /// <summary>
        /// View Text structure
        /// </summary>
        private static void ViewTextModel()
        {
            if (IsTMEmpty())
            {
                MenuManager.WaitForContinue("Нет данных для отображения.");
                return;
            }

            Console.WriteLine("АБЗАЦЫ:");
            foreach (var item in _textModel.Paragraphs)
            {
                Console.WriteLine(string.Format("{0} - {1}", item.Number, item.Content));
            }
            Console.WriteLine();

            Console.WriteLine("ПРЕДЛОЖЕНИЯ:");
            foreach (var item in _textModel.Paragraphs.Select(p => p.Sentences))
            {
                item.ToList()
                    .ForEach(s => Console.WriteLine(string.Format("{0}:{1} - {2}", s.ParagraphNumber, s.Number, s.Content)));
            }
            Console.WriteLine();

            Console.WriteLine("СЛОВА:");
            foreach (var item in _textModel.Paragraphs.Select(p => p.Sentences))
            {
                item.Select(s => s).ToList()
                    .ForEach(s => s.Words.Select(w => w).ToList()
                                    .ForEach(w => {
                                        Console.Write(string.Format("{0}:{1}:{2} - {3, -22}\t:", w.ParagraphNumber
                                                                                               , w.SentenseNumber
                                                                                               , w.Number
                                                                                               , w.Content));
                                        w.WordParts.Select(wp => wp).ToList()
                                                   .ForEach(x => Console.Write(string.Format("\t{0, -20}", x)));
                                        Console.WriteLine();
                                    }));
            }
            Console.WriteLine();
            MenuManager.WaitForContinue();
        }

        /// <summary>
        /// View Concordance by text
        /// </summary>
        private static void ViewConcordance()
        {
            MenuManager.WaitForContinue("Показ конкорданса.");
        }

        /// <summary>
        /// View Concordsnce by text to A4 page format
        /// </summary>
        private static void ViewConcordanceA4()
        {
            MenuManager.WaitForContinue("Предметный указатель слов для А4.");
        }

        /// <summary>
        /// View Concordsnce by text to A5 page format
        /// </summary>
        private static void ViewConcordanceA5()
        {
            MenuManager.WaitForContinue("Предметный указатель слов для А5.");
        }

        /// <summary>
        /// View sorted text by sentences lengt
        /// </summary>
        private static void SortTextBySentenceLength()
        {
            MenuManager.WaitForContinue("Показать текст сортированный по количеству слов в предложении.");
        }

        /// <summary>
        /// View selected words in interrogative sentences
        /// </summary>
        private static void DisplayWordsQuestion()
        {
            MenuManager.WaitForContinue("Показать слова вопросительных предложений.");
        }

        /// <summary>
        /// Replace words in selected sentences
        /// </summary>
        private static void ReplaceWords()
        {
            MenuManager.WaitForContinue("Заменить слова заданной длины в предложении.");
        }

        /// <summary>
        /// Delete words in text
        /// </summary>
        private static void DeleteWords()
        {
            MenuManager.WaitForContinue("Удалить слова заданной длины в тексте.");
        }

        #endregion



        #region READ_STREAM
        //###############################################################################################################################

        /// <summary>
        /// Get text from file next types: *.txt (...)
        /// </summary>
        /// <returns></returns>
        private static string ReadFile(Stream fileStream)
        {

            // variants of read file content by it's extentions
            FileStream fs = fileStream as FileStream;
            string extension = (Path.GetExtension(fs.Name)).ToLower();
            switch (extension)
            {
                case ".txt":
                    return ReadTextFile(fileStream);
                default:
                    break;
            }
            return String.Empty; ;
        }


        /// <summary>
        /// Get file content from text file
        /// </summary>
        /// <param name="fileStream">file stream</param>
        /// <returns>string file content</returns>
        private static string ReadTextFile(Stream fileStream)
        {
            if (fileStream == null) { return String.Empty; }

            string fileContent = string.Empty;
            using (StreamReader reader = new StreamReader(fileStream, System.Text.Encoding.Default))
            {
                try
                {
                    fileContent = reader.ReadToEnd();
                }
                catch (Exception)
                {
                    fileContent = ReadTextLines(fileStream);
                }
            }
            return fileContent;
        }


        /// <summary>
        /// Read text by lines
        /// </summary>
        /// <param name="fileStream">File Stream</param>
        /// <returns>string all text by line</returns>
        private static string ReadTextLines(Stream fileStream)
        {
            if (fileStream == null) { return String.Empty; }

            StringBuilder fileContent = null;
            try
            {
                using (StreamReader reader = new StreamReader(fileStream, System.Text.Encoding.Default))
                {
                    while (reader.Peek() >= 0)
                    {
                        if (fileContent == null) { fileContent = new StringBuilder(); }
                        fileContent.Append(reader.ReadLine());
                        fileContent.Append(string.Format("\r\n"));
                    }
                }
            }
            catch (Exception)
            {
                fileContent.Append(string.Format("\r\nОшибка чтения файла!"));
            }
            return (fileContent == null) || (fileContent.Length < 1) ? string.Empty : fileContent.ToString();
        }


        #endregion // READ_STREAM



    }
}
