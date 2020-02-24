using System;
using System.Linq;

namespace ManualTimeLogger.Domain
{
    /// <summary>
    /// This input parser requires a:
    ///  * duration (prepend with *)
    ///  * description (prepend with $)
    ///
    /// Optionally provide:
    ///  * issue number (prepend with #)
    ///  * label (prepend with @)
    /// </summary>
    public class LogEntryInputParser
    {
        // TODO, do something with logging and parse results/validation results
        private string IssueNumberSpecialChar => "#";
        private string DurationSpecialChar => "*";
        private string DescriptionSpecialChar => "$";
        private string LabelSpecialChar => "@";
        private string AllSectionMarkers => IssueNumberSpecialChar + DurationSpecialChar + DescriptionSpecialChar + LabelSpecialChar;

        private readonly IssueNumberParser _issueNumberParser;
        private readonly DurationParser _durationParser;
        private readonly DescriptionParser _descriptionParser;
        private readonly LabelParser _labelParser;

        public LogEntryInputParser()
        {
            _issueNumberParser = new IssueNumberParser(new InputPartSelector(IssueNumberSpecialChar, AllSectionMarkers, allowSpaces: false));
            _durationParser = new DurationParser(new InputPartSelector(DurationSpecialChar, AllSectionMarkers, allowSpaces:false));
            _descriptionParser = new DescriptionParser(new InputPartSelector(DescriptionSpecialChar, AllSectionMarkers, allowSpaces: true));
            _labelParser = new LabelParser(new InputPartSelector(LabelSpecialChar, AllSectionMarkers, allowSpaces: true));
        }

        public bool TryParse(string input, out LogEntry logEntry)
        {
            var issueNumberParseResult = _issueNumberParser.Parse(input);
            var durationParseResult = _durationParser.Parse(input);
            var descriptionParseResult = _descriptionParser.Parse(input);
            var labelParseResult = _labelParser.Parse(input);

            var isOverallSuccess = !string.IsNullOrEmpty(input) &&
                                   IsSpecialCharacterCountMaxOne(input) &&
                                   issueNumberParseResult.IsSuccess &&
                                   durationParseResult.IsSuccess &&
                                   descriptionParseResult.IsSuccess &&
                                   labelParseResult.IsSuccess;

            try
            {
                logEntry = isOverallSuccess 
                    ? new LogEntry(issueNumberParseResult.Value,
                        durationParseResult.Value,
                        descriptionParseResult.Value,
                        labelParseResult.Value,
                        DateTime.Today)
                    : null;
                return isOverallSuccess;
            }
            catch (Exception)
            {
                logEntry = null;
                return false;
            }
        }

        /// <summary>
        /// Checks if sections occur once at maximum.
        /// </summary>
        /// <param name="input"></param>
        private bool IsSpecialCharacterCountMaxOne(string input)
        {
            return AllSectionMarkers.Aggregate(true, (result, specialChar) =>
                {
                    return result && input.Count(character => character == specialChar) < 2;
                });
        }
    }
}