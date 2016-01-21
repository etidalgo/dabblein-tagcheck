using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Remoting.Channels;

// TextReader Class (System.IO) <https://msdn.microsoft.com/en-us/library/system.io.textreader(v=vs.110).aspx>


namespace TagCheck
{
    enum ElementType
    {
        Unknown = 0,
        Generic = 1,
        Tag = 2
       
    }
    public class TagElement
    {
        public enum TagElementType
        {
            Unknown = 0,
            Start = 1,
            End = 2,
            Single = 3
        }

        public TagElementType TagType { get; private set; }
        public string TagName { get; private set; }

        public TagElement(TagElementType tagType, string tagName)
        {
            TagType = tagType;
            TagName = tagName;
        }

        public static void ReadAttribute(TextReader tr)
        {
            
        }

        public static void ReadToCloseBracket(TextReader tr)
        {
            Char penultimateChar;
            while (Char.IsWhiteSpace((Char)tr.Peek()))
            {
                tr.Read();
            }
        }

        public static TagElement ReadTag( TextReader tr)
        {
            TagElementType tagType = TagElementType.Unknown;
            string tagName = "";

            tr.Read(); // <

            Tokenizer.ReadWhiteSpace(tr);
            if (tr.Peek() == '/')
            {
                tagType = TagElementType.End;
            }
            Tokenizer.ReadWhiteSpace(tr);
            
            if (Char.IsLetterOrDigit((Char) tr.Peek())) // tag
            {
                tagName = Tokenizer.ReadAlphaNumeric(tr);
            }
            Tokenizer.ReadWhiteSpace(tr);

            if (tagType == TagElementType.Unknown)
            {
                if (Char.IsLetterOrDigit((Char) tr.Peek()))
                {
                    tagType = TagElementType.Start;
                    // read attributes
                    // read to closing
                }
                else if (tr.Peek() == '/')
                {
                    tagType = TagElementType.Single;
                }
            }
            Tokenizer.ReadToChar(tr, '>');
            

            return new TagElement(tagType, tagName);
        }
    }

    public class Tokenizer
    {
        private static Char CONST_ESCAPE = '\\';

        public static void ReadWhiteSpace(TextReader tx)
        {
            while (Char.IsWhiteSpace((Char) tx.Peek()))
            {
                tx.Read();
            }
        }

        // [\w]+
        public static string ReadWord(TextReader tx)
        {
            string word = "";
            while (Char.IsLetter((Char) tx.Peek()))
            {
                word += tx.Read();
            }
            return word;
        }

        // [\d]+
        public static string ReadNumber(TextReader tx)
        {
            string number = "";
            while (Char.IsDigit((Char) tx.Peek()))
            {
                number += tx.Read();
            }
            return number;
        }

        public static string ReadAlphaNumeric(TextReader tx)
        {
            string word = "";
            while (Char.IsLetterOrDigit((Char) tx.Peek()))
            {
                word += tx.Read();
            }
            return word;
        }

        public static string ReadEnclosedExpression(TextReader tx, Char closingChar)
        {
            Char previousChar = ' '; // empty
            string expr = "";
            Char thisChar = (Char) tx.Peek();
            while (thisChar != closingChar && previousChar != CONST_ESCAPE)
            {
                previousChar = thisChar;
                expr += tx.Read();
                thisChar = (Char) tx.Peek();
            }
            return expr;
        }

        public static string ReadToChar(TextReader tx, Char closingChar)
        {
            string expr = "";
            while ((Char) tx.Peek() != closingChar)
            {
                expr += tx.Read();
            }
            return expr;
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
            using (StringReader sr = new StringReader(raw))
            {
                Stack<TagElement> tags = new Stack<TagElement>();
                Tokenizer.ReadWhiteSpace(sr);
                Char nextChar;
                while (sr.Peek() != '<' )
                {
                    sr.Read();
                }

                switch (sr.Peek())
                {
                    case '<':
                        TagElement tagElement = TagElement.ReadTag(sr);

                        break;
                    default:
                        break;
                }
            } 
            return new ParseResult(false);
        }
        public ParseResult Parse(TextReader tr)
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
