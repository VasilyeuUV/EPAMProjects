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

            List<string> words = await Task.Run(() =>
                                 Regex.Split(text, Const.WORD_DELIMITER)
                                      .Where(s => (s = s.Trim()) != string.Empty)
                                      .ToList());

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



        #region TEXTMODEL_OPERATIONS


        /// <summary>
        /// Create list concordance words from TextModelobject
        /// </summary>
        /// <param name="paragraphs"></param>
        /// <returns></returns>
        internal static IEnumerable<ConcordanceItemModel> GetTMWordParts(IEnumerable<ParagraphModel> paragraphs)
        {
            List<WordModel> lst = new List<WordModel>();


            // ТУТ ИНОГДА ВЫДАЕТ ОШИБКУ О НЕДОСТАТОЧНОСТИ РАЗМЕРА МАССИВА (РАЗОБРАТЬСЯ)
            paragraphs.AsParallel()
                      .ForAll(p => lst.AddRange(p.Sentences.SelectMany(w => w.Words).ToList()));


            return lst.Where(w => w != null)
                      .GroupBy(w => w.Content)
                      .Select(x => ConcordanceItemModel.NewInstance(x))
                      .Where(x => x != null)
                      .OrderBy(x => x.Word)
                      .ThenBy(x => x.Positions.OrderBy(p => p.ParagraphNumber)
                                              .ThenBy(p => p.SentenceNumber)
                                              .ThenBy(p => p.WordNumber))
                      .ToList();
        }


        /// <summary>
        /// Get all words, symbols, etc.
        /// </summary>
        /// <param name="textModel"></param>
        /// <returns></returns>
        internal static Task<IEnumerable<string>> GetTMWordParts(TextModel textModel)
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
        /// Convert IContentable list to needed class 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        internal static List<T> ConvertIContentabletToList<T>(IEnumerable<IContentable> collection)
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
        //##############################################################################################################################

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
