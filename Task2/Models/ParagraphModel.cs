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
        /// Paragraph Words list
        /// </summary>
        internal IEnumerable<SentenceModel> Sentences => _sentences == null ? _sentences = GetSentences(Content) : _sentences;

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
        /// Get paragraph content converted sentences
        /// </summary>
        /// <param name="paragraph">string paragraph content</param>
        /// <returns>list converted sentences</returns>
        private IEnumerable<SentenceModel> GetSentences(string paragraph)
        {
            if (string.IsNullOrWhiteSpace(paragraph)) { return null; }
            
            return Regex.Split(paragraph, Const.SENTENCE_DELIMITER)
                        .Where(s => s.Trim().Length > 1)
                        .Select((x, n) => SentenceModel.NewInstance(this.Number, x, ++n));
        }

    }
}
