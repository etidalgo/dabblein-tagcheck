using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagCheckLibrary;

// Todo: 
// Get NUnit working
// Clean up TagElement.
// 
namespace TagCheck
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] testCases = {
                    @"<image display src=""blahblahblah"" />",
                    @"<image src=""blahblahblah"" />",
                    "<html><head></head><body></body></html>", 
                    "<b><i>Some text properly nested</i></b>",
                    "<b><i>Some text <image src=\"blahblahblah\" />properly nested with single tag</i></b>",
                    "<b><i>Some text properly nested with single tag</i><image src=\"blahblahblah\" /></b>",
                    "<b><i>Some text badly nested</b></i>",
                    "<b><i>Some text Missing closing</i>", 
                    "<i>Some text Missing Opening</b></i>",
                    "<i>Some text Missing Opening</i></b>"
            };

            HTMLParser parser = new HTMLParser();
            ParseResult result;
            foreach (var testCase in testCases)
            {
                result = parser.Check(testCase);
                Console.WriteLine("Result: {0} {1}\r\n {2}", result.IsValid, result.Message, testCase);
            }
        }
    }
}
