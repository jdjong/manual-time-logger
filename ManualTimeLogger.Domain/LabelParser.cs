using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualTimeLogger.Domain
{
    public class LabelParser
    {
        private readonly InputPartSelector _selector;

        public LabelParser(InputPartSelector selector)
        {
            _selector = selector;
        }

        public ParseResult<string> Parse(string input)
        {
            var label = _selector.Get(input).InputPart;
            return new ParseResult<string>(true, label);
        }
    }
}
