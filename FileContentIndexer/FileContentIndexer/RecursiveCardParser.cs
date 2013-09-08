using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileContentIndexer
{
    public static class RecursiveCardParser
    {
        public static IEnumerable<WordCardValue> ParseBlock(string block)
        { 
            var tagStarts = block.IndexOf("[");
            if(tagStarts == -1)
            {
                return new List<WordCardValue>() { new WordCardValue(WordCardValue.ValueTypes.Text, block) };
            }
            // Check if bracket is screened, so it's not a tag
            if(tagStarts > 0 && block.Substring(tagStarts - 1, 1) == "\\")
            {
                var pseudoTagEnds = block.IndexOf("\\]", tagStarts);
                var pseudoTagValue = block.Substring(block.IndexOf("\\["), block.IndexOf("\\]") - block.IndexOf("\\[") + 2); // It's better to remove brackets here and add 'em only during UI render for a transcription value type.
                return new List<WordCardValue>() { new WordCardValue(WordCardValue.ValueTypes.Text, pseudoTagValue, ParseBlock(block.Substring(block.IndexOf("\\]") + 2))) };
            }

            var tagEnds = block.IndexOf("]", tagStarts);
            if(tagEnds == -1)
            {
                return new List<WordCardValue>() {new WordCardValue(WordCardValue.ValueTypes.Text, block)};
            }
            var tag = block.Substring(tagStarts, tagEnds - tagStarts + 1);
            if(!TagsAnalyser.IsTag(tag))
            {
                return new List<WordCardValue>() { new WordCardValue(WordCardValue.ValueTypes.Text, block) };
            }
            var closingTag = TagsAnalyser.GetMatchingClosingTag(tag);
            var closingTagLocation = block.IndexOf(closingTag);
            var tagContent = block.Substring(tagEnds + 1, closingTagLocation - tagEnds - 1);
            var blockValue = new WordCardValue(WordCardValue.ValueTypes.Block, tagContent, ParseBlock(tagContent));
            var otherRawData = block.Substring(closingTagLocation + closingTag.Length);
            var otherBlocks = new WordCardValue(WordCardValue.ValueTypes.Block, otherRawData, ParseBlock(otherRawData));
            return new List<WordCardValue>() {blockValue, otherBlocks};
        }
        
    }
}
