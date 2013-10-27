using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PhoneLib.DAL.DictReaders;
using PhoneLib.Helpers;

namespace PhoneLib
{
    public class DictionaryDataProvider
    {
        private const bool CaseSensitiveSearch = false;

        private const string FileName = "data/dict.dsl";

        private DictIndex _index;

        // TODO: Read about proper Singleton architecture, moron!
        public DictIndex Index
        {
            get
            {
                if (_index == null)
                {
                    _index = DictIndexerDsl.GetDictionaryIndex(FileName);
                }
                return _index;
            }
            set { _index = value; }
        }

        private DictionaryDataProvider()
        {
        }

        public DictionaryDataProvider(bool lazyInit = false)
        {
            if (!lazyInit)
            {
                Index = DictIndexerDsl.GetDictionaryIndex(FileName);
            }
        }

        public DictionaryDataProvider(Stream dictFileStream, bool lazyInit = false)
        {
            if (!lazyInit)
            {
                Index = DictIndexerDsl.GetIndexFromStream(dictFileStream);
            }
        }

        public WordCard GetWordCard(string wordPart)
        {
            var time = DateTime.Now;
            var result = Index.GetWordCard(wordPart, CaseSensitiveSearch);
            var time2 = DateTime.Now - time;
            return result;
        }

        public List<TagContent> GetCardAsTags(string wordPart, Stream stream = null)
        {
            var card = GetWordCard(wordPart);
            return card == null ? null : GetCardAsTags(card, stream);
        }

        public List<TagContent> GetCardAsTags(WordCard card, Stream stream = null)
        {
            if (stream == null)
            {
                return RecursiveCardParser.DivideSiblingTags(FileHelper.GetBlockFromFile(card.CardFileTextBlock, FileName));    
            }

            stream.Position = 0;
            return RecursiveCardParser.DivideSiblingTags(FileHelper.GetBlockFromStream(card.CardFileTextBlock, stream));
        }

        public string GetCardAsHtmlString(string wordPart, Stream stream = null)
        {
            var card = GetWordCard(wordPart);
            return card == null ? null : GetCardAsHtmlString(card, stream);
        }

        public string GetCardAsHtmlString(WordCard card, Stream stream = null)
        {
            if (stream == null)
            {
                return DslToHtmlRegexpConverter.ConvertStringToHtml(FileHelper.GetBlockFromFile(card.CardFileTextBlock, FileName));
            }

            stream.Position = 0;
            var time = DateTime.Now;
            var block = FileHelper.GetBlockFromStream(card.CardFileTextBlock, stream);
            var time2 = DateTime.Now;
            var result =  DslToHtmlRegexpConverter.ConvertStringToHtml(block);
            var time3 = DateTime.Now;
            var deltaStream = time2 - time;
            var deltaParse = time3 - time2;
            return result;
        }
    }
}
