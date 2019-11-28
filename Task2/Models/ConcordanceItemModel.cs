using System.Collections.Generic;
using System.Linq;

namespace Task2.Models
{
    internal class ConcordanceItemModel
    {
        /*
         * Абзац (список номер)
         * Предложение (список номер)
         * позиция (список номеров)
         */

        internal struct Position
        {
            internal int ParagraphNumber { get; private set; }
            internal int SentenceNumber { get; private set; }
            internal int WordNumber { get; private set; }

            public Position(int p = 0, int s = 0, int w = 0)
            {
                this.ParagraphNumber = p;
                this.SentenceNumber = s;
                this.WordNumber = w;
            }
        }

        /// <summary>
        /// Concordance item word 
        /// </summary>
        internal string Word { get; private set; }

        /// <summary>
        /// Word coordinates in text content
        /// </summary>
        public ICollection<Position> Positions { get; private set; }


        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="word">word content</param>
        internal ConcordanceItemModel(string word)
        {
            this.Word = word;
            this.Positions = new List<Position>();
        }

        /// <summary>
        /// Create a new ConcordanceItemModel object from a list of grouped words
        /// </summary>
        /// <param name="group">IGrouping<string, WordModel></param>
        /// <returns>new ConcordanceItemModel object</returns>
        internal static ConcordanceItemModel NewInstance(IGrouping<string, WordModel> group)
        {
            if (group == null || string.IsNullOrWhiteSpace(group.Key)) { return null; }

            var cItem = new ConcordanceItemModel(group.Key);
            foreach (var item in group)
            {
                cItem.Positions.Add(new Position(item.ParagraphNumber, item.SentenseNumber, item.Number));
            }
            return cItem;
        }




        internal void AddPosition(WordModel item)
        {
            //if (item.)
            //{

            //}

            //foreach (int n in item.Numbers)
            //{
            //    this.Positions.Add(new Position(item.ParagraphNumber, item.SentenseNumber, n));
            //}
        }
    }
}
