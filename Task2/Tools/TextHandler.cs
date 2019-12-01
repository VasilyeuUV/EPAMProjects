using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Task2.Interfaces;
using Task2.Models;

namespace Task2.Tools
{
    internal static class TextHandler
    {

        /// <summary>
        /// Get Words from text
        /// </summary>
        /// <param name="text">text content</param>
        /// <returns>string list words</returns>
        internal static async Task<IEnumerable<string>> ParseTextToWordsAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return null; }

            List<string> words = Task.Run(() =>
                                 Regex.Split(text, Const.WORD_DELIMITER)
                                      .Where(s => (s = s.Trim()) != string.Empty)
                                      .ToList()).Result;

            List<string> result = new List<string>();
            words.AsParallel()
                 .ForAll(w =>
                 {
                     var item = GetWordContent(w);
                     if (!string.IsNullOrWhiteSpace(item))
                     {
                         // capital letter
                         item = IsUpperCase(item) ? item
                                            : System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(item.ToLower());

                         // ИНДЕКС НАХОДИТСЯ ВНЕ ГРАНИЦ (?)
                         result.Add(item);

                     }
                 });

            return result.Distinct().Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x).ToList();
        }


        internal static IEnumerable<string> ParseTextToWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return null; }

            IEnumerable<string> words = Task.Run(() =>
                                 Regex.Split(text, Const.WORD_DELIMITER)
                                      .Where(s => (s = s.Trim()) != string.Empty)
                                      .ToList()).Result;

            List<string> result = new List<string>();

            //foreach (var item in words)
            //{
            //    var w = GetWordContent(item);
            //    if (!string.IsNullOrWhiteSpace(w))
            //    {
            //        // capital letter
            //        w = IsUpperCase(w) ? w
            //                           : System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(w.ToLower());
            //        result.Add(w);

            //    }
            //}



            words.AsParallel()
                 .ForAll(w =>
                 {
                     var item = GetWordContent(w);
                     if (!string.IsNullOrWhiteSpace(item))
                     {
                         // capital letter
                         item = IsUpperCase(item) ? item
                                            : System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(item.ToLower());

                         // ИНДЕКС НАХОДИТСЯ ВНЕ ГРАНИЦ (?)
                         result.Add(item);

                     }
                 });

            return result.Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x).ToList();
        }
               

        /// <summary>
        /// Create list concordance words from TextModelobject
        /// </summary>
        /// <param name="paragraphs"></param>
        /// <returns></returns>
        internal static IOrderedEnumerable<IGrouping<string, WordPartModel>> GetGroupedWordParts(IEnumerable<WordPartModel> wp, 
                                                                  IEnumerable<string> unique = null,
                                                                  TextLayoutModel page = null)
        {
            var result = wp.Where(w => w.Content.Length > 0)
                           .GroupBy(w => w.Content)
                           .OrderBy(gr => gr.Key);
                           //.ToList();
            return result;
        }





        #region TEXTMODEL_OPERATIONS
        //##############################################################################################################################

        /// <summary>
        /// Get all words, symbols, etc.
        /// </summary>
        /// <param name="textModel"></param>
        /// <returns></returns>
        internal static Task<IEnumerable<WordPartModel>> GetTMWordParts(TextModel textModel)
        {
            if (TextHandler.IsEmptyTextModel(textModel)) { return null; }

            var result = Task.Run(() =>
                  textModel.Paragraphs.SelectMany(p =>
                            p.Sentences.SelectMany(s =>
                                s.Words.SelectMany(w =>
                                    w.WordPartsModel.Select(wp =>
                                    {
                                        ref WordPartModel rwp = ref wp;
                                        return rwp;
                                    }).ToList())))
            );
            return result;
        }


        /// <summary>
        /// Check TextModel object for null or empty
        /// </summary>
        /// <returns></returns>
        internal static bool IsEmptyTextModel(TextModel tm)
        {
            return tm == null || tm.Paragraphs == null
                              || tm.Paragraphs.Count() < 1;
        }


        /// <summary>
        /// Get list string of TM list elements
        /// </summary>
        /// <param name="textModel"></param>
        /// <returns></returns>
        internal static IEnumerable<IContentable> GetTMElements(IEnumerable<IContentable> elements)
        {
            if (IsEmpty(elements)) { return null; }
            return elements.Select(x => x).ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        internal static IContentable GetTMElements(IEnumerable<IContentable> elements, int n = 1)
        {
            if (IsEmpty(elements) || n < 1 || n > elements.Count()) { return null; }
            return elements.Skip(n - 1).Take(1).FirstOrDefault();
        }

        #endregion





        #region TEXT_CONVERTERS
        //##############################################################################################################################








        /// <summary>
        /// Make string from List string </string>
        /// </summary>
        /// <param name="lines">strings list</param>
        /// <returns>string</returns>
        internal static string GetText(IEnumerable<string> lines)
        {
            if (IsEmpty(lines)) { return null; }

            StringBuilder result = new StringBuilder();
            int viewCountLimit = 0;
            foreach (var line in lines)
            {
                result.Append(line + Const.NEW_PARAGRAPH);

                if (++viewCountLimit > Const.SENTENCE_VIEW_LIMIT) { break; }
            }

            return result.ToString().Trim().Length > 0 ? result.ToString() : string.Empty;
        }





        /// <summary>
        /// Convert IContentable list to needed class 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        internal static IEnumerable<T> ConvertIContentableToList<T>(IEnumerable<IContentable> collection)
            where T : class, IContentable
        {
            if (TextHandler.IsEmpty(collection)) { return null; }

            List<T> lst = new List<T>();
            foreach (var item in collection)
            {
                T result = item as T;
                if (result == null) { continue; }
                lst.Add(result);

            }
            return lst;
        }




        /// <summary>
        /// Converts text to standard form
        /// </summary>
        /// <param name="fileContent">text content</param>
        /// <returns></returns>
        internal static string OptimizeText(string fileContent)
        {
            if (String.IsNullOrEmpty(fileContent = fileContent.Trim())) { return String.Empty; }

            var paragraphs = Regex.Split(fileContent, Const.PARAGRAPH_DELIMITER)
                .Where(p => !string.IsNullOrEmpty(p = p.Trim()))
                .Select(p => p = OptimizePunctuation(Regex.Replace(p, @"\s+", " ").Trim()));

            var str = paragraphs.Aggregate(new StringBuilder(), (s, item) => s.Append(item + "\r\n"));
            return str.ToString();
        }


        /// <summary>
        /// Get Words from text (example: World
        /// </summary>
        /// <param name="text">text content</param>
        /// <returns>string list words</returns>
        internal static IEnumerable<string> OptimizeWords(IEnumerable<string> words)
        {
            var result = new List<string>();
            words.AsParallel()
                .ForAll(w =>
                {
                    w = GetWordContent(w);
                    if (!string.IsNullOrWhiteSpace(w.Trim()))
                    {
                        // capital letter
                        w = IsUpperCase(w) ? w
                                           : System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(w.ToLower());
                        result.Add(w);
                    }
                });

            return result.Distinct().Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x);
        }


        #endregion // TEXT_CONVERTERS




        #region TEXT_PARSER
        //##############################################################################################################################

        /// <summary>
        /// Split text to word content
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        internal static async Task<List<string>> SplitToWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return null; }
            return await Task.Run(() => Regex.Split(text, Const.WORD_DELIMITER).ToList());
        }


        #endregion // TEXT_PARSER




        #region PUNCTUATION
       

        /// <summary>
        /// Optimize text content punctuation
        /// </summary>
        /// <param name="value">text content</param>
        /// <returns>optimize text</returns>
        internal static string OptimizePunctuation(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) { return string.Empty; }

            var regPattern = string.Empty;
            string strTmp = value.Trim();
            foreach (var item in Const.PUNCTUATION_MARKS)
            {
                regPattern = @"\s+" + Regex.Escape(item.ToString()) + @"\s+";
                strTmp = Regex.Replace(strTmp, regPattern.ToString(), item.ToString() + " ").Trim();
                regPattern = @"\s+" + Regex.Escape(item.ToString()) + @"\s*$";
                strTmp = Regex.Replace(strTmp, regPattern.ToString(), item.ToString() + " ").Trim();
            }
            return strTmp;
        }


        /// <summary>
        /// Delete non-alphanumeric characters at the beginning and end of a line (TEMPORARY VARIANT)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static string GetWordContent(string item)
        {
            if (string.IsNullOrWhiteSpace(item)) { return string.Empty; }

            Regex reg = new Regex("[^a-zA-Zа-яА-ЯёЁ0-9]+$");    
            var result = reg.Replace(item, "");
            reg = new Regex("^[^a-zA-Zа-яА-ЯёЁ0-9]+");
            result = reg.Replace(result, "");
            return result;
        }



        #endregion



        #region CHECK
        //##############################################################################################################################

        /// <summary>
        /// Check list to empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <returns></returns>
        internal static bool IsEmpty<T>(IEnumerable<T> lst)
        {
            return (lst == null || lst.Count() < 1) ? true : false;
        }


        /// <summary>
        /// Check word to UpperCase
        /// </summary>
        /// <param name="str">string content</param>
        /// <returns>formatted string</returns>
        internal static bool IsUpperCase(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return false; }
            if (str.Length < 2) { return false; }
            if (str.ToUpper() == str) { return true; }
            return false;
        }


        #endregion


    }
}
