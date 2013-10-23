using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneLib;

namespace ConcoleClient
{
    class Program
    {
        private const string FileName = "data/dict.dsl";
        private static readonly List<Tuple<long, int, string>> ArticlesIndices = new List<Tuple<long, int, string>>();
        private static Dictionary<string, Tuple<long, int>> Index = new Dictionary<string, Tuple<long, int>>();

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            CreateIndex();

            while (true)
            {
                Console.WriteLine("Enter a word");
                var w = Console.ReadLine();
                if (string.IsNullOrEmpty(w))
                {
                    break;
                }
                var card = Index.FirstOrDefault(e => e.Key.StartsWith(w));
                if (card.Key == null)
                {
                    Console.WriteLine("Word not found");
                    continue;
                }
                var newParserResult = RecursiveCardParser.DivideSiblingTags(GetBlock(card.Value.Item1, card.Value.Item2));
                Console.WriteLine("");
                //Console.WriteLine(TagPrinter(newParserResult));
                Console.WriteLine("Parsed");
            }
        }

        static string TagPrinter(TagContent tagContent)
        {
            var result = "";
            if (tagContent.ContentType == TagContentTypes.Tag)
            {
                var tag = tagContent.Value as Tag;
                result = result + " " + string.Format(
                    "Tag: {0} Value: {2} {1}"
                    , tag.GetTagType()
                    , Environment.NewLine
                    , TagPrinter(tag.TagContent));
            }
            else
            {
                result = result + " " + (tagContent.Value as string) + Environment.NewLine;
            }
            return result;
        }

        static string TagPrinter(IEnumerable<TagContent> tagContent)
        {
            var result = "";
            foreach (var content in tagContent)
            {
                result = result + TagPrinter(content);
            }
            return result;
        }

        static void CreateIndex()
        {
            using (var stream = new FileStream(FileName, FileMode.Open))
            {
                var reader = new StreamReader(stream);
                var block = string.Empty;
                long fileLength = 0;
                long fileLengthTmp = 0;
                var linesInBlock = 0;
                var blockKey = string.Empty;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(block) && !line.StartsWith("#") && !string.IsNullOrEmpty(line))
                    {
                        blockKey = line;
                    }
                    block += line;
                    linesInBlock++;
                    if (string.IsNullOrEmpty(line))
                    {
                        var pos = Encoding.Unicode.GetBytes(block);
                        var tuple = new Tuple<long, int, string>(fileLength, pos.Length + linesInBlock * 2, blockKey);
                        ArticlesIndices.Add(tuple);
                        block = string.Empty;
                        fileLength += tuple.Item2;
                        linesInBlock = 0;
                        blockKey = string.Empty;
                    }
                    else
                    {
                        var pos = Encoding.Unicode.GetBytes(block);
                        fileLengthTmp += pos.Length;
                    }
                }


            }

            foreach (var articlesIndex in ArticlesIndices)
            {
                try
                {
                    Index.Add(articlesIndex.Item3, new Tuple<long, int>(articlesIndex.Item1, articlesIndex.Item2));
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }

            }
        }

        static string GetBlock(long from, int howManyBytes)
        {
            using (var stream = new FileStream(FileName, FileMode.Open))
            {
                var reader = new BinaryReader(stream);
                stream.Position = from;
                byte[] a = reader.ReadBytes(howManyBytes);
                return Encoding.Unicode.GetString(a);
            }
        }
    }
}
