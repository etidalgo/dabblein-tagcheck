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
        Unknown = 0, // 
        Malformed = 1, // error
        Content = 2, // ordinary text
        Tag = 3
       
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
        public string TagText { get; private set; }

        public TagElement(TagElementType tagType, string tagName, string tagText)
        {
            TagType = tagType;
            TagName = tagName;
            TagText = tagText;
        }

        public static void ReadAttributes(TextReader tr ) {
            Tokenizer.ReadWhiteSpace(tr);
            while( TagElement.IsAttributeStart(tr)) {
                TagElement.ReadAttribute(tr);
                Tokenizer.ReadWhiteSpace(tr);
            }
        }
        public static bool IsAttributeStart(TextReader tr) {
            return Char.IsLetterOrDigit((Char) tr.Peek());
        }

        // \w+=[["]\w+["]]
        public static void ReadAttribute(TextReader tr)
        {
            Tokenizer.ReadAlphaNumeric(tr);
            Tokenizer.ReadWhiteSpace(tr);
            if ((Char)tr.Peek() == '/' || (Char)tr.Peek() == '>')
                return;
            if ((Char)tr.Peek() == '=')
            {
                tr.Read();
                Tokenizer.ReadWhiteSpace(tr);
                Tokenizer.ReadQuotedExpression(tr);
            }
            else {
                // error, shouldn't be anything here
            }
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
            string tagText = "";
            tr.Read(); // <
            try {

            } 
            catch( Exception e ) { // catch all, return unknown tag type

            }
            Tokenizer.ReadWhiteSpace(tr);
            if (tr.Peek() == '/')
            {
                tagType = TagElementType.End;
                if (Char.IsLetterOrDigit((Char)tr.Peek())) // tag
                {
                    tagName = Tokenizer.ReadAlphaNumeric(tr);
                }
                Tokenizer.ReadWhiteSpace(tr);
                if (tr.Peek() == '>')
                {
                    tagText = String.Format("</{0}>", tagName);
                    return new TagElement(TagElementType.End, tagName, tagText);
                }
                else
                {
                    return new TagElement(TagElementType.Unknown, tagName, "");
                }
            }
            
            Tokenizer.ReadWhiteSpace(tr);

            Char nextChar = (Char)tr.Peek();
            if (Char.IsLetterOrDigit(nextChar))
            {
                tagName = Tokenizer.ReadAlphaNumeric(tr);
                Tokenizer.ReadWhiteSpace(tr);

                TagElement.ReadAttributes(tr);
                Tokenizer.ReadWhiteSpace(tr);
            } else {
                return new TagElement(TagElementType.Unknown, "", "");
            }


            if (nextChar == '>')
            {
                tagType = TagElementType.Start;
                tagText = String.Format("<{0}>", tagName);
            }
            else if (tr.Peek() == '/')
            {
                tagType = TagElementType.Single;
                tagText = String.Format("<{0}/>", tagName);
            }
            Tokenizer.ReadToChar(tr, '>');
            
            return new TagElement(tagType, tagName, tagText);
        }
    }

    public class Tokenizer
    {
        private static Char CONST_ESCAPE = '\\';

        public static void ReadWhiteSpace(TextReader tr)
        {
            while (Char.IsWhiteSpace((Char) tr.Peek()))
            {
                tr.Read();
            }
        }

        // [\w]+
        public static string ReadWord(TextReader tr)
        {
            string word = "";
            while (Char.IsLetter((Char) tr.Peek()))
            {
                word += (Char)tr.Read();
            }
            return word;
        }

        // [\d]+
        public static string ReadNumber(TextReader tr)
        {
            string number = "";
            while (Char.IsDigit((Char) tr.Peek()))
            {
                number += (Char)tr.Read();
            }
            return number;
        }

        public static string ReadAlphaNumeric(TextReader tr)
        {
            string word = "";
            while (Char.IsLetterOrDigit((Char) tr.Peek()))
            {
                word += ((Char)tr.Read());
            }
            return word;
        }

        public static string ReadQuotedExpression(TextReader tr)
        {
            Char closingChar = (Char)tr.Peek();
            Char previousChar = ' '; // empty
            string expr = "";
            Char thisChar = (Char)tr.Peek();
            while (thisChar != closingChar && previousChar != CONST_ESCAPE)
            {
                previousChar = thisChar;
                expr += (Char)tr.Read();
                thisChar = (Char)tr.Peek();
            }
            return expr;
        }
        public static string ReadToChar(TextReader tr, Char closingChar)
        {
            string expr = "";
            while ((Char) tr.Peek() != closingChar)
            {
                expr += (Char)tr.Read();
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
            using (StringReader sr = new StringReader(raw))
            {
                Stack<TagElement> tags = new Stack<TagElement>();
                Tokenizer.ReadWhiteSpace(sr);
                Char nextChar;

                while (sr.Peek() != -1)
                {
                    switch ((Char)sr.Peek())
                    {
                        case '<':
                            TagElement tagElement = TagElement.ReadTag(sr);
                            if (tagElement.TagType == TagElement.TagElementType.Start)
                            {
                                tags.Push(tagElement);
                            }
                            else if (tagElement.TagType == TagElement.TagElementType.End)
                            {
                                if (tags.Count <= 0)
                                {
                                    string message = String.Format("Expected end of tags found {0}", tagElement.TagName);
                                    return new ParseResult(false, message);
                                }
                                else
                                {
                                    //TagElement lastTag = tags.Pop();
                                    //string message = String.Format("Expected {0} found end of text", lastTag.TagName);

                                }
                                // verify that end tag corresponds to last open tag
                            }
                            else if (tagElement.TagType == TagElement.TagElementType.Single)
                            {
                                // do nothing, ignore
                            }
                            else
                            {
                                // unknown, not really a tag, treat as generic content
                            }
                            break;
                        default:
                            sr.Read();
                            // skip generic content
                            break;
                    }
                }
                if (tags.Count > 0)
                {
                    TagElement unmatched = tags.Pop();
                    string message = String.Format("Expected {0} found end of text", unmatched.TagName );
                    return new ParseResult(false, message);
                }
            } 
            return new ParseResult(true, "Correctly tagged paragraph");
        }
    }
}
