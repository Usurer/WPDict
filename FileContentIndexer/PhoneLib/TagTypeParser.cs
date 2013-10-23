using System.Text.RegularExpressions;

namespace PhoneLib
{
    public static class TagTypeParser
    {
        public static TagTypes GetTagTypeFromName(string tagName)
        {
            tagName = tagName.Trim(new[] {' '});
            tagName = tagName.TrimStart(new[] { '[' });
            tagName = tagName.TrimEnd(new[] { ']' });
            var paragraph = new Regex("^m[0-9]*$");
            var matchResult = paragraph.Match(tagName);
            if(matchResult.Success)
            {
                return TagTypes.Paragraph;
            }
            if (DslTags.Tags.ContainsKey(tagName))
            {
                return DslTags.Tags[tagName];
            }
            return TagTypes.Undefined;
        }
    }
}
