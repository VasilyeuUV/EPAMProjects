using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        internal IEnumerable<ParagraphModel> Paragraphs => _paragraphs == null ? _paragraphs = SetParagraphs(Content) : _paragraphs;


        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="fileContent">string file content</param>
        private TextModel(string fileContent)
        {
            this.Content = fileContent.Trim();
        }

        /// <summary>
        /// Create new TextModel object
        /// </summary>
        /// <param name="fileContent">string file content</param>
        /// <returns>new TextModel object</returns>
        internal static TextModel NewInstance(string fileContent)
        {
            return string.IsNullOrWhiteSpace(fileContent) ? null : new TextModel(fileContent);
        }


        /// <summary>
        /// Get file content converted paragraps
        /// </summary>
        /// <param name="fileContent"> string file content</param>
        /// <returns>text paragraps</returns>
        private static IEnumerable<ParagraphModel> SetParagraphs(string fileContent)
        {
            if (string.IsNullOrEmpty(fileContent.Trim())) { return null; }

            return Regex.Split(fileContent, Const.PARAGRAPH_DELIMITER)
                        .Where(p => p.Trim().Length > 1)
                        .Select((x, n) => ParagraphModel.NewInstance(x, ++n))
                        .ToList();
        }



        #region CONVERTERS
        //##############################################################################################################################


        /// <summary>
        /// Get sentences from this text
        /// </summary>
        /// <returns>SentenceModel objects list</returns>
        internal IEnumerable<SentenceModel> GetSentences(int nParagraph = 0, int nSentence = 0)
        {
            IEnumerable<SentenceModel> result = null;
            result = this.Paragraphs.SelectMany(p => p.Sentences).Where(s => s != null).Select(s => s);

            if (nParagraph > 0) { result = result.Where(s => s.ParagraphNumber == nParagraph); }
            if (nSentence > 0) { result = result.Where(s => s.Number == nSentence); }

            return (this.Paragraphs == null || this.Paragraphs.Count() < 1) ? null
                : result;
        }


        /// <summary>
        /// Get all words, punctuations mark and other symbols from this text
        /// </summary>
        /// <returns>WordPartModel list</returns>
        internal IEnumerable<WordPartModel> GetWords(int nParagraph = 0, int nSentence = 0)
        {
            IEnumerable<SentenceModel> sentences = GetSentences();            
            var result = sentences.SelectMany(s => s.Words.SelectMany(wp => wp.WordPartsModel));

            if (nParagraph > 0) { result = result.Where(s => s.ParagraphNumber == nParagraph); }
            if (nSentence > 0) { result = result.Where(s => s.SentenseNumber == nSentence); }
            return result;
        }


        /// <summary>
        /// Get words starting with a letter or number from this text
        /// </summary>
        /// <returns>String list</returns>
        internal IEnumerable<string> GetUniqueWords()
        {
           return (GetWords().Select(w => w.Content)).Distinct().Where(w => char.IsLetter(w[0]) || char.IsDigit(w[0]));
        }





        #endregion CONVERTERS






    }
}










