using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TagCheckLibrary
{
    class TokenUtils
    {
        private static Char CONST_ESCAPE = '\\';

        public static void ReadWhiteSpace(TextReader tr)
        {
            while (Char.IsWhiteSpace((Char)tr.Peek()))
            {
                tr.Read();
            }
        }

        // [\w]+
        public static string ReadWord(TextReader tr)
        {
            string word = "";
            while (Char.IsLetter((Char)tr.Peek()))
            {
                word += (Char)tr.Read();
            }
            return word;
        }

        // [\d]+
        public static string ReadNumber(TextReader tr)
        {
            string number = "";
            while (Char.IsDigit((Char)tr.Peek()))
            {
                number += (Char)tr.Read();
            }
            return number;
        }

        public static string ReadAlphaNumeric(TextReader tr)
        {
            string word = "";
            while (Char.IsLetterOrDigit((Char)tr.Peek()))
            {
                word += ((Char)tr.Read());
            }
            return word;
        }

        public static string ReadQuotedExpression(TextReader tr)
        {
            Char closingChar = (Char)tr.Read();
            Char previousChar = ' '; // empty
            string expr = "";
            Char thisChar = (Char)tr.Peek();
            while (thisChar != closingChar && previousChar != CONST_ESCAPE)
            {
                previousChar = thisChar;
                expr += (Char)tr.Read();
                thisChar = (Char)tr.Peek();
            }
            tr.Read(); // read closing char
            return expr;
        }
        public static string ReadToChar(TextReader tr, Char closingChar)
        {
            string expr = "";
            while ((Char)tr.Peek() != closingChar)
            {
                expr += (Char)tr.Read();
            }
            return expr;
        }
        
    }
}
