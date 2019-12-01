using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Task2.Interfaces;
using Task2.Tools;

namespace Task2.Models
{
    internal sealed class ParagraphModel : IContentable
    {
        private IEnumerable<SentenceModel> _sentences = null;

        /// <summary>
        /// Paragraph number
        /// </summary>
        internal int Number { get; private set; }

        /// <summary>
        /// Paragraph content
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Sentences Words list
        /// </summary>
        internal IEnumerable<SentenceModel> Sentences => _sentences == null ? _sentences = SetSentences(Content) : _sentences;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="number">paragraph number in text in order</param>
        /// <param name="paragraph">paragraph content</param>
        private ParagraphModel(string paragraph, int number)
        {
            this.Number = number;
            this.Content = paragraph.Trim();
        }

        /// <summary>
        /// New ParagraphModel
        /// </summary>
        /// <param name="paragraph">paragraph content</param>
        /// <param name="number">paragraph number</param>
        /// <returns>new ParagraphModel object</returns>
        internal static ParagraphModel NewInstance(string paragraph, int number)
        {
            return (string.IsNullOrWhiteSpace(paragraph) || number < 1)
                   ? null
                   : new ParagraphModel(paragraph, number);
        }

        /// <summary>
        /// Get paragraph content converted to sentences
        /// </summary>
        /// <param name="paragraph">string paragraph content</param>
        /// <returns>list converted sentences</returns>
        private IEnumerable<SentenceModel> SetSentences(string paragraph)
        {
            if (string.IsNullOrWhiteSpace(paragraph)) { return null; }

            var arrTmp = Regex.Split(paragraph, Const.SENTENCE_DELIMITER)
                                   .Where(s => s.Trim().Length > 0)
                                   .ToList();
            if (arrTmp == null || arrTmp.Count() < 1) { return null; }


            List<string> result = new List<string>();
            foreach (var item in arrTmp)
            {
                if (item.Length == 1 && char.IsPunctuation(item[0]))
                {
                    if (result.Count() < 1) { result.Add(string.Empty); }
                    string s = result.Last();
                    s += item;
                    result.Remove(result.Last());
                    result.Add(s);
                    continue;
                }
                result.Add(item);
            }

            return result.Select((x, n) => SentenceModel.NewInstance(this.Number, ++n, x))
                         .ToList();

            //return Regex.Split(paragraph, Const.SENTENCE_DELIMITER)
            //            .Where(s => s.Trim().Length > 0)
            //            .Select((x, n) => SentenceModel.NewInstance(this.Number, ++n, x))
            //            .ToList();
        }


        #region CONVERTERS
        //##############################################################################################################################


        /// <summary>
        /// Get all words, punctuations mark and other symbols from this paragraph
        /// </summary>
        /// <returns>WordPartModel list</returns>
        internal IEnumerable<WordPartModel> GetWords(int nSentence = 0)
        {
            var result = this.Sentences.SelectMany(s => s.Words.SelectMany(wp => wp.WordPartsModel));
            if (nSentence > 0) { result = result.Where(s => s.SentenseNumber == nSentence); }
            return result;
        }


        #endregion CONVERTERS




    }
}
