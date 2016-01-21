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
    }

    public class TagElementParser
    {
        public static int ReadAttributes(TextReader tr)
        {
            int count = 0;
            TokenUtils.ReadWhiteSpace(tr);
            while (TagElementParser.IsAttributeStart(tr))
            {
                TagElementParser.ReadAttribute(tr);
                TokenUtils.ReadWhiteSpace(tr);
                count++;
            }
            return count;
        }

        public static bool IsAttributeStart(TextReader tr)
        {
            return Char.IsLetterOrDigit((Char)tr.Peek());
        }

        // \w+=[["]\w+["]]
        public static bool ReadAttribute(TextReader tr)
        {
            TokenUtils.ReadAlphaNumeric(tr);
            TokenUtils.ReadWhiteSpace(tr);
            if ((Char)tr.Peek() == '/' || (Char)tr.Peek() == '>')
                return true;
            if ((Char)tr.Peek() == '=')
            {
                tr.Read();
                TokenUtils.ReadWhiteSpace(tr);
                TokenUtils.ReadQuotedExpression(tr);
            }

            return true;
        }

        public static TagElement ReadTag(TextReader tr)
        {
            TagElement.TagElementType tagType = TagElement.TagElementType.Unknown;
            string tagName = "";
            string tagText = "";
            tr.Read(); // <

            TokenUtils.ReadWhiteSpace(tr);
            if (tr.Peek() == '/')
            {
                tr.Read();
                tagType = TagElement.TagElementType.End;
                if (Char.IsLetterOrDigit((Char)tr.Peek())) // tag
                {
                    tagName = TokenUtils.ReadAlphaNumeric(tr);
                }
                TokenUtils.ReadWhiteSpace(tr);
                if (tr.Peek() == '>')
                {
                    tr.Read();
                    tagText = String.Format("</{0}>", tagName);
                    return new TagElement(TagElement.TagElementType.End, tagName, tagText);
                }
                else
                {
                    return new TagElement(TagElement.TagElementType.Unknown, tagName, "");
                }
            }

            TokenUtils.ReadWhiteSpace(tr);

            Char nextChar = (Char)tr.Peek();
            if (Char.IsLetterOrDigit(nextChar))
            {
                tagName = TokenUtils.ReadAlphaNumeric(tr);
                TokenUtils.ReadWhiteSpace(tr);

                TagElementParser.ReadAttributes(tr);
                TokenUtils.ReadWhiteSpace(tr);
            }
            else
            {
                return new TagElement(TagElement.TagElementType.Unknown, "", "");
            }

            if ((Char)tr.Peek() == '>')
            {
                tr.Read();
                tagType = TagElement.TagElementType.Start;
                tagText = String.Format("<{0}>", tagName);
            }
            else if (tr.Peek() == '/')
            {
                TokenUtils.ReadToChar(tr, '>');
                tagType = TagElement.TagElementType.Single;
                tagText = String.Format("<{0}/>", tagName);
            }

            return new TagElement(tagType, tagName, tagText);
        }

    }
}
