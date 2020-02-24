using System.Globalization;

namespace ManualTimeLogger.Domain
{
    public class DurationParser
    {
        private readonly InputPartSelector _selector;

        public DurationParser(InputPartSelector selector)
        {
            _selector = selector;
        }

        public ParseResult<float> Parse(string input)
        {
            var decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var durationString = _selector.Get(input).InputPart;
            var parseResult = float.TryParse(durationString?.Replace(",", decimalSeparator).Replace(".", decimalSeparator), out var duration);

            return new ParseResult<float>(parseResult, duration);
        }
    }
}
