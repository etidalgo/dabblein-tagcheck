using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagCheck
{
    public class ParseResult
    {
        public bool IsValid { get; private set; }
        public string Message { get; private set; }
        public ParseResult( bool isValid, string message )
        {
            IsValid = isValid;
            Message = message;
        }
    }
}
