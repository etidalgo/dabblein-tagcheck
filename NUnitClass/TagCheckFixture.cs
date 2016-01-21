using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using NUnit.Framework;
using TagCheckLibrary;
using System.Collections;

namespace TagCheck.Tests
{
    [TestFixture]
    public class TagCheckFixture
    {
        [SetUp]
        public void SetupContext()
        {

        }

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
        }

        [Test, TestCaseSource(typeof(TestDataClass), "AttributeTestCases")]
        public int CanReadAttributes( string raw )
        {
            using (StringReader tr = new StringReader(raw)) {
                return TagElementParser.ReadAttributes(tr);
            }
        }

        [Test, TestCaseSource(typeof(TestDataClass), "TagTestCases")]
        public void CanReadTags(string raw, TagElement.TagElementType expectedElementType, string expectedName)
        {
            using (StringReader tr = new StringReader(raw))
            {

                TagElement tag = TagElementParser.ReadTag(tr);
                Assert.AreEqual(expectedElementType, tag.TagType);
                Assert.AreEqual(expectedName, tag.TagName);
            }
        }
        [Test, TestCaseSource(typeof(TestDataClass), "NestedTagCases")]
        public void HandleNormalNestedTags(string prevTagName, string prevTagText, string currTagText, bool expectedResult, int expectedCount)
        {
            HTMLParser parser = new HTMLParser();
            string message;
            System.Collections.Generic.Stack<TagElement> tags = new System.Collections.Generic.Stack<TagElement>();
            tags.Push(new TagElement(TagElement.TagElementType.Start, prevTagName, prevTagText));

            using (StringReader tr = new StringReader(currTagText))
            {
                bool result = parser.ProcessTag(tags, tr, out message);
                Assert.AreEqual(expectedResult, result);
                Assert.AreEqual(expectedCount, tags.Count);
            }
        }

        [Test, TestCaseSource(typeof(TestDataClass), "GeneralTestCases")]
        public void HandleGeneral(string raw, bool expectedResult)
        {
            HTMLParser parser = new HTMLParser();

            ParseResult result = parser.Check(raw);
            Assert.AreEqual(expectedResult, result.IsValid);
        }
    }

    public class TestDataClass
    {
        public static IEnumerable AttributeTestCases
        {
            get{
                yield return new TestCaseData("").Returns(0);
                yield return new TestCaseData("alpha").Returns(1);
                yield return new TestCaseData("key=\"value\"").Returns(1);
                yield return new TestCaseData("alpha key=\"value\" beta").Returns(3);
            }
        }

        public static IEnumerable NestedTagCases
        {
            get
            {
                yield return new TestCaseData("x", "<X>", "</x>", true, 0);
                yield return new TestCaseData("X", "<X>", "</x>", true, 0); // case insensitive
                yield return new TestCaseData("b", "<b>", "</x>", false, 1);
                yield return new TestCaseData("b", "<b>", "<i>", true, 2); // pushes <i> onto tag stack
            }
        }

        public static IEnumerable TagTestCases
        {
            get
            {
                yield return new TestCaseData(@"<image />", TagElement.TagElementType.Single, "image");
                yield return new TestCaseData(@"<image display/>", TagElement.TagElementType.Single, "image");
                yield return new TestCaseData(@"<image src=""beta.jpg"" />", TagElement.TagElementType.Single, "image");
                yield return new TestCaseData(@"<image src=""beta.jpg"" display />", TagElement.TagElementType.Single, "image");
                yield return new TestCaseData("<div>", TagElement.TagElementType.Start, "div");
                yield return new TestCaseData("<div singleAttribute attrib= \"somthing\">", TagElement.TagElementType.Start, "div");
                yield return new TestCaseData("</b>", TagElement.TagElementType.End, "b");
                yield return new TestCaseData("</b/>", TagElement.TagElementType.Unknown, "b");
            }
        }

        public static IEnumerable GeneralTestCases
        {
            get
            {
                yield return new TestCaseData(@"<image display src=""blahblahblah"" />", true);
                yield return new TestCaseData(@"<image src=""blahblahblah"" />", true);
                yield return new TestCaseData("<html><head></head><body></body></html>", true);
                yield return new TestCaseData("<b><i>Some text properly nested</i></b>", true);
                yield return new TestCaseData("<b><i>Some text <image src=\"blahblahblah\" />properly nested with single tag</i></b>", true);
                yield return new TestCaseData("<b><i>Some text properly nested with single tag</i><image src=\"blahblahblah\" /></b>", true);
                yield return new TestCaseData("<b><i>Some text badly nested</b></i>", false);
                yield return new TestCaseData("<b><i>Some text Missing closing</i>", false);
                yield return new TestCaseData("<i>Some text Missing Opening</b></i>", false);
                yield return new TestCaseData("<i>Some text Missing Opening</i></b>", false);
            }
        }
    }
}
