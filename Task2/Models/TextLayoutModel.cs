using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Task2.Tools;

namespace Task2.Models
{
    public sealed class TextLayoutModel
    {

        /// <summary>
        /// Text content Page view
        /// </summary>
        public IDictionary<int, IDictionary<int, string>> Pages { get; private set; }

        private TextLayoutModel(string content)
        {
            this.Pages = ConvertToPages(content);
        }

        /// <summary>
        /// Create new TextLayoutModel object
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static TextLayoutModel NewInstance(string content)
        {
            return string.IsNullOrWhiteSpace(content) ? null : new TextLayoutModel(content);
        }


        /// <summary>
        /// Convert text to Page
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static IDictionary<int, IDictionary<int, string>> ConvertToPages(string content,
                                                                                 IReadOnlyDictionary<Const.PageParameter, int> pageParam = null)
        {
            if (string.IsNullOrWhiteSpace(content)) { return null; }
            if (pageParam == null) { pageParam = Const.Page_A4Standart; }

            var partsLst = Regex.Split(content, Const.WORD_DELIMITER);
            if (partsLst == null || partsLst.Count() < 1) { return null; }

            int nLine = 0;
            var lines = new Dictionary<int, string>();
            int nPage = 0;
            var pages = new Dictionary<int, IDictionary<int, string>>();
            StringBuilder line = new StringBuilder();
            foreach (var item in partsLst)
            {
                if (item == "\r\n" || line.Length + item.Length > pageParam[Const.PageParameter.CharsLineCount])    // A4 - [PageParameter.CharsLineCount] = 60,
                {
                    lines.Add(++nLine, line.ToString());
                    line.Clear();
                }
                if (lines.Count + 1 > pageParam[Const.PageParameter.LinesPageCount])    // A4 - [PageParameter.LinesPageCount] = 30,
                {
                    pages.Add(++nPage, lines);
                    lines = new Dictionary<int, string>();
                }
                line.Append(item);
            }
            if (line.ToString().Trim().Length > 0) { lines.Add(++nLine, line.ToString()); }
            if (lines.Count > 0) { pages.Add(++nPage, lines); }

            return pages;
        }


    }
}
