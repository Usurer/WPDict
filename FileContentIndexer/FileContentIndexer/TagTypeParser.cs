﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileContentIndexer
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
