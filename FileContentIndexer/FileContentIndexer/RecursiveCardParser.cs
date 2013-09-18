using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileContentIndexer
{
    public static class RecursiveCardParser
    {
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
                if (requsiveSearchResult == null)
                {
                    return null;
                }
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
                    //result.Add(block);
                    Console.Write(block);
                    break;
                }
                // Adding a part of the string without tags into a result.
                //result.Add(block.Substring(0, openingTag.StartsAt));
                if (!string.IsNullOrWhiteSpace(block.Substring(0, openingTag.StartsAt)))
                {
                    Console.Write(block.Substring(0, openingTag.StartsAt));
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

    public class Tag
    {
        public string TagName { get; set; }
    }
}
