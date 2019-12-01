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

        private TextLayoutModel(string content, IReadOnlyDictionary<Const.PageParameter, int> pageStandart)
        {
            this.Pages = ConvertToPages(content, pageStandart);
        }

        /// <summary>
        /// Create new TextLayoutModel object
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        internal static TextLayoutModel NewInstance(string content, IReadOnlyDictionary<Const.PageParameter, int> pageStandart = null)
        {
            if (pageStandart == null) { pageStandart = Const.Page_A4Standart; }
            return string.IsNullOrWhiteSpace(content) ? null : new TextLayoutModel(content, pageStandart);
        }


        #region CONVERTERS
        //##############################################################################################################################


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
                if (item == Const.NEW_PARAGRAPH || line.Length + item.Length > pageParam[Const.PageParameter.CharsLineCount])   
                {
                    lines.Add(++nLine, line.ToString());
                    line.Clear();
                }
                if (lines.Count + 1 > pageParam[Const.PageParameter.LinesPageCount])   
                {
                    pages.Add(++nPage, lines);
                    lines = new Dictionary<int, string>();
                }
                if (item != Const.NEW_PARAGRAPH)
                {
                    line.Append(item);
                }                
            }
            if (line.ToString().Trim().Length > 0) { lines.Add(++nLine, line.ToString()); }
            if (lines.Count > 0) { pages.Add(++nPage, lines); }

            return pages;
        }



        /// <summary>
        /// Make string from TextLayoutModel object
        /// </summary>
        /// <param name="book">TextLayoutModel object</param>
        /// <returns>string text</returns>
        internal string ConvertToText()
        {
            if (Pages == null || this.Pages.Count() < 1) { return string.Empty; }
            IEnumerable<string> lines = this.ConvertToLines();
            return TextHandler.GetText(lines);
        }


        internal IEnumerable<string> ConvertToLines()
        {
            if (this.Pages == null || this.Pages.Count() < 1) { return null; }
            return this.Pages.SelectMany(v => v.Value.Select(x => x.Value));
        }


        //    //TextModel tm = null;
        //    //if (book == null) { tm = _textModel; }
        //    //else
        //    //{
        //    //    StringBuilder s = new StringBuilder();
        //    //    book.Pages.ToList().ForEach(x => x.Value.ToList().ForEach(p => s.Append(p.Value)));
        //    //    string bookLines = s.ToString();
        //    //    tm = TextModel.NewInstance(bookLines);
        //    //}

        //    //var lines = tm.Paragraphs.Select(x => x.Content).ToList();

        //    // Слово(№ строки, количество слов в строке)



        #endregion CONVERTERS


    }
}
