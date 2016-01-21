using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Remoting.Channels;


namespace TagCheckLibrary
{
    enum ElementType
    {
        Unknown = 0, // 
        Malformed = 1, // error
        Content = 2, // ordinary text
        Tag = 3
    }

    public class HTMLParser
    {
        public ParseResult Check(string raw)
        {

            using (StringReader tr = new StringReader(raw))
            {
                Stack<TagElement> tags = new Stack<TagElement>();
                TokenUtils.ReadWhiteSpace(tr);

                string message;

                while (tr.Peek() != -1)
                {
                    switch ((Char)tr.Peek())
                    {
                        case '<':
                            if (!ProcessTag(tags, tr, out message)) {
                                return new ParseResult(false, message);
                            }
                            break;
                        default:
                            tr.Read();
                            // skip generic content
                            break;
                    }
                }
                if (tags.Count > 0)
                {
                    TagElement unmatched = tags.Pop();
                    return new InvalidParseResult(String.Format("Expected {0} found end of text", unmatched.TagName));
                }
            }
            return new ParseResult(true, "Correctly tagged paragraph");
        }

        private bool ProcessTag(Stack<TagElement> tags, TextReader tr, out string message)
        {
            message = "";
            TagElement tagElement = TagElementParser.ReadTag(tr);
            if (tagElement.TagType == TagElement.TagElementType.Start)
            {
                tags.Push(tagElement);
            }
            else if (tagElement.TagType == TagElement.TagElementType.End)
            {
                if (tags.Count <= 0)
                {
                    message = String.Format("Expected normal text or start tag found closing tag {0}", tagElement.TagName);
                    return false;
                }
                else
                {
                    TagElement lastTag = tags.Peek();
                    if (!lastTag.TagName.Equals(tagElement.TagName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        message = String.Format("Expected /{0} found {1}", lastTag.TagName, tagElement.TagName);
                        return false;
                    }
                    else
                    {
                        tags.Pop(); // accounted for
                    }
                }
            }
            else if (tagElement.TagType == TagElement.TagElementType.Single)
            {
                // do nothing, ignore
            }
            else
            {
                // unknown, not really a tag, treat as generic content
            }
            return true;
        }
    }
}
