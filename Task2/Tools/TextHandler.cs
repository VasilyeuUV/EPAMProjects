using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Task2.Tools
{
    internal static class TextHandler
    {

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

        #endregion // TEXT_CONVERTERS






        #region PUNCTUATION
        //##############################################################################################################################

        /// <summary>
        /// Optimize text content punctuation
        /// </summary>
        /// <param name="value">text content</param>
        /// <returns>optimize text</returns>
        internal static string OptimizePunctuation(string value)
        {
            if (String.IsNullOrWhiteSpace(value)) { return String.Empty; }

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


        #endregion



    }
}
