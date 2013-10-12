using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileContentIndexer
{
    public enum TagTypes
    {
        Bold,
        Italics,
        Underline,
        Coloured,
        FullTrnModePart,
        Paragraph,
        TranslationZone,
        ExamplesZone,
        CommentsZone,
        Multimeida,
        LinkUrl,
        NotIndexedText,
        Label,
        StressedVowel,
        Language,
        DictCardhyperlink,
        Subscript,
        Superscript,
        Undefined
    }

    public static class DslTags
    {
        public static Dictionary<string, TagTypes> Tags = new Dictionary<string, TagTypes>()
            {
                {"b", TagTypes.Bold}, {"i", TagTypes.Italics}, {"u", TagTypes.Underline}, {"c", TagTypes.Coloured},
                {"*", TagTypes.FullTrnModePart}, {"m", TagTypes.Paragraph}, {"trn", TagTypes.TranslationZone}, {"ex", TagTypes.ExamplesZone},
                {"com", TagTypes.CommentsZone}, {"s", TagTypes.Multimeida}, {"url", TagTypes.LinkUrl}, {"!trs", TagTypes.NotIndexedText},
                {"p", TagTypes.Label}, {"'", TagTypes.StressedVowel}, {"lang", TagTypes.Language}, {"ref", TagTypes.DictCardhyperlink},
                {"sub", TagTypes.Subscript}, {"sup", TagTypes.Superscript}
            };
    }
}