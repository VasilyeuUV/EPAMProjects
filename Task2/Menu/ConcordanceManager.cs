using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task2.Interfaces;
using Task2.Models;
using Task2.Tools;

namespace Task2.Menu
{
    public static class ConcordanceManager
    {
        internal delegate void method();

        /// <summary>
        /// Coefficient of displayed TextModel elements
        /// </summary>
        private const int VIEW_COEF = 5;

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
                ToDisplay.WaitForContinue("Нет данных для обработки.");
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
            ToDisplay.ViewTitle("СТРУКТУРА ТЕКСТА", true);

            ToDisplay.ViewTitle("АБЗАЦЫ");
            ToDisplay.ViewBody(MakeParagraphString(Const.Task.GetTextModel));

            ToDisplay.ViewTitle("ПРЕДЛОЖЕНИЯ");
            ToDisplay.ViewBody(MakeSentencesString(Const.Task.GetTextModel));

            ToDisplay.ViewTitle("СЛОВА");
            ToDisplay.ViewBody(MakeWordsString(Const.Task.GetTextModel));

            ToDisplay.WaitForContinue();
        }







        /// <summary>
        /// View Concordance by text
        /// </summary>
        private static void ViewConcordance()
        {
            ToDisplay.WaitForContinue("Показ конкорданса.");
        }

        /// <summary>
        /// View Concordsnce by text to A4 page format
        /// </summary>
        private static void ViewConcordanceA4()
        {
            ToDisplay.WaitForContinue("Предметный указатель слов для А4.");
        }

        /// <summary>
        /// View Concordsnce by text to A5 page format
        /// </summary>
        private static void ViewConcordanceA5()
        {
            ToDisplay.WaitForContinue("Предметный указатель слов для А5.");
        }

        /// <summary>
        /// View sorted text by sentences lengt
        /// </summary>
        private static void SortTextBySentenceLength()
        {
            // Показать текст сортированный по количеству слов в предложении
            ViewTitle("СОРТИРОВКА ТЕКСТА ПО ДЛИНЕ ПРЕДЛОЖЕНИЙ");
            DisplaySortTMbyElement(Const.Sort.Sentences);
            ToDisplay.WaitForContinue();
        }


        /// <summary>
        /// View selected words in interrogative sentences
        /// </summary>
        private static void DisplayWordsQuestion()
        {
            ToDisplay.WaitForContinue("Показать слова вопросительных предложений.");
        }

        /// <summary>
        /// Replace words in selected sentences
        /// </summary>
        private static void ReplaceWords()
        {
            ToDisplay.WaitForContinue("Заменить слова заданной длины в предложении.");
        }

        /// <summary>
        /// Delete words in text
        /// </summary>
        private static void DeleteWords()
        {
            ToDisplay.WaitForContinue("Удалить слова заданной длины в тексте.");
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

        #endregion // CONCORDANCE_SECOND_MENU




        #region PARAGRAPH_STRING_MAKER
        //###############################################################################################################################

        /// <summary>
        /// Paragraph options to display
        /// </summary>
        /// <returns></returns>
        private static string MakeParagraphString(Const.Task task)
        {
            int viewCount = VIEW_COEF;
            if (_textModel.Paragraphs.Count() > viewCount)
            {
                ToDisplay.ViewInfo(_textModel.Paragraphs.Count(), viewCount);
            }

            switch (task)
            {
                case Const.Task.GetTextModel: return GetParagraphsDefaultString(viewCount);
            }
            return string.Empty;

        }

        /// <summary>
        /// Default String format for view paragraphs
        /// </summary>
        /// <param name="ViewCount"></param>
        /// <returns></returns>
        private static string GetParagraphsDefaultString(int ViewCount)
        {
            StringBuilder str = new StringBuilder();
            int count = 1;
            foreach (var item in _textModel.Paragraphs)
            {
                str.Append(string.Format("{0} - {1}\n", item.Number, item.Content));
                if (++count >= ViewCount)
                {
                    str.Append(string.Format("\n"));
                    break;
                }
            }
            string result = str.ToString().Trim();
            return string.IsNullOrWhiteSpace(result) ? "Нет данных для отображения." : result;
        }

        #endregion // PARAGRAPH_STRING_MAKER




        #region SENTENCE_STRING_MAKER
        //###############################################################################################################################

        /// <summary>
        /// Sentences options to display
        /// </summary>
        /// <param name="task">Task number</param>
        /// <param name="sent">Sentences list </param>
        /// <returns></returns>
        private static string MakeSentencesString(Const.Task task, IEnumerable<IContentable> sent = null)
        {
            int viewCount = VIEW_COEF * 2;
            IEnumerable<IContentable> sentences = sent == null ? GetSentences() : sent;

            if (sentences.Count() > viewCount)
            {
                ToDisplay.ViewInfo(sentences.Count(), viewCount);
            }

            switch (task)
            {
                case Const.Task.GetTextModel: return GetSentencesDefaultString(viewCount, sentences);
            }
            return string.Empty;
        }

        /// <summary>
        /// Default String format for view sentences
        /// </summary>
        /// <param name="viewCount"></param>
        /// <param name="sentences"></param>
        /// <returns></returns>
        private static string GetSentencesDefaultString(int viewCount, IEnumerable<IContentable> sentences)
        {
            StringBuilder str = new StringBuilder();
            int count = 1;
            foreach (var item in sentences)
            {
                var sentence = item as SentenceModel;
                if (sentence == null) { continue; }

                str.Append(string.Format("{0}:{1} - {2}", sentence.ParagraphNumber, sentence.Number, sentence.Content));
                if (++count >= viewCount)
                {
                    str.Append(string.Format("\n"));
                    break;
                }
            }
            string result = str.ToString().Trim();
            return string.IsNullOrWhiteSpace(result) ? "Нет данных для отображения." : result;

        }





        #endregion // SENTENCES_STRING_MAKER




        #region WORD_STRING_MAKER
        //###############################################################################################################################

        /// <summary>
        /// Words options to display
        /// </summary>
        /// <param name="getTextModel"></param>
        /// <returns></returns>
        private static string MakeWordsString(Const.Task task)
        {
            int viewCount = VIEW_COEF * 10;
            IEnumerable<IContentable> words = GetWords();

            if (words.Count() > viewCount)
            {
                ToDisplay.ViewInfo(words.Count(), viewCount);
            }

            switch (task)
            {
                case Const.Task.GetTextModel: return GetWordDefaultString(viewCount, words);
            }
            return string.Empty;
        }


        /// <summary>
        /// Default String format for view words
        /// </summary>
        /// <param name="viewCount"></param>
        /// <param name="words"></param>
        /// <returns></returns>
        private static string GetWordDefaultString(int viewCount, IEnumerable<IContentable> words)
        {
            StringBuilder str = new StringBuilder();
            int count = 1;
            foreach (var item in words)
            {
                var w = item as WordModel;
                if (w == null) { continue; }

                str.Append(string.Format(string.Format("{0}:{1}:{2} - {3, -22}\t:", w.ParagraphNumber
                                                       , w.SentenseNumber
                                                       , w.Number
                                                       , w.Content)));
                w.WordParts.Select(wp => wp).ToList()
                           .ForEach(x => str.Append(string.Format("\t{0, -20}", x)));
                str.Append(string.Format("\n"));

                if (++count >= viewCount)
                {
                    str.Append(string.Format("\n"));
                    break;
                }
            }
            string result = str.ToString().Trim();
            return string.IsNullOrWhiteSpace(result) ? "Нет данных для отображения." : result;

            //foreach (var item in _textModel.Paragraphs.Select(p => p.Sentences))
            //{
            //    item.Select(s => s).ToList()
            //        .ForEach(s => s.Words.Select(w => w).ToList()
            //                        .ForEach(w =>
            //                        {
            //                            Console.Write(string.Format("{0}:{1}:{2} - {3, -22}\t:", w.ParagraphNumber
            //                                                                                   , w.SentenseNumber
            //                                                                                   , w.Number
            //                                                                                   , w.Content));
            //                            w.WordParts.Select(wp => wp).ToList()
            //                                       .ForEach(x => Console.Write(string.Format("\t{0, -20}", x)));
            //                            Console.WriteLine();
            //                        }));
            //}
        }


        #endregion // WORD_STRING_MAKER






        #region GET_TEXTMODEL_ELEMENTS
        //###############################################################################################################################

        /// <summary>
        /// Get this TextModel Senteces
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<IContentable> GetSentences()
        {
            return TextHandler.GetTMElements(_textModel.Paragraphs.SelectMany(p => p.Sentences));
        }


        /// <summary>
        /// Get this TextModel Words list
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<IContentable> GetWords()
        {
            return TextHandler.GetTMElements(_textModel.Paragraphs.SelectMany(p => p.Sentences).SelectMany(s => s.Words));
        }


        #endregion // GET_TEXTMODEL_ELEMENTS





        #region DISPLAY_TEXT
        //###############################################################################################################################

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elt"></param>
        /// <param name="asc"></param>
        private static void DisplaySortTMbyElement(Const.Sort elt, bool asc = false)
        {            
            switch (elt)
            {
                case Const.Sort.Sentences:
                    var sent = GetSentences();
                    sent = asc ? sent.OrderBy(x => x.Content.Length) : sent.OrderByDescending(x => x.Content.Length);
                    break;
                default:
                    break;
            }            
        }


        /// <summary>
        /// Show information if there are many objects
        /// </summary>
        private static void ViewIfMoreToDisplayLimit(int objCount, int viewCount)
        {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"(показано {viewCount} из {objCount})");
                Console.ForegroundColor = ConsoleColor.White;
        }


        /// <summary>
        /// View operation Title
        /// </summary>
        /// <param name="title"></param>
        private static void ViewTitle(string title)
        {
            Console.Clear();
            Console.WriteLine(title + ":");
            Console.WriteLine();
        }
        #endregion // DISPLAY_TEXT



    }
}
