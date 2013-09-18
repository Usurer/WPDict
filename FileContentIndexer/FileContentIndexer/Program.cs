﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileContentIndexer
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
            ReadArticles();

            //while (true)
            //{
            //    Console.WriteLine("Enter a word");
            //    var w = Console.ReadLine();
            //    if (string.IsNullOrEmpty(w))
            //    {
            //        break;
            //    }
            //    var el = Index[w];
            //    //ReadArticles(el.Item1, el.Item2);
            //    //Console.WriteLine(GetBlock(el.Item1, el.Item2));
            //    var wc = CardParserObsolete.ParseBlock(GetBlock(el.Item1, el.Item2));

            //}

            while (true)
            {
                Console.WriteLine("Enter a word");
                var w = Console.ReadLine();
                if (string.IsNullOrEmpty(w))
                {
                    break;
                }
                //var el = Index[w];
                var els1 = Index.First(e => e.Key.StartsWith(w));
                var newParserResult = RecursiveCardParser.DivideSiblingTags(GetBlock(els1.Value.Item1, els1.Value.Item2));
                var result = RecursiveCardParser.ParseBlock(GetBlock(els1.Value.Item1, els1.Value.Item2));
                var s = result.Aggregate(string.Empty, (current, wordCardValue) => current + wordCardValue.GetValues());
                Console.WriteLine(s);
                //var els = Index.Where(e => e.Key.StartsWith(w)).Take(10).Select(e => CardParserObsolete.ParseBlock(GetBlock(e.Value.Item1, e.Value.Item2)));
                //foreach (var wordCard in els)
                //{
                //    Console.WriteLine(wordCard.Key);
                //    foreach (var value in wordCard.Values)
                //    {
                //        Console.WriteLine("-->{0}", value);
                //    }
                //}
                //ReadArticles(el.Item1, el.Item2);
            }

            //while (true)
            //{
            //    Console.WriteLine("Enter a char");
            //    var w = Console.ReadKey();
            //    var s = new string(w.KeyChar, 1);
            //    if (string.IsNullOrEmpty(s))
            //    {
            //        return;
            //    }
            //    var el = Index.Where(e => e.Key.StartsWith(s)).Select(e => e);
            //    Console.WriteLine("Found {0} items ", el.Count());
            //}
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
                        //Console.WriteLine("Pos = {0}, length = {1}", tuple.Item1, tuple.Item2);
                        //Console.WriteLine("---------Block end---------------");
                        block = string.Empty;
                        fileLength += tuple.Item2;
                        linesInBlock = 0;
                        blockKey = string.Empty;
                    }
                    else
                    {
                        var pos = Encoding.Unicode.GetBytes(block);
                        fileLengthTmp += pos.Length;
                        //Console.WriteLine(line);
                        //Console.WriteLine("Current pos bytes = {0}, line length bytes = {1}", fileLengthTmp, pos.Length);
                        //Console.WriteLine("Line length symbols = {0}", line.Length);
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

        static void ReadArticles()
        {
            using (var stream = new FileStream(FileName, FileMode.Open))
            {
                var indices = ArticlesIndices.ToArray();
                var size = indices.Length;
                var reader = new BinaryReader(stream);
                Byte[] a;

                for (int i = 0; i < size; i++)
                {
                    if (i < size - 1)
                    {
                        stream.Position = 2 + indices[i].Item1;
                        a = reader.ReadBytes(indices[i].Item2);
                        var l = Encoding.Unicode.GetString(a);
                        //Console.WriteLine("Reading from = {0}, bytes  = {1}", indices[i].Item1, indices[i].Item2);
                        //Console.WriteLine(l);
                        //Console.WriteLine("/*****************/");
                    }
                }
            }
        }

        static void ReadArticles(long from, int howManyBytes)
        {
            using (var stream = new FileStream(FileName, FileMode.Open))
            {
                var indices = ArticlesIndices.ToArray();
                var size = indices.Length;
                var reader = new BinaryReader(stream);
                Byte[] a;

                stream.Position = from;
                a = reader.ReadBytes(howManyBytes);
                var l = Encoding.Unicode.GetString(a);
                Console.WriteLine(l);
                //Console.WriteLine("Bytes are: ");
                //foreach (var singleByte in a)
                //{
                //    s
                //}
                //Console.WriteLine();
                //Console.WriteLine("---------");
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