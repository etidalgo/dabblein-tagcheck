using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TagCheckLibrary
{
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

        public static void ReadAttributes(TextReader tr)
        {
            TokenUtils.ReadWhiteSpace(tr);
            while (TagElement.IsAttributeStart(tr))
            {
                TagElement.ReadAttribute(tr);
                TokenUtils.ReadWhiteSpace(tr);
            }
        }
        public static bool IsAttributeStart(TextReader tr)
        {
            return Char.IsLetterOrDigit((Char)tr.Peek());
        }

        // \w+=[["]\w+["]]
        public static void ReadAttribute(TextReader tr)
        {
            TokenUtils.ReadAlphaNumeric(tr);
            TokenUtils.ReadWhiteSpace(tr);
            if ((Char)tr.Peek() == '/' || (Char)tr.Peek() == '>')
                return;
            if ((Char)tr.Peek() == '=')
            {
                tr.Read();
                TokenUtils.ReadWhiteSpace(tr);
                TokenUtils.ReadQuotedExpression(tr);
            }
            else
            {
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

        public static TagElement ReadTag(TextReader tr)
        {
            TagElementType tagType = TagElementType.Unknown;
            string tagName = "";
            string tagText = "";
            tr.Read(); // <
            try
            {

            }
            catch (Exception e)
            { // catch all, return unknown tag type

            }
            TokenUtils.ReadWhiteSpace(tr);
            if (tr.Peek() == '/')
            {
                tr.Read();
                tagType = TagElementType.End;
                if (Char.IsLetterOrDigit((Char)tr.Peek())) // tag
                {
                    tagName = TokenUtils.ReadAlphaNumeric(tr);
                }
                TokenUtils.ReadWhiteSpace(tr);
                if (tr.Peek() == '>')
                {
                    tr.Read();
                    tagText = String.Format("</{0}>", tagName);
                    return new TagElement(TagElementType.End, tagName, tagText);
                }
                else
                {
                    return new TagElement(TagElementType.Unknown, tagName, "");
                }
            }

            TokenUtils.ReadWhiteSpace(tr);

            Char nextChar = (Char)tr.Peek();
            if (Char.IsLetterOrDigit(nextChar))
            {
                tagName = TokenUtils.ReadAlphaNumeric(tr);
                TokenUtils.ReadWhiteSpace(tr);

                TagElement.ReadAttributes(tr);
                TokenUtils.ReadWhiteSpace(tr);
            }
            else
            {
                return new TagElement(TagElementType.Unknown, "", "");
            }


            if ((Char)tr.Peek() == '>')
            {
                tr.Read();
                tagType = TagElementType.Start;
                tagText = String.Format("<{0}>", tagName);
            }
            else if (tr.Peek() == '/')
            {
                TokenUtils.ReadToChar(tr, '>');
                tagType = TagElementType.Single;
                tagText = String.Format("<{0}/>", tagName);
            }

            return new TagElement(tagType, tagName, tagText);
        }
    }
}
