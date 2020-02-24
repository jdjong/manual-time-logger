namespace ManualTimeLogger.Domain
{
    public class IssueNumberParser
    {
        private readonly InputPartSelector _selector;
        private const int DefaultIssueNumber = 0;

        public IssueNumberParser(InputPartSelector selector)
        {
            _selector = selector;
        }

        public ParseResult<int> Parse(string input)
        {
            var issueNumberString = _selector.Get(input).InputPart;

            if (string.IsNullOrEmpty(issueNumberString))
            {
                return new ParseResult<int>(true, DefaultIssueNumber);
            }

            var parseResult = int.TryParse(issueNumberString, out var issueNumber);
            return new ParseResult<int>(parseResult, issueNumber);
        }
    }
}
