using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private const int PARAGRAPH_VIEW_LIMIT = 10;
        private const int SENTENCE_VIEW_LIMIT = 25;
        private const int WORD_VIEW_LIMIT = 100;

        private static TextModel _textModel = null;
        private static IEnumerable<string> _tmWordParts = null;
        private static IEnumerable<string> _uniqueWord = null;

        public static IEnumerable<string> UniqueWord => _uniqueWord == null 
            ? TextHandler.ParseTextToWordsAsync(_textModel.Content).Result 
            : _uniqueWord;

        public static IEnumerable<string> TmWordParts => _tmWordParts == null
            ? TextHandler.GetTMWordParts(_textModel).Result
            : _tmWordParts;
        
        


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
                               SortSentencesByNumberOfWord, // "Показать текст сортированный по количеству слов в предложении
                               DisplayWordsQuestion,        // "Показать слова вопросительных предложений"
                               ReplaceWords,                // "Заменить слова заданной длины в предложении"
                               DeleteWords,                 // "Удалить слова заданной длины в тексте"
                               Back };                      // "Назад"
            SelectMenuItem(operation, items, methods);
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
            //"Показ конкорданса."
            //ToDisplay.ViewTitle("КОНКОРДАНС", true);
            //ToDisplay.ViewBody(MakeConcordanceString(Const.Task.ViewConcordance));
            //

            foreach (var item in TmWordParts)
            {
                Console.WriteLine(item);
            }
            ToDisplay.WaitForContinue();
        }


        /// <summary>
        /// View sorted text by sentences lenght
        /// </summary>
        private static void SortSentencesByNumberOfWord()
        {
            // Показать текст сортированный по количеству слов в предложении
            ToDisplay.ViewTitle("СОРТИРОВКА ПРЕДЛОЖЕНИЙ ПО КОЛИЧЕСТВУ СЛОВ", true);

            ToDisplay.ViewTitle("ИСХОДНЫЙ ТЕКСТ");
            ToDisplay.ViewBody(MakeSentencesString(Const.Task.GetTextModel));

            ToDisplay.ViewTitle("СОРТИРОВАННЫЙ ПО ПРЕДЛОЖЕНИЯМ ТЕКСТ");
            ToDisplay.ViewBody(MakeSentencesString(Const.Task.SortTextBySentence));  
            
            ToDisplay.WaitForContinue();
        }


        /// <summary>
        /// Concordance options to display
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private static string MakeConcordanceString(Const.Task task)
        {
            if (UniqueWord.Count() > WORD_VIEW_LIMIT)
            {
                ToDisplay.ViewInfo(UniqueWord.Count(), WORD_VIEW_LIMIT);
            }
            else { ToDisplay.ViewInfo(UniqueWord.Count()); }
            
            switch (task)
            {
                case Const.Task.ViewConcordance: return GetConcordanceString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Get Concordance
        /// </summary>
        /// <param name="page_A4Standart"></param>
        /// <returns></returns>
        private static string GetConcordanceString(IReadOnlyDictionary<Const.PageParameter, int> page = null)
        {
            //List<WordModel> lst = new List<WordModel>();
            //// ТУТ ИНОГДА ВЫДАЕТ ОШИБКУ О НЕДОСТАТОЧНОСТИ РАЗМЕРА МАССИВА (РАЗОБРАТЬСЯ)
            //_textModel.Paragraphs.AsParallel()
            //          .ForAll(p => lst.AddRange(p.Sentences.SelectMany(w => w.Words).ToList()));




            StringBuilder str = new StringBuilder();

            //str.Append(string.Format("КОНКОРДАНС ({0} слов)\n\n", _concordance.WordUniqueCount));

            if (page == null)
            {
                
                int count = 1;
                foreach (var item in _textModel.Paragraphs)
                {
                    str.Append(string.Format("{0} - {1}\n", item.Number, item.Content));
                    if (++count >= WORD_VIEW_LIMIT)
                    {
                        str.Append(string.Format("\n"));
                        break;
                    }
                }
                string result = str.ToString().Trim();
                
            }

            return "";
            //return string.IsNullOrWhiteSpace(result) ? "Нет данных для отображения." : result;
        }



        /// <summary>
        /// View full concordance
        /// </summary>
        //private static void ViewConcordance()
        //{
        //    StringBuilder str = new StringBuilder();

        //    str.Append(string.Format("КОНКОРДАНС ({0} слов)\n\n", _concordance.WordUniqueCount));

        //    int count = 0;

        //    foreach (var item in _concordance.Words)
        //    {
        //        str.Append(string.Format("{0}. {1, -25} : {2, 3} шт.\n       ", ++count, item.Word, item.Positions.Count));
        //        str.Append(ViewConcordance(item));
        //        str.Append(string.Format("\n"));
        //    }

        //    WaitForContinue(str.ToString());
        //}

        ///// <summary>
        ///// View full concordance
        ///// </summary>
        //private static void ViewConcordance()
        //{
        //    StringBuilder str = new StringBuilder();

        //    str.Append(string.Format("КОНКОРДАНС ({0} слов)\n\n", _concordance.WordUniqueCount));

        //    int count = 0;

        //    foreach (var item in _concordance.Words)
        //    {
        //        str.Append(string.Format("{0}. {1, -25} : {2, 3} шт.\n       ", ++count, item.Word, item.Positions.Count));
        //        str.Append(ViewConcordance(item));
        //        str.Append(string.Format("\n"));
        //    }

        //    WaitForContinue(str.ToString());
        //}



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
            if (_textModel.Paragraphs.Count() > PARAGRAPH_VIEW_LIMIT)
            {
                ToDisplay.ViewInfo(_textModel.Paragraphs.Count(), PARAGRAPH_VIEW_LIMIT);
            }
            else { ToDisplay.ViewInfo(_textModel.Paragraphs.Count()); }


            switch (task)
            {
                case Const.Task.GetTextModel: return GetParagraphsDefaultString();                
            }
            return string.Empty;

        }


        /// <summary>
        /// Default String format for view paragraphs
        /// </summary>
        /// <param name="ViewCount"></param>
        /// <returns></returns>
        private static string GetParagraphsDefaultString()
        {
            StringBuilder str = new StringBuilder();
            int count = 1;
            foreach (var item in _textModel.Paragraphs)
            {
                str.Append(string.Format("{0} - {1}\n", item.Number, item.Content));
                if (++count >= PARAGRAPH_VIEW_LIMIT)
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
            IEnumerable<IContentable> sentences = sent == null ? GetSentences() : sent;
            List<SentenceModel> lst = TextHandler.ConvertIContentabletToList<SentenceModel>(sentences);

            if (sentences.Count() > SENTENCE_VIEW_LIMIT)
            {
                ToDisplay.ViewInfo(sentences.Count(), SENTENCE_VIEW_LIMIT);
            }
            else { ToDisplay.ViewInfo(sentences.Count()); }

            switch (task)
            {
                case Const.Task.GetTextModel: return GetSentencesDefaultString(lst);
                case Const.Task.SortTextBySentence: return GetSentencesSortedString(lst, false);
            }
            return string.Empty;
        }





        /// <summary>
        /// Default String format for view sentences
        /// </summary>
        /// <param name="viewCount"></param>
        /// <param name="sentences"></param>
        /// <returns></returns>
        private static string GetSentencesDefaultString(List<SentenceModel> sentences)
        {
            StringBuilder str = new StringBuilder();
            int count = 1;
            foreach (var item in sentences)
            {
                //var sentence = item as SentenceModel;
                //if (sentence == null) { continue; }

                str.Append(string.Format("{0}:{1}. {2}\n", item.ParagraphNumber, item.Number, item.Content));
                if (++count >= SENTENCE_VIEW_LIMIT)
                {
                    str.Append(string.Format("\n"));
                    break;
                }
            }
            string result = str.ToString().Trim();
            return string.IsNullOrWhiteSpace(result) ? "Нет данных для отображения." : result;

        }


        /// <summary>
        /// String to view offers sorted by number of word
        /// </summary>
        /// <param name="asc">sort by ascending (true) or descending (false)</param>
        /// <returns>sentences string sorted by length </returns>
        private static string GetSentencesSortedString(List<SentenceModel> sentences, bool asc = true)
        {
            var sent = asc ? sentences.OrderBy(s => s.Words.Count()) : sentences.OrderByDescending(s => s.Words.Count());

            StringBuilder str = new StringBuilder();
            int count = 1;
            foreach (var item in sent)
            {
                str.Append(string.Format("({0} шт.) {1}\n",
                    item.Words.Count(),
                    item.Content));
                if (++count >= SENTENCE_VIEW_LIMIT)
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
            IEnumerable<IContentable> words = GetWords();

            if (words.Count() > WORD_VIEW_LIMIT)
            {
                ToDisplay.ViewInfo(words.Count(), WORD_VIEW_LIMIT);
            }
            else { ToDisplay.ViewInfo(words.Count()); }


            switch (task)
            {
                case Const.Task.GetTextModel: return GetWordDefaultString(words);
            }
            return string.Empty;
        }


        /// <summary>
        /// Default String format for view words
        /// </summary>
        /// <param name="viewCount"></param>
        /// <param name="words"></param>
        /// <returns></returns>
        private static string GetWordDefaultString(IEnumerable<IContentable> words)
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

                if (++count >= WORD_VIEW_LIMIT)
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
