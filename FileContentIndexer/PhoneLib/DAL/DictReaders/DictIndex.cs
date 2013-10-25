using System;
using System.Collections.Generic;
using System.Linq;

namespace PhoneLib.DAL.DictReaders
{
    public class DictIndex
    {
        private Dictionary<string, FileTextBlock> Index = new Dictionary<string, FileTextBlock>();

        public bool AddWordCard(WordCard card)
        {
            if (Index.ContainsKey(card.CardKey))
            {
                return false;
            }
            
            Index.Add(card.CardKey, card.CardFileTextBlock);
            return true;
        }

        public WordCard GetWordCard(string wordBeginning, bool caseSensitive = false)
        {
            var match = Index.FirstOrDefault(card => card.Key.StartsWith(wordBeginning, caseSensitive
                                                    ? StringComparison.InvariantCulture
                                                    : StringComparison.CurrentCultureIgnoreCase));
            if (match.Key == null)
            {
                return null;
            }
            return new WordCard() {CardKey = match.Key, CardFileTextBlock = match.Value};
        }
    }
}
