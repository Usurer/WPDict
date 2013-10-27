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

        public static string GetTagFormatString(Tag tag)
        {
            var tagType = GetTagTypeFromName(tag.TagName);
            var opening = string.Empty;
            var closing = string.Empty;

            switch (tagType)
            {
                case TagTypes.Paragraph:
                    opening = string.Format("<div class='{0}'>", tag.TagName);
                    closing = "</div>";
                    break;
                case TagTypes.Italics:
                    opening = string.Format("<span class='italics'>");
                    closing = "</span>";
                    break;
                case TagTypes.Underline:
                    opening = string.Format("<span class='underline'>");
                    closing = "</span>";
                    break;
                case TagTypes.Bold:
                    opening = string.Format("<span class='bold'>");
                    closing = "</span>";
                    break;
                case TagTypes.Coloured:
                    opening = string.Format("<span class='coloured'>");
                    closing = "</span>";
                    break;
                case TagTypes.TranslationZone:
                    opening = string.Format("<div class='translation'>");
                    closing = "</div>";
                    break;
                default:
                    break;
            }
            return string.Format("{0}{{0}}{1}", opening, closing);
        }
    }
}
