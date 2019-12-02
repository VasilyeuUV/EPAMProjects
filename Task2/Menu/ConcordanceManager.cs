using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        private static TextModel _textModel = null;
        private static IEnumerable<WordPartModel> _tmWordParts = null;
        private static IEnumerable<string> _uniqueWord = null;

        public static IEnumerable<string> UniqueWord => _uniqueWord ?? TextHandler.ParseTextToWordsAsync(_textModel.Content).Result;

        internal static IEnumerable<WordPartModel> TmWordParts
        {
            get
            {
                return _tmWordParts ?? TextHandler.GetTMWordParts(_textModel).Result;
            }
            private set
            {
                _tmWordParts = value;
            }
        }

        private static string[] items = {
            "Text structure",
            "Show Concordance",
            "A4 word index",
            "A5 word index",
            "Show text sorted by word count in sentences",
            "Show words of interrogative sentences",
            "Replace words of a given length in a sentence",
            "Delete words of a given length in the text",
            "Back"
        };


        #region CONCORDANCE_SECOND_MENU
        //###############################################################################################################################

        /// <summary>
        /// Concordance view menu
        /// </summary>
        internal static void DisplayOperationMenuAsync(TextModel textModel)
        {
            if (TextHandler.IsEmptyTextModel(textModel))
            {
                ToDisplay.WaitForContinue("Нет данных для обработки.");
                return; ;
            }

            _textModel = textModel;
            string operation = "ОПЕРАЦИИ:";
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
            ToDisplay.ViewTitle(items[0].ToUpper(), true);

            ToDisplay.ViewTitle("PARAGRAPHS");
            ToDisplay.ViewBody(MakeParagraphString(Const.Task.GetTextModel));

            ToDisplay.ViewTitle("SENTENCES");
            ToDisplay.ViewBody(MakeSentencesString(Const.Task.GetTextModel));

            ToDisplay.ViewTitle("WORDS");
            ToDisplay.ViewBody(MakeWordsString(Const.Task.GetTextModel));

            ToDisplay.WaitForContinue();
        }
        

        /// <summary>
        /// View Concordance by text
        /// </summary>
        private static void ViewConcordance()
        {
            ConcordanceModel concordance = ConcordanceModel.CreateConcordanceAsync(_textModel.Paragraphs.Select(p => p.Content)).Result;

            ToDisplay.ViewTitle(items[1].ToUpper(), true);

            ToDisplay.ViewTitle("INITIAL TEXT");
            ViewParagraphInfo(_textModel.Paragraphs.Count());
            ToDisplay.ViewBody(_textModel.ContentToView());

            ToDisplay.ViewTitle("CONCORDANCE");
            ViewWordInfo();
            ToDisplay.ViewConcordanceTableHeader();
            var conc = concordance.ToString();
            ToDisplay.ViewBody(conc);

            ToDisplay.ViewTitle("To save Concordance press Enter?");
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.Enter)
            {
                FileReadWriter.WriteTxtFileAsync(conc);
            }

            ToDisplay.WaitForContinue();

        }
               

        /// <summary>
        /// View Concordsnce by text to A4 page format
        /// </summary>
        private static void ViewConcordanceA4()
        {
            TextLayoutModel textA4 = TextLayoutModel.NewInstance(_textModel.Content);
            var textA4Lines = textA4.ConvertToLines();
            ConcordanceModel concordance = ConcordanceModel.CreateConcordanceAsync(textA4Lines).Result;

            ToDisplay.ViewTitle(items[2].ToUpper(), true);
            ToDisplay.ViewTitle("INITIAL TEXT");
            ViewSentenceInfo(textA4Lines.Count());
            ToDisplay.ViewBody(textA4.ConvertToText());

            ToDisplay.ViewTitle(items[2].ToUpper());
            ViewWordInfo();
            ToDisplay.ViewConcordanceTableHeader();
            var conc = concordance.ToString();
            ToDisplay.ViewBody(conc);

            ToDisplay.ViewTitle("To save Concordance press Enter?");
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.Enter)
            {
                FileReadWriter.WriteTxtFileAsync(concordance.ToString());
            }

            ToDisplay.WaitForContinue();
        }

        
        /// <summary>
        /// View Concordsnce by text to A5 page format
        /// </summary>
        private static void ViewConcordanceA5()
        {
            TextLayoutModel textA5 = TextLayoutModel.NewInstance(_textModel.Content, Const.Page_A5);
            var textA4Lines = textA5.ConvertToLines();
            ConcordanceModel concordance = ConcordanceModel.CreateConcordanceAsync(textA4Lines).Result;

            ToDisplay.ViewTitle(items[3].ToUpper(), true);
            ToDisplay.ViewTitle("INITIAL TEXT");
            ViewSentenceInfo(textA4Lines.Count());
            ToDisplay.ViewBody(textA5.ConvertToText());

            ToDisplay.ViewTitle(items[2].ToUpper());
            ViewWordInfo();
            ToDisplay.ViewConcordanceTableHeader();
            var conc = concordance.ToString();
            ToDisplay.ViewBody(conc);

            ToDisplay.ViewTitle("To save Concordance press Enter?");
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.Enter)
            {
                FileReadWriter.WriteTxtFileAsync(concordance.ToString());
            }

            ToDisplay.WaitForContinue();
        }


        /// <summary>
        /// View sorted text by sentences lenght
        /// </summary>
        private static void SortSentencesByNumberOfWord()
        {
            ToDisplay.ViewTitle(items[4].ToUpper(), true);

            ToDisplay.ViewTitle("INITIAL TEXT");
            ToDisplay.ViewBody(MakeSentencesString(Const.Task.GetTextModel));

            ToDisplay.ViewTitle("RESULT");
            ToDisplay.ViewBody(MakeSentencesString(Const.Task.SortTextBySentence));

            ToDisplay.WaitForContinue();
        }


        /// <summary>
        /// View selected words in interrogative sentences
        /// </summary>
        private static void DisplayWordsQuestion()
        {
            IEnumerable<IContentable> needfulSentences = TextHandler.GetInterrogativeSentences(_textModel.GetSentences());
            IEnumerable<IContentable> words = TextHandler.GetUniqueWords(needfulSentences);

            ConsoleKeyInfo key;
            do
            {
                ToDisplay.ViewTitle(items[5].ToUpper(), true);

                ToDisplay.ViewTitle("INITIAL TEXT");
                ToDisplay.ViewBody(MakeSentencesString(Const.Task.GetTextModel));

                ToDisplay.ViewTitle("INTERROGATIVE SENTENCES");
                ToDisplay.ViewBody(MakeSentencesString(Const.Task.InterrogativeSentences, needfulSentences));

                if (needfulSentences != null && needfulSentences.Count() > 0)
                {
                    int findWordLength = EnterNumberMenu("Enter word length: ", Const.MAX_WORD_LENGTH);
                    var selectedWords = words.Where(w => w.Content.Length == findWordLength)
                                      .GroupBy(w => w.Content)
                                      .OrderBy(w => w.Key);


                    ToDisplay.ViewTitle("SUITABLE WORDS");
                    ToDisplay.ViewBody(ConvertToString(selectedWords.Select(x => x.Key)));
                }
                else
                {
                    ToDisplay.ViewBody("There are no QUESTIONAL PROPOSALS in the text");
                }
                key = ToDisplay.WaitForContinue("Press Esc for back");

            } while (key.Key != ConsoleKey.Escape);                        
        }


        /// <summary>
        /// Replace words in selected sentences
        /// </summary>
        private static void ReplaceWords()
        {
            const int MAX_SENTENCE_COUNT = 10;

            IEnumerable<IContentable> needfulSentences = _textModel.GetSentences().Take(MAX_SENTENCE_COUNT);
            var sentences = needfulSentences.Select(x => x.Content).ToArray();

            ConsoleKeyInfo key;
            do
            {
                ToDisplay.ViewTitle(items[6].ToUpper(), true);

                ToDisplay.ViewTitle("SENTENCES");
                ToDisplay.ViewBody(ConvertToString(sentences));

                int selectedSentenceNumber = EnterNumberMenu("Enter the sentence number: ", MAX_SENTENCE_COUNT);
                if (selectedSentenceNumber > sentences.Count()) { selectedSentenceNumber = sentences.Count(); }

                var selectedSentence = sentences[selectedSentenceNumber - 1];
                var words = TextHandler.ParseTextToWordsAsync(selectedSentence).Result;

                ToDisplay.ViewTitle("SELECTED SENTENCES");
                ToDisplay.ViewBody(selectedSentence);

                int findWordLength = EnterNumberMenu("Enter word length: ", Const.MAX_WORD_LENGTH);
                var selectedWords = words.Where(w => w.Length == findWordLength)
                                         .Distinct();
                ToDisplay.ViewTitle("SUITABLE WORDS");
                ToDisplay.ViewBody(ConvertToString(selectedWords));

                if (selectedWords != null && selectedWords.Count() > 0)
                {
                    Console.WriteLine("Enter new text");
                    string newWord = Console.ReadLine();

                    string newSentence = selectedSentence;
                    foreach (var item in selectedWords)
                    {
                        newSentence = newSentence.Replace(item.ToLower(), newWord);
                    }

                    ToDisplay.ViewTitle("RESULT");
                    ToDisplay.ViewBody(newSentence);
                }
                else
                {
                    ToDisplay.ViewBody("No matching words");
                }
                
                key = ToDisplay.WaitForContinue("Press Esc for back");

            } while (key.Key != ConsoleKey.Escape);
        }



        /// <summary>
        /// Delete words in text
        /// </summary>
        private static void DeleteWords()
        {
            string consonantsRussian = "йцкнгшщзхфвпрлджбтмсч";
            string consonantsEnglish = "qwrtplkjhgfdszxcvbnm";
            string txt = _textModel.Content;

            ConsoleKeyInfo key;
            do
            {
                ToDisplay.ViewTitle(items[7].ToUpper(), true);

                ToDisplay.ViewTitle("INITIAL TEXT");
                ToDisplay.ViewBody(txt);

                int findWordLength = EnterNumberMenu("Enter word length: ", Const.MAX_WORD_LENGTH);
                var selectedWords = TmWordParts.Where(w => w.Content.Length == findWordLength
                                               && (consonantsRussian.IndexOf(char.ToLower(w.Content[0])) != -1
                                                   || consonantsEnglish.IndexOf(char.ToLower(w.Content[0])) != -1))
                                               .Select(w => w.Content);
                foreach (var item in selectedWords)
                {
                    txt =  Regex.Replace(txt, $@"\s*\w*{item}\b", "", RegexOptions.IgnoreCase).Trim();
                } 

                ToDisplay.ViewTitle("RESULT");
                ToDisplay.ViewBody(txt);

                key = ToDisplay.WaitForContinue("Press Esc for back");

            } while (key.Key != ConsoleKey.Escape);
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






        #region CONCORDANCE
        //###############################################################################################################################









        #endregion // CONCORDANCE






        #region PARAGRAPH_STRING_MAKER
        //###############################################################################################################################

        /// <summary>
        /// Paragraph options to display
        /// </summary>
        /// <returns></returns>
        private static string MakeParagraphString(Const.Task task)
        {
            if (_textModel.Paragraphs.Count() > Const.PARAGRAPH_VIEW_LIMIT)
            {
                ToDisplay.ViewInfo("paragraphs", _textModel.Paragraphs.Count(), Const.PARAGRAPH_VIEW_LIMIT);
            }
            else { ToDisplay.ViewInfo("paragraphs", _textModel.Paragraphs.Count()); }


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
                if (++count >= Const.PARAGRAPH_VIEW_LIMIT)
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
            IEnumerable<IContentable> sentences = sent ?? GetSentences();
            IEnumerable<SentenceModel> lst = TextHandler.ConvertIContentableToList<SentenceModel>(sentences);
            
            if (sentences.Count() > Const.SENTENCE_VIEW_LIMIT)
            {
                ToDisplay.ViewInfo("sentences", sentences.Count(), Const.SENTENCE_VIEW_LIMIT);
            }
            else { ToDisplay.ViewInfo("sentences", sentences.Count()); }

            switch (task)
            {
                case Const.Task.GetTextModel:               return GetSentencesDefaultString(lst);
                case Const.Task.SortTextBySentence:         return GetSentencesSortedString(lst, false);
                case Const.Task.InterrogativeSentences:     return GetSentencesDefaultString(lst);
            }
            return string.Empty;
        }





        /// <summary>
        /// Default String format for view sentences
        /// </summary>
        /// <param name="viewCount"></param>
        /// <param name="sentences"></param>
        /// <returns></returns>
        private static string GetSentencesDefaultString(IEnumerable<SentenceModel> sentences)
        {
            if (sentences == null) { return string.Empty; }

            StringBuilder str = new StringBuilder();
            int count = 1;
            foreach (var item in sentences)
            {
                //var sentence = item as SentenceModel;
                //if (sentence == null) { continue; }

                str.Append(string.Format("{0}:{1}. {2}\n", item.ParagraphNumber, item.Number, item.Content));
                if (++count >= Const.SENTENCE_VIEW_LIMIT)
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
        private static string GetSentencesSortedString(IEnumerable<SentenceModel> sentences, bool asc = true)
        {
            var sent = asc ? sentences.OrderBy(s => s.Words.Count()) : sentences.OrderByDescending(s => s.Words.Count());

            StringBuilder str = new StringBuilder();
            int count = 1;
            foreach (var item in sent)
            {
                str.Append(string.Format("({0} шт.) {1}\n",
                    item.Words.Count(),
                    item.Content));
                if (++count >= Const.SENTENCE_VIEW_LIMIT)
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
        private static string MakeWordsString(Const.Task task, IEnumerable<IContentable> words = null)
        {
            if (words == null) { words = GetWords(); }           

            if (words.Count() > Const.WORD_VIEW_LIMIT)
            {
                ToDisplay.ViewInfo("words", words.Count(), Const.WORD_VIEW_LIMIT);
            }
            else { ToDisplay.ViewInfo("words", words.Count()); }


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

                if (++count >= Const.WORD_VIEW_LIMIT)
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
        /// View Paragraph information (number of displayed / from count)
        /// </summary>
        /// <param name="viewCount"></param>
        private static void ViewParagraphInfo(int viewCount)
        {
            if (viewCount > Const.PARAGRAPH_VIEW_LIMIT)
            {
                ToDisplay.ViewInfo("paragraphs", viewCount, Const.PARAGRAPH_VIEW_LIMIT);
            }
            else { ToDisplay.ViewInfo("paragraphs", viewCount); }
        }

        /// <summary>
        /// View Sentences information (number of displayed / from count)
        /// </summary>
        /// <param name="viewCount"></param>
        private static void ViewSentenceInfo(int viewCount)
        {
            if (viewCount > Const.SENTENCE_VIEW_LIMIT)
            {
                ToDisplay.ViewInfo("lines", viewCount, Const.SENTENCE_VIEW_LIMIT);
            }
            else { ToDisplay.ViewInfo("lines", viewCount); }
        }

        /// <summary>
        /// View Word information (number of displayed / from count)
        /// </summary>
        private static void ViewWordInfo()
        {
            if (UniqueWord.Count() > Const.WORD_VIEW_LIMIT)
            {
                ToDisplay.ViewInfo("words", UniqueWord.Count(), Const.WORD_VIEW_LIMIT);
            }
            else { ToDisplay.ViewInfo("words", UniqueWord.Count()); }
        }

        /// <summary>
        /// Enter Wordl length menu
        /// </summary>
        /// <returns></returns>
        private static int EnterNumberMenu(string v, int maxValue)
        {
            int n;
            do
            {
                Console.Write(v);
                try
                {
                    n = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                    n = 0;
                }
                if (n < 1 || n > maxValue)
                {
                    Console.Write("Incorrect value. Enter a numerical value.");
                    ClearCurrentConsoleLine();
                }

            } while (n < 1 || n > maxValue);
            Console.WriteLine();
            ClearCurrentConsoleLine();
            Console.WriteLine();
            return n;
        }


        /// <summary>
        /// Clear console at current position
        /// </summary>
        private static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop - 1;
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }


        /// <summary>
        ///  Convert list to string
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        private static string ConvertToString(IEnumerable<string> words)
        {
            StringBuilder result = new StringBuilder();

            int count = 0;
            foreach (var item in words)
            {
                result.Append(item + Const.NEW_PARAGRAPH);

                if (++count > Const.WORD_VIEW_LIMIT) { break; }
            }
            return result.ToString();
        }


        #endregion // DISPLAY_TEXT





    }
}
