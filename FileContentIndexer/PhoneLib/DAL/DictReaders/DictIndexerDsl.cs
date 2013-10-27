using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhoneLib.DAL.DictReaders
{
    public static class DictIndexerDsl
    {
        public static DictIndex GetDictionaryIndex(string dictFilePath, bool isPhone = false)
        {
            DictIndex index;
            if (isPhone)
            {
                try
                {
                    var resource = Application.GetResourceStream(new Uri(dictFilePath, UriKind.Relative));
                    index = GetIndexFromStream(resource.Stream);
                    return index;
                }
                catch (Exception e)
                {
                    // TODO: Implement logger.
                }
            }

            using (var stream = new FileStream(dictFilePath, FileMode.Open))
            {
                index = GetIndexFromStream(stream);
            }

            return index;
        }

        public static DictIndex GetIndexFromStream(Stream stream)
        {
            var index = new DictIndex();
            if (index.LoadIndex())
            {
                return index;
            }

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
                    blockKey = line; // It should be a dictionary word.
                }
                block += line;
                linesInBlock++;
                /* In DSL we have empty lines as a dividers, so everything between a blockKey word and an empty line is a word card*/
                // TODO: Check in DSL docs if empty line is one and only divider
                if (string.IsNullOrEmpty(line))
                {
                    var pos = Encoding.Unicode.GetBytes(block);
                    var blockStartsAtByte = fileLength;
                    // I assume that second part is to get amount of newLine-like symbols. But I'm not sure already ))
                    var blockSizeIs = pos.Length + linesInBlock*2;

                    index.AddWordCard(new WordCard()
                        {
                            CardKey = blockKey,
                            CardFileTextBlock = new FileTextBlock() {Length = blockSizeIs, StartsAtByte = blockStartsAtByte}
                        });

                    block = string.Empty;
                    fileLength += blockSizeIs;
                    linesInBlock = 0;
                    blockKey = string.Empty;
                }
                else
                {
                    var pos = Encoding.Unicode.GetBytes(block);
                    fileLengthTmp += pos.Length;
                }
            }
            
            index.SaveIndex();
            return index;
        }
    }
}
