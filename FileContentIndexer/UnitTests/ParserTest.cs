using System;
using System.Collections.Generic;
using PhoneLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class ParserTest
    {
        private const string Arry = @"'Arry
\['ærı\] [com][i][c][p]n[/p][/c][/i][/com]
[trn][m1]1. [com][i][c][p]прост.[/p][/c][/i][/com] = Harry [/m]
[m1]2. [com][i][c][p]пренебр.[/p][/c] [/i][/com]кокни, весёлый и не очень грамотный лондонец [/m]
[m1][/m][/trn]";

        [TestInitialize]
        private void SetupTest()
        {
            
        }

        [TestMethod]
        public void GetTagAndPositionTest()
        {
            var parseResult = RecursiveCardParser.GetTagAndPosition(Arry);
            var expectedResult = new TagPart("com", 16, false);

            Assert.AreEqual(parseResult.IsClosingPartOfTag, expectedResult.IsClosingPartOfTag);
            Assert.AreEqual(parseResult.PartValue, expectedResult.PartValue);
            Assert.AreEqual(parseResult.StartsAt, expectedResult.StartsAt);
        }

        [TestMethod]
        public void ParseTagsTest()
        {
            var divisionResult = RecursiveCardParser.DivideSiblingTags(Arry);
            var tag = divisionResult[1].Value as Tag;
            Assert.IsNotNull(tag);
            var parseRes = TagTypeParser.GetTagTypeFromName(tag.TagName);
            Assert.AreNotEqual(parseRes, TagTypes.Undefined);
        }

        [TestMethod]
        public void DivideSiblingTagsTest()
        {
            var divisionResult = RecursiveCardParser.DivideSiblingTags(Arry);
            var expectedListLength = 3;
            Assert.AreEqual(divisionResult.Count, expectedListLength);

            var firstTagString = @"'Arry
\['ærı\] ";
            var firstTag = new Tag() { TagContent = new List<TagContent>() { new TagContent(firstTagString) } };

            Assert.AreEqual(divisionResult[0].Value, firstTag.TagContent[0].Value);

            var secondTag = new Tag()
            {
                TagName = "com", TagContent = new List<TagContent>()
                {
                    new TagContent(new Tag()
                    {
                        TagName = "i",
                        TagContent = new List<TagContent>()
                        {
                            new TagContent(new Tag()
                            {
                                TagName = "c",
                                TagContent = new List<TagContent>()
                                {
                                    new TagContent(new Tag()
                                    {
                                        TagName = "p",
                                        TagContent = new List<TagContent>()
                                        {
                                            new TagContent("n")
                                        }
                                    })      
                                }
                            })      
                        }
                    })
                }
            };

            Assert.IsTrue(divisionResult[1].Equals(new TagContent(secondTag)));

            var thirdTag = new Tag() { TagName = "trn" };
        }
    }
}
