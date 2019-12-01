using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Task2.Interfaces;
using Task2.Tools;

namespace Task2.Models
{
    internal sealed class SentenceModel : IContentable
    {
        private IEnumerable<WordModel> _words = null;

        /// <summary>
        /// Paragraph number
        /// </summary>
        internal int ParagraphNumber { get; private set; }

        /// <summary>
        /// This sentence number in paragraph
        /// </summary>
        internal int Number { get; private set; }

        /// <summary>
        /// Sentence content  (ПОКА ДЛЯ СТРОК)
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Word content of sentence
        /// </summary>
        internal IEnumerable<WordModel> Words => _words == null ? _words = SetWords(Content) : _words;


        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="paragraphNumber">paragraph number</param>
        /// <param name="sentenceNumber">sentence number in paragraph in order</param>
        /// <param name="sentence">sentence context</param>
        private SentenceModel(int paragraphNumber, int sentenceNumber, string sentence)
        {
            this.ParagraphNumber = paragraphNumber;
            this.Number = sentenceNumber;
            this.Content = sentence;
        }

        /// <summary>
        /// New Sentence Model
        /// </summary>
        /// <param name="paragraphNumber">paragraph number</param>
        /// <param name="number">sentance number</param>
        /// <param name="sentence">sentence content</param>
        /// <returns>new SentenceModel object</returns>
        internal static SentenceModel NewInstance(int paragraphNumber, int number, string sentence)
        {
            return (string.IsNullOrWhiteSpace(sentence) || number < 1 || paragraphNumber < 1)
                   ? null
                   : new SentenceModel(paragraphNumber, number, sentence);
        }

        /// <summary>
        /// Get sentence content converted to words
        /// </summary>
        /// <param name="sentence">this sentence content</param>
        /// <returns>words list</returns>
        private IEnumerable<WordModel> SetWords(string sentence)
        {
            if (string.IsNullOrWhiteSpace(sentence)) { return null; }

            return Regex.Split(sentence, Const.WORD_DELIMITER)
                        .Where(s => s.Trim().Length > 0)
                        .Select((word, n) => WordModel.NewInstance(word, ++n, this.ParagraphNumber, this.Number))
                        .ToList();
        }


        #region CONVERTERS
        //##############################################################################################################################


        /// <summary>
        /// Get all words, punctuations mark and other symbols from this sentence
        /// </summary>
        /// <returns>WordPartModel list</returns>
        internal IEnumerable<WordPartModel> GetWords()
        {
            return this.Words.SelectMany(wp => wp.WordPartsModel);
        }

        #endregion CONVERTERS


    }
}




