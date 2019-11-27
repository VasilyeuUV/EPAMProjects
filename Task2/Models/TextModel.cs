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
        internal IEnumerable<ParagraphModel> Paragraphs => _paragraphs == null ? _paragraphs = GetParagraphs(Content) : _paragraphs;


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
        internal static TextModel NewInstance(string fileContent)
        {
            return string.IsNullOrWhiteSpace(fileContent) ? null : new TextModel(fileContent);
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
    }
}










