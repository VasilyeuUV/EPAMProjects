using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Task2.Interfaces;
using Task2.Tools;

namespace Task2.Models
{
    public sealed class TextModel : IContentable
    {
        private IEnumerable<ParagraphModel> _paragraphs = null;

        /// <summary>
        /// Text optimized content
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Text paragraphs
        /// </summary>
        internal IEnumerable<ParagraphModel> Paragraphs => _paragraphs == null ? _paragraphs = GetParagraphs(Content) : _paragraphs;

        /// <summary>
        /// Text content Page view
        /// </summary>
        public IDictionary<int, IDictionary<int, string>> Pages { get; private set; }


        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="fileContent">string file content</param>
        private TextModel(string fileContent)
        {
            this.Content = fileContent;
        }

                /// <summary>
        /// Create new TextModel object
        /// </summary>
        /// <param name="fileContent">string file content</param>
        /// <returns>new TextModel object</returns>
        internal static TextModel NewInstance(string fileContent,
                                              IReadOnlyDictionary<Const.PageParameter, int> pageParam = null)
        {
            if (string.IsNullOrWhiteSpace(fileContent)) { return null; }
            if (pageParam == null) { pageParam = Const.Page_A4Standart; }

            var result = new TextModel(fileContent);
            result.ConvertToPages();

            return result; 
        }


        /// <summary>
        /// Get file content converted paragraps
        /// </summary>
        /// <param name="fileContent"> string file content</param>
        /// <returns>text paragraps</returns>
        private static IEnumerable<ParagraphModel> GetParagraphs(string fileContent)
        {
            if (string.IsNullOrEmpty(fileContent.Trim())) { return null; }

            return Regex.Split(fileContent, Const.PARAGRAPH_DELIMITER)
                   .Where(p => p.Trim() != string.Empty)
                   .Select((x, n) => ParagraphModel.NewInstance(x, ++n));

        }


        /// <summary>
        /// Convert text to Page
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        internal IDictionary<int, IDictionary<int, string>> ConvertToPages(IReadOnlyDictionary<Const.PageParameter, int> pageParam = null)
        {
            string content = this.Content;
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










