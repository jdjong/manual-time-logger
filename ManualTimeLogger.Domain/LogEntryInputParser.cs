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

        private readonly IssueNumberParser _issueNumberParser;
        private readonly DurationParser _durationParser;
        private readonly DescriptionParser _descriptionParser;
        private readonly LabelParser _labelParser;

        public LogEntryInputParser()
        {
            var allSectionMarkers = IssueNumberSpecialChar + DurationSpecialChar + DescriptionSpecialChar + LabelSpecialChar;

            _issueNumberParser = new IssueNumberParser(new InputPartSelector(IssueNumberSpecialChar, allSectionMarkers, allowSpaces: false));
            _durationParser = new DurationParser(new InputPartSelector(DurationSpecialChar, allSectionMarkers, allowSpaces:false));
            _descriptionParser = new DescriptionParser(new InputPartSelector(DescriptionSpecialChar, allSectionMarkers, allowSpaces: true));
            _labelParser = new LabelParser(new InputPartSelector(LabelSpecialChar, allSectionMarkers, allowSpaces: true));
        }

        public bool TryParse(string input, out LogEntry logEntry)
        {
            var issueNumberParseResult = _issueNumberParser.Parse(input);
            var durationParseResult = _durationParser.Parse(input);
            var descriptionParseResult = _descriptionParser.Parse(input);
            var labelParseResult = _labelParser.Parse(input);

            var isOverallSuccess = IsInputValid(input) &&
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
        /// Checks if the required sections are provided
        /// and sections occur once at maximum.
        /// </summary>
        /// <param name="input"></param>
        private bool IsInputValid(string input)
        {
            // There is some input
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            // Only one input part at max.
            var countDescriptionSpecialChars = input.Count(character => character.ToString() == DescriptionSpecialChar);
            var countDurationSpecialChars = input.Count(character => character.ToString() == DurationSpecialChar);
            var countIssueNumberSpecialChars = input.Count(character => character.ToString() == IssueNumberSpecialChar);
            var countLabelSpecialChars = input.Count(character => character.ToString() == LabelSpecialChar);
            if (countDescriptionSpecialChars > 1 ||
                countDurationSpecialChars > 1 ||
                countLabelSpecialChars > 1 ||
                countIssueNumberSpecialChars > 1)
            {
                return false;
            }

            return true;
        }
    }
}