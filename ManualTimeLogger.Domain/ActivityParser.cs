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

        public ParseResult<Activity> Parse(string input)
        {
            var activityInputPart = _selector.Get(input).InputPart?.ToLower();
            var parseResult = Enum.TryParse<Activity>(activityInputPart, out var activity);

            return new ParseResult<Activity>(parseResult, activity);
        }
    }
}
