using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileContentIndexer
{
    public class WordCardValue
    {
        public enum ValueTypes
        {
            Text,
            Block,
            WordType //Verb, noun, etc.
        }

        public string RawContent { get; set; }

        public IEnumerable<WordCardValue> Subcontent { get; set; }

        public ValueTypes ValueType { get; set; }

        public WordCardValue()
        {
        }

        public WordCardValue(ValueTypes type, string content)
        {
            ValueType = type;
            RawContent = content;
        }

        public WordCardValue(ValueTypes type, string content, IEnumerable<WordCardValue> subcontent)
        {
            ValueType = type;
            RawContent = content;
            Subcontent = subcontent;
        }

        public string GetValues()
        {
            var result = string.Empty;
            if (Subcontent == null || string.IsNullOrEmpty(RawContent))
            {
                return string.IsNullOrEmpty(RawContent) ? string.Empty : RawContent;
            }
            foreach (var wordCardValue in Subcontent)
            {
                if(string.IsNullOrEmpty(wordCardValue.RawContent))
                {
                    continue;
                }
                if (wordCardValue.ValueType == ValueTypes.Text)
                {
                    result = result + " " + wordCardValue.RawContent + ";";
                }
                if (wordCardValue.Subcontent != null)
                {
                    result = result + wordCardValue.GetValues();
                }
            }
            return result;
        }
    }
}
