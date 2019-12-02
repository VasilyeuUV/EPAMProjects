using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task2.Interfaces;

namespace Task2.Models
{
    internal sealed class WordPartModel : IContentable
    {
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
        /// CTOR
        /// </summary>
        /// <param name="word">word from sentence</param>
        /// <param name="numbers">word places in the sentense</param>
        private WordPartModel(string word, int position, int paragraphNumber, int sentenseNumber)
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
        internal static WordPartModel NewInstance(string word, int number, int paragraphNumber, int sentenceNumber)
        {
            if (string.IsNullOrEmpty(word)
                || number < 1
                || paragraphNumber < 1
                || sentenceNumber < 1)
            {
                return null;
            }
            return new WordPartModel(word, number, paragraphNumber, sentenceNumber);
        }

    }
}

