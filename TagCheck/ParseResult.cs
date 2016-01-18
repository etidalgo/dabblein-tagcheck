using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagCheck
{
    class ParseResult
    {
        public bool IsValid { get; private set; }
        public ParseResult( bool isValid )
        {
            IsValid = isValid;
        }
    }
}
