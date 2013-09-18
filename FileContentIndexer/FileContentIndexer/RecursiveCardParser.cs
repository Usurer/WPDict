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
        
        //String is for tag name, int is for it's start position within a block, bool is for is it a closing tag or not.
        //Tag is taken with brackets.
        public static TagPart GetTagAndPosition(string block)
        {
            var tagStarts = block.IndexOf("[");
            if(tagStarts == -1)
            {
                return null;
            }

            // Checking if this is not a screened bracket like /[
            if(tagStarts > 0 && block[tagStarts - 1] == '\\')
            {
                var requsiveSearchResult = GetTagAndPosition(block.Substring(tagStarts + 1));
                return new TagPart(requsiveSearchResult.PartValue, requsiveSearchResult.StartsAt + tagStarts + 1, requsiveSearchResult.IsClosingPartOfTag);
            }
            
            var tagEnds = block.IndexOf("]", tagStarts);
            while(tagEnds != -1)
            {
                if (block[tagEnds - 1] == '/')
                {
                    tagEnds = block.IndexOf("]", tagEnds);
                }
                else
                {
                    var isClosingTag = block.Substring(tagStarts + 1, 1) == "/";
                    var extractTagValueFrom = tagStarts + (isClosingTag ? "[/".Length : "[".Length);
                    var tag = block.Substring(extractTagValueFrom, tagEnds - extractTagValueFrom);
                    return new TagPart(tag, tagStarts, isClosingTag);
                }
            }
            throw new Exception("Missing matching closing bracket");
        }

        public static List<object> DivideSiblingTags(string block)
        {
            var result = new List<object>();
            var currentPosition = 0;
            while(true)
            {
                block = block.Substring(currentPosition);
                var openingTag = GetTagAndPosition(block);
                if (openingTag == null)
                {
                    // No tags found.
                    break;
                }
                TagPart closingTag;
                var tagOpeningEndsAt = openingTag.PartValue.Length + openingTag.StartsAt + "]".Length;
                var openedTags = 0;
                var closedTags = 0;
                var lookupPosition = tagOpeningEndsAt;
                while(true)
                {
                    closingTag = GetTagAndPosition(block.Substring(lookupPosition));
                    if(closingTag == null)
                    {
                        break;
                    }

                    closingTag.StartsAt = closingTag.StartsAt + lookupPosition;

                    if (!closingTag.IsClosingPartOfTag)
                    {
                        openedTags++;
                    }
                    else
                    {
                        closedTags++;
                        if (closedTags > openedTags)
                        {
                            break;
                        }
                    }
                    
                    lookupPosition = closingTag.StartsAt + closingTag.PartValue.Length + (closingTag.IsClosingPartOfTag ? "[/]".Length : "[]".Length);
                }
                
                var tagContentRaw = block.Substring(tagOpeningEndsAt + 1, closingTag.StartsAt - tagOpeningEndsAt - 1);
                var tagContent = DivideSiblingTags(tagContentRaw);
                result.Add(new List<object>() {openingTag, closingTag, tagContent});
                currentPosition = closingTag.PartValue.Length + closingTag.StartsAt + "[/]".Length;
            }

            return result;
        }
    }

    /// <summary>
    /// Tag part is an opening or closing part of a tag like [tag] or [/tag]
    /// </summary>
    public class TagPart
    {
        public int StartsAt { get; set; }

        public string PartValue { get; set; }

        public bool IsClosingPartOfTag { get; set; }

        public TagPart()
        {
        }

        public TagPart(string val, int partStart, bool isClosingTag)
        {
            StartsAt = partStart;
            PartValue = val;
            IsClosingPartOfTag = isClosingTag;
        }

    }
}
