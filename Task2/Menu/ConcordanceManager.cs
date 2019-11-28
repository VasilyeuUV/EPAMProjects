using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task2.Models;
using Task2.Tools;

namespace Task2.Menu
{
    public static class ConcordanceManager
    {
        internal delegate void method();

        private static TextModel _textModel = null;
        private static IEnumerable<string> _tmWords = null;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="textModel"></param>
        /// <returns></returns>
        private static Task<IEnumerable<string>> GetTMWords(TextModel textModel)
        {
            if (TextHandler.IsEmptyTextModel(textModel)) { return null; }

            var result = Task.Run(() =>
                  textModel.Paragraphs.SelectMany(p =>
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

        


        #region CONCORDANCE_SECOND_MENU
        //###############################################################################################################################

        /// <summary>
        /// Concordance view menu
        /// </summary>
        internal async static void DisplayOperationMenuAsync(TextModel textModel)
        {
            if (TextHandler.IsEmptyTextModel(textModel))
            {
                MenuManager.WaitForContinue("Нет данных для обработки.");
                return; ;
            }

            _textModel = textModel;            

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
            method[] methods = new method[] {
                               ViewTextModel,               // "Структура текста"
                               ViewConcordance,             // "Показать Конкорданс",
                               ViewConcordanceA4,           // "Предметный указатель слов для А4"
                               ViewConcordanceA5,           // "Предметный указатель слов для А5"
                               SortTextBySentenceLength,    // "Показать текст сортированный по количеству слов в предложении
                               DisplayWordsQuestion,        // "Показать слова вопросительных предложений"
                               ReplaceWords,                // "Заменить слова заданной длины в предложении"
                               DeleteWords,                 // "Удалить слова заданной длины в тексте"
                               Back };                      // "Назад"
            SelectMenuItem(operation, items, methods);

            _tmWords = await GetTMWords(textModel);
        }




        /// <summary>
        /// View Text structure
        /// </summary>
        private static void ViewTextModel()
        {
            //if (IsTMEmpty())
            //{
            //    MenuManager.WaitForContinue("Нет данных для отображения.");
            //    return;
            //}

            //Console.WriteLine("АБЗАЦЫ:");
            //foreach (var item in _textModel.Paragraphs)
            //{
            //    Console.WriteLine(string.Format("{0} - {1}", item.Number, item.Content));
            //}
            //Console.WriteLine();

            //Console.WriteLine("ПРЕДЛОЖЕНИЯ:");
            //foreach (var item in _textModel.Paragraphs.Select(p => p.Sentences))
            //{
            //    item.ToList()
            //        .ForEach(s => Console.WriteLine(string.Format("{0}:{1} - {2}", s.ParagraphNumber, s.Number, s.Content)));
            //}
            //Console.WriteLine();

            //Console.WriteLine("СЛОВА:");
            //foreach (var item in _textModel.Paragraphs.Select(p => p.Sentences))
            //{
            //    item.Select(s => s).ToList()
            //        .ForEach(s => s.Words.Select(w => w).ToList()
            //                        .ForEach(w => {
            //                            Console.Write(string.Format("{0}:{1}:{2} - {3, -22}\t:", w.ParagraphNumber
            //                                                                                   , w.SentenseNumber
            //                                                                                   , w.Number
            //                                                                                   , w.Content));
            //                            w.WordParts.Select(wp => wp).ToList()
            //                                       .ForEach(x => Console.Write(string.Format("\t{0, -20}", x)));
            //                            Console.WriteLine();
            //                        }));
            //}
            //Console.WriteLine();
            //MenuManager.WaitForContinue();
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



        /// <summary>
        /// Return to the menu one level higher
        /// </summary>
        private static void Back()
        {
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

        #endregion


    }
}
