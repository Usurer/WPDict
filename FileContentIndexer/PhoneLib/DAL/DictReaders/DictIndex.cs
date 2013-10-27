using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhoneLib.DAL.DictReaders
{
    public class DictIndex
    {
        private Dictionary<string, FileTextBlock> Index = new Dictionary<string, FileTextBlock>();

        public bool AddWordCard(WordCard card)
        {
            if (Index.ContainsKey(card.CardKey))
            {
                return false;
            }
            
            Index.Add(card.CardKey, card.CardFileTextBlock);
            return true;
        }

        public WordCard GetWordCard(string wordBeginning, bool caseSensitive = false)
        {
            var match = Index.FirstOrDefault(card => card.Key.StartsWith(wordBeginning, caseSensitive
                                                    ? StringComparison.InvariantCulture
                                                    : StringComparison.CurrentCultureIgnoreCase));
            if (match.Key == null)
            {
                return null;
            }
            return new WordCard() {CardKey = match.Key, CardFileTextBlock = match.Value};
        }

        public void SaveIndex()
        {
            System.IO.IsolatedStorage.IsolatedStorageFile local = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
            if (!local.DirectoryExists("IndicesFolder"))
            {
                local.CreateDirectory("IndicesFolder");
            }

            using (var isoFileStream = new System.IO.IsolatedStorage.IsolatedStorageFileStream("IndicesFolder\\Index.txt", System.IO.FileMode.OpenOrCreate, local))
            {
                using (var isoFileWriter = new System.IO.StreamWriter(isoFileStream))
                {
                    foreach (var card in Index)
                    {
                        var cardAsString = string.Format("[[{0}]] {1} {2}{3}", card.Key.Trim("\n".ToCharArray()), card.Value.StartsAtByte, card.Value.Length, Environment.NewLine);
                        isoFileWriter.WriteLine(cardAsString);
                    }
                }
            }
        }

        public bool LoadIndex()
        {
            System.IO.IsolatedStorage.IsolatedStorageFile local = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
            var newIndex = new Dictionary<string, FileTextBlock>();

            if (!local.FileExists("IndicesFolder\\Index.txt"))
            {
                return false;
            }

            using (var isoFileStream = new System.IO.IsolatedStorage.IsolatedStorageFileStream("IndicesFolder\\Index.txt", System.IO.FileMode.Open, local))
            {
                using (var isoFileReader = new System.IO.StreamReader(isoFileStream))
                {
                    var line = isoFileReader.ReadLine();
                    while (!isoFileReader.EndOfStream)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            var firstSpace = line.IndexOf(" ", line.IndexOf("]]"));
                            if (firstSpace > 0)
                            {
                                var secondSpace = line.IndexOf(" ", firstSpace + 1);
                                var wordKey = line.Substring(2, line.IndexOf("]]") - 2);
                                var startsAt = long.Parse(line.Substring(firstSpace + 1, secondSpace - firstSpace).Trim(' '));
                                var length = int.Parse(line.Substring(secondSpace + 1).Trim(' '));
                                newIndex.Add(wordKey, new FileTextBlock() { Length = length, StartsAtByte = startsAt });    
                            }
                        }
                        line = isoFileReader.ReadLine();
                    }
                }
            }

            Index = newIndex;
            return true;
        }

        public void DeleteIndex()
        {
            System.IO.IsolatedStorage.IsolatedStorageFile local = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();

            if (local.FileExists("IndicesFolder\\Index.txt"))
            {
                local.DeleteFile("IndicesFolder\\Index.txt");
            }
        }
    }
}
