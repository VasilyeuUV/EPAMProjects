using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Task2.Models;

namespace Task2.Tools
{
    internal static class TextHandler
    {

        #region TEXTMODEL_OPERATIONS

        /// <summary>
        /// Check TextModel object for null or empty
        /// </summary>
        /// <returns></returns>
        internal static bool IsEmptyTextModel(TextModel tm)
        {
            return tm == null || tm.Paragraphs == null
                              || tm.Paragraphs.Count() < 1;
        }


        #endregion





        #region TEXT_CONVERTERS
        //##############################################################################################################################

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
