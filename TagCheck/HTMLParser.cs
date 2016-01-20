using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TextReader Class (System.IO) <https://msdn.microsoft.com/en-us/library/system.io.textreader(v=vs.110).aspx>


namespace TagCheck
{
    public class TagElement
    {
        public int TagType { get; private set; }

        public TagElement()
        {

        }

        public static TagElement Parse( TextReader tr)
        {

        }
    }
    public class HTMLParser
    {
        public HTMLParser() { 
        }
        public ParseResult Check(string raw)
        {
            // tokenize
            // validate
            return new ParseResult(false);
        }

        public void Parse(string raw)
        {

        }


    }
}
