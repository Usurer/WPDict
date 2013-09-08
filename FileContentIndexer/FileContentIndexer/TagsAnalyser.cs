using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileContentIndexer
{
    public static class TagsAnalyser
    {
        public static IEnumerable<string> Tags = new List<string>()
                                                       {
                                                           "com"
                                                           ,"i"
                                                           ,"b"
                                                           ,"c"
                                                           ,"p"
                                                           ,"trn"
                                                           ,"m"
                                                           ,"m1"
                                                           ,"m2"
                                                           ,"m3"
                                                           ,"ex"
                                                           ,"lang"
                                                           ,"*"
                                                       };

        public static string GetMatchingClosingTag(string tag)
        {
            var hasBrackets = tag.StartsWith("[");
            if (!tag.StartsWith("[m") && !tag.StartsWith("m"))
            {
                if (tag.IndexOf(" ") != -1)
                {
                    tag = tag.Substring(0, tag.IndexOf(" "));
                }
                return hasBrackets ? "[/" + tag.Substring(1) : "/" + tag;
            }

            return hasBrackets ? "[/m]" : "m";
        }

        public static bool IsTag(string possibleTag)
        {
            if(possibleTag.StartsWith("["))
            {
                possibleTag = possibleTag.Substring(1);
            }
            if(possibleTag.EndsWith("]"))
            {
                possibleTag = possibleTag.Substring(0, possibleTag.Length - 1);
            }
            return Tags.Any(t => t.StartsWith(possibleTag, StringComparison.InvariantCultureIgnoreCase))
                || Tags.Any(t => possibleTag.StartsWith(t, StringComparison.InvariantCultureIgnoreCase)); //Equals?
        }
    }
}
