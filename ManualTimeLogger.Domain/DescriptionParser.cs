namespace ManualTimeLogger.Domain
{
    public class DescriptionParser
    {
        private readonly InputPartSelector _selector;

        public DescriptionParser(InputPartSelector selector)
        {
            _selector = selector;
        }

        public ParseResult<string> Parse(string input)
        {
            var description = _selector.Get(input).InputPart;

            if (string.IsNullOrEmpty(description))
            {
                return new ParseResult<string>(false, null);
            }

            return new ParseResult<string>(true, description);
        }
    }
}
