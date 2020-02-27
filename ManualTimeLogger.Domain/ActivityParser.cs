using System;

namespace ManualTimeLogger.Domain
{
    public class ActivityParser
    {
        private readonly InputPartSelector _selector;

        public ActivityParser(InputPartSelector selector)
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
