using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Task2.Tools
{
    /// <summary>
    /// Constants for TextModel
    /// </summary>
    internal static class Const
    {
        internal static readonly char[] PUNCTUATION_MARKS = { '.', ',', '!', '?', ':', ';' };


        #region DELIMITERS
        //###############################################################################################################################



        internal const string PARAGRAPH_DELIMITER = @"(\r\n?|\n){2}";
        internal const string WORD_DELIMITER = @"(\s+)";
        internal static readonly string SENTENCE_DELIMITER = GetSentenceDelimiter(PUNCTUATION_MARKS);

        /// <summary>
        /// Sentence delimiter instalation
        /// </summary>
        /// <param name="punctuations">array char of punctuation mark</param>
        /// <returns>string delimiter for Regexp</returns>
        private static string GetSentenceDelimiter(char[] punctuations)
        {
            return punctuations.Where(x => x != ','
                                        && x != ':'
                                        && x != ';')
                               .Aggregate(new StringBuilder(),
                                         (s, item) => s.Append("(" + Regex.Escape(item.ToString()) + @")\s+|"),
                                         s => s.Remove(s.Length - 1, 1))
                               .ToString();

        }

        #endregion




        #region ENUMS
        //###############################################################################################################################


        internal enum Task
        {
            GetTextModel = 1,
            SortTextBySentence,
            ViewConcordance,

        }

        /// <summary>
        /// Page Parameters
        /// </summary>
        internal enum PageParameter
        {
            LinesPageCount = 1,
            CharsLineCount,
        }



        #endregion





        #region PAGE_STANDARTS
        //###############################################################################################################################


        /// <summary>
        /// A4 default page standart
        /// </summary>
        internal static IReadOnlyDictionary<PageParameter, int> Page_A4Standart { get; }
            = new Dictionary<PageParameter, int>()
            {
                [PageParameter.LinesPageCount] = 28,
                [PageParameter.CharsLineCount] = 65,
            };

        /// <summary>
        /// A4 default page standart
        /// </summary>
        internal static IReadOnlyDictionary<PageParameter, int> Page_A5 { get; }
            = new Dictionary<PageParameter, int>()
            {
                [PageParameter.LinesPageCount] = 14,
                [PageParameter.CharsLineCount] = 32,
            };


        #endregion




    }
}
