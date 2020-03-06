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
    ///  * activity (prepend with !)
    /// </summary>
    public class LogEntryInputParser
    {
        private string IssueNumberSpecialChar => "#";
        private string DurationSpecialChar => "*";
        private string DescriptionSpecialChar => "$";
        private string LabelSpecialChar => "@";
        private string ActivitySpecialChar => "!";
        private string AllSectionMarkers => IssueNumberSpecialChar + DurationSpecialChar + DescriptionSpecialChar + LabelSpecialChar + ActivitySpecialChar;

        private readonly IssueNumberParser _issueNumberParser;
        private readonly DurationParser _durationParser;
        private readonly DescriptionParser _descriptionParser;
        private readonly LabelParser _labelParser;
        private readonly ActivityParser _activityParser;

        public LogEntryInputParser()
        {
            _issueNumberParser = new IssueNumberParser(new InputPartSelector(IssueNumberSpecialChar, AllSectionMarkers, allowSpaces: false));
            _durationParser = new DurationParser(new InputPartSelector(DurationSpecialChar, AllSectionMarkers, allowSpaces:false));
            _descriptionParser = new DescriptionParser(new InputPartSelector(DescriptionSpecialChar, AllSectionMarkers, allowSpaces: true));
            _labelParser = new LabelParser(new InputPartSelector(LabelSpecialChar, AllSectionMarkers, allowSpaces: true));
            _activityParser = new ActivityParser(new InputPartSelector(ActivitySpecialChar, AllSectionMarkers, allowSpaces: true));
        }

        public bool TryParse(string input, out LogEntry logEntry)
        {
            var issueNumberParseResult = _issueNumberParser.Parse(input);
            var durationParseResult = _durationParser.Parse(input);
            var descriptionParseResult = _descriptionParser.Parse(input);
            var labelParseResult = _labelParser.Parse(input);
            var activityParseResult = _activityParser.Parse(input);

            var isOverallSuccess = !string.IsNullOrEmpty(input) &&
                                   IsSpecialCharacterCountMaxOne(input) &&
                                   issueNumberParseResult.IsSuccess &&
                                   durationParseResult.IsSuccess &&
                                   descriptionParseResult.IsSuccess &&
                                   labelParseResult.IsSuccess &&
                                   activityParseResult.IsSuccess;

            try
            {
                logEntry = isOverallSuccess 
                    ? new LogEntry(issueNumberParseResult.Value,
                        durationParseResult.Value,
                        descriptionParseResult.Value,
                        labelParseResult.Value,
                        activityParseResult.Value,
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