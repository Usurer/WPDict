using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using PhoneLib;
using PhoneLib.Helpers;

namespace ConcoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            Console.WriteLine("Initializing dictionary...");
            var dictionary = new DictionaryDataProvider();

            while (true)
            {
                Console.WriteLine("Enter a word");
                var w = Console.ReadLine();
                if (string.IsNullOrEmpty(w))
                {
                    break;
                }
                var card = dictionary.GetWordCard(w);
                if (card == null)
                {
                    Console.WriteLine("Word not found");
                    continue;
                }
                var newParserResult = dictionary.GetCardAsTags(card);
                Console.WriteLine("");
                Console.WriteLine(newParserResult.ConvertToNoTagsString());
                Console.WriteLine("Parsed");
            }
        }
    }
}
