using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task2.Tools;

namespace Task2.Models
{
    internal class ConcordanceModel
    {
        /// <summary>
        /// Concordance words devided by page
        /// </summary>
        internal IDictionary<string, IDictionary<int, int>> Words { get; private set;}


        /// <summary>
        /// CTOR
        /// </summary>
        private ConcordanceModel()
        {
            this.Words = new SortedDictionary<string, IDictionary<int, int>>();
        }

                                    


        /// <summary>
        /// Create new ConcordanseModel object
        /// </summary>
        /// <param name="lines">text lines list</param>
        /// <returns>new ConcordanceModel object or null</returns>
        internal static ConcordanceModel CreateConcordance(IEnumerable<string> lines = null)
        {
            if (lines == null) { return null; }

            ConcordanceModel result = new ConcordanceModel();
                        
            int nLine = 0;
            foreach (var line in lines)
            {
                ++nLine;
                var words = TextHandler.ParseTextToWords(line)/*.OrderByDescending(x => x.Length)*/;   // массив слов строки
                var uWords = words.Distinct();

                foreach (var word in uWords)
                {
                    int wordCount = words.Where(x => x == word).Count();
                    if (!result.Words.ContainsKey(word))
                    {
                        IDictionary<int, int> d = new Dictionary<int, int>() { [nLine] = wordCount };
                        result.Words.Add(word, d);
                    }
                    else { result.Words[word].Add(nLine, wordCount); }
                }
            }
            return result;
        }



        /// <summary>
        /// The ConcordanceModel object to string converter is limited by the Const.PARAGRAPH_VIEW_LIMIT parameter
        /// </summary>
        /// <returns>string representation of an object</returns>
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            //List<string> lst = new List<string>();

            string firstLetter = "";
            int viewCount = 1;
            foreach (var item in this.Words)
            {
                string word = item.Key;
                if (firstLetter != word[0].ToString().ToUpper())
                {
                    str.Append(Const.NEW_PARAGRAPH);
                    firstLetter = word[0].ToString().ToUpper();
                    str.Append(string.Format("{0}:{1}", firstLetter, Const.NEW_PARAGRAPH));
                }

                str.Append(string.Format("- {0, -25} {1}: ", word.ToLower(), item.Value.Sum(v => v.Value)));
                foreach (var d in item.Value)
                {
                    str.Append(string.Format("{0}; ", d.Key));
                }
                str.Append(Const.NEW_PARAGRAPH);

                //lst.Add(word);

                if (++viewCount > Const.WORD_VIEW_LIMIT) { break; }
            }
            str.Append(Const.NEW_PARAGRAPH);

            string result = str.ToString().Trim();
            return string.IsNullOrWhiteSpace(result) ? "No data to display." : result;

        }






    ///// <summary>
    ///// Get Concordance
    ///// </summary>
    ///// <param name="page_A4Standart"></param>
    ///// <returns></returns>
    //private static string ConvertListToString(IEnumerable<string> lst = null)
    //{
    //    if (lst == null) { lst = _textModel.Paragraphs.Select(p => p.Content); }





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




    //    IDictionary<string, Dictionary<int, int>> words = null; // CreateConcordance(lst);

    //    StringBuilder str = new StringBuilder();
    //    string firstLetter = "";
    //    foreach (var item in words)
    //    {
    //        string word = item.Key;
    //        if (firstLetter != word[0].ToString().ToUpper())
    //        {
    //            str.Append(string.Format("\r\n"));
    //            firstLetter = word[0].ToString().ToUpper();
    //            str.Append(string.Format("{0}:\r\n", firstLetter));
    //        }
    //        str.Append(string.Format("- {0, -25} {1}: ", word/*.ToLower()*/, item.Value.Count()));
    //        foreach (var d in item.Value)
    //        {
    //            str.Append(string.Format("{0}; ", d.Key));
    //        }
    //        str.Append(string.Format("\r\n", firstLetter));
    //    }
    //    str.Append(string.Format("\r\n"));

    //    string result = str.ToString().Trim();
    //    return string.IsNullOrWhiteSpace(result) ? "Нет данных для отображения." : result;
    //}








    ///// <summary>
    ///// Create a concordance for each line of text
    ///// </summary>
    ///// <param name="lines">text lines list</param>
    ///// <returns>list of concordances for each line of text</returns>
    //private static IDictionary<string, IDictionary<int, int>> CreateConcordance(IEnumerable<string> lines = null)
    //{
    //    if (lines == null) { lines = _textModel.Paragraphs.Select(p => p.Content); }

    //    IDictionary<string, IDictionary<int, int>> result = new Dictionary<string, IDictionary<int, int>>();
    //    int nLine = 0;
    //    foreach (var line in lines)
    //    {
    //        ++nLine;
    //        var words = TextHandler.ParseTextToWords(line).OrderByDescending(x => x.Length);   // массив слов строки
    //        var uWords = words.Distinct();

    //        foreach (var word in uWords)
    //        {
    //            int wordCount = words.Where(x => x == word).Count();
    //            if (!result.ContainsKey(word))
    //            {
    //                Dictionary<int, int> d = new Dictionary<int, int>() { [nLine] = wordCount };
    //                result.Add(word, d);
    //            }
    //            else { result[word].Add(nLine, wordCount); }
    //        }
    //    }
    //    return new SortedDictionary<string, IDictionary<int, int>>(result);
    //}






    ///// <summary>
    ///// Create Concordance from string
    ///// </summary>
    ///// <param name="str">source string</param>
    ///// <returns>New ConcordanceModel or null</returns>
    //private IEnumerable<WordPartModel> CreateConcordance(string str)
    //{
    //    if (string.IsNullOrWhiteSpace(str)) { return null; }
    //    var tm = TextModel.NewInstance(str);
    //    return tm.GetWords();

    //}



    #region CONVERTERS
    //##############################################################################################################################



    #endregion CONVERTERS




}
}
