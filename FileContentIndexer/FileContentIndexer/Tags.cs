using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileContentIndexer
{
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

        public List<TagContent> TagContent { get; set; }

        public  bool Equals(Tag tag)
        {
            var result = tag != null
                   && TagName.Equals(tag.TagName)
                   && TagContent.Count == tag.TagContent.Count;
            if (!result)
            {
                return false;
            }
            var contentAsArray = TagContent.ToArray();
            var otherContentAsArray = tag.TagContent.ToArray();
            return !contentAsArray.Where((t, i) => !t.Equals(otherContentAsArray[i])).Any();
        }
    }

    public enum TagContentTypes
    {
        Tag,
        String
    }

    public class TagContent
    {
        public TagContentTypes ContentType { get; set; }

        public object Value { get; set; }

        public TagContent(string stringValue)
        {
            ContentType = TagContentTypes.String;
            Value = stringValue;
        }

        public TagContent(Tag tagValue)
        {
            ContentType = TagContentTypes.Tag;
            Value = tagValue;
        }

        public bool Equals(TagContent comparedContent)
        {
            return comparedContent != null 
                && ContentType == comparedContent.ContentType 
                && (ContentType == TagContentTypes.String 
                    ? (Value as string).Equals(comparedContent.Value as string) 
                    : (Value as Tag).Equals(comparedContent.Value as Tag));
        }
    }
}
