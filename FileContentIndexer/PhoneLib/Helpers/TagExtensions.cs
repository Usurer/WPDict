using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhoneLib.Helpers
{
    public static class TagExtensions
    {
        public static string ConvertToNoTagsString(this IEnumerable<TagContent> tagContent)
        {
            var result = string.Empty;
            foreach (var content in tagContent)
            {
                if (content.ContentType == TagContentTypes.Tag)
                {
                    result = result + ConvertToNoTagsString(content);
                }
                else
                {
                    result = result + content.Value;
                }
            }
            return result;
        }

        static string ConvertToNoTagsString(this TagContent tagContent)
        {
            var result = string.Empty;
            if (tagContent.ContentType == TagContentTypes.String)
            {
                return tagContent.Value.ToString();
            }

            var content = (tagContent.Value as Tag).TagContent;

            result = result + " " + ConvertToNoTagsString(content);
            return result;
        }

        public static string ConvertToStyledStrings(this IEnumerable<TagContent> tagContent)
        {
            var result = string.Empty;
            foreach (var content in tagContent)
            {
                if (content.ContentType == TagContentTypes.Tag)
                {
                    //result = result + ConvertToNoTagsString(content);
                    var formatString = TagTypeParser.GetTagFormatString(content.Value as Tag);
                    var formattedContent = string.Format(formatString, ConvertToStyledString(content));
                    result = string.Format("{0}{1}", result, formattedContent);
                }
                else
                {
                    result = result + content.Value.ToString().ConvertExtendedAscii();
                }
            }
            return result;
        }

        public static string ConvertToStyledString(this TagContent tagContent)
        {
            var result = string.Empty;
            if (tagContent.ContentType == TagContentTypes.String)
            {
                return tagContent.Value.ToString().ConvertExtendedAscii();
            }

            var content = (tagContent.Value as Tag).TagContent;

            result = result + " " + ConvertToStyledStrings(content);
            return result;
        }
    }
}
