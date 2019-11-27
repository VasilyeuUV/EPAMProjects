using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Task2.Interfaces;
using Task2.Tools;

namespace Task2.Models
{
    internal sealed class WordModel : IContentable
    {
        private ICollection<string> _wordParts = null;

        /// <summary>
        /// Paragraph number
        /// </summary>
        internal int ParagraphNumber { get; private set; }

        /// <summary>
        /// Sentence number
        /// </summary>
        internal int SentenseNumber { get; private set; }

        /// <summary>
        /// Word Number in sentence content
        /// </summary>
        internal int Number { get; private set; }


        /// <summary>
        /// Word content
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Word Parts in word content
        /// </summary>
        internal ICollection<string> WordParts => _wordParts == null ? _wordParts = GetWordParts(Content) : _wordParts;


        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="word">word from sentence</param>
        /// <param name="numbers">word places in the sentense</param>
        private WordModel(string word, int position, int paragraphNumber, int sentenseNumber)
        {
            this.Content = word;
            this.Number = position;
            this.ParagraphNumber = paragraphNumber;
            this.SentenseNumber = sentenseNumber;
        }

        /// <summary>
        /// New Word Model
        /// </summary>
        /// <param name="word">word content</param>
        /// <param name="number">word number</param>
        /// <param name="paragraphNumber">paragraph number</param>
        /// <param name="sentenceNumber">sentence number</param>
        /// <returns>new WordModel object</returns>
        internal static WordModel NewInstance(string word, int number, int paragraphNumber, int sentenceNumber)
        {
            if (string.IsNullOrWhiteSpace(word)
                || number < 1
                || paragraphNumber < 1
                || sentenceNumber < 1)
            {
                return null;
            }
            return new WordModel(word, number, paragraphNumber, sentenceNumber);
        }


        /// <summary>
        /// Parse Word to word component (example: "World"! => ["], [World], ["], [!])
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private ICollection<string> GetWordParts(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) { return null; }

            string word = TextHandler.GetWordContent(content);
            string w = Regex.Replace(content, word, "1", RegexOptions.IgnoreCase);
            var result = w.ToCharArray().Select(c => c.ToString()).ToList();

            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] == "1")
                {
                    result[i] = word;
                }
            }

            return result;
        }


    }
}

