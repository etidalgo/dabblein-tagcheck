using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = "<html><head></head><body></body></html>";
            var badly = "<b><i>Some text</b></i>";

            HTMLParser parser = new HTMLParser();
            ParseResult result = parser.Check(text);
            Console.WriteLine("Result: {0} ", result.IsValid);
            result = parser.Check(badly);
            Console.WriteLine("Result: {0}", result.IsValid);
        }
    }
}
