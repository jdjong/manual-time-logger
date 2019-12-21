using System;
using System.Globalization;
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
    /// </summary>
    public class LogEntryInputParser
    {
        // TODO, do something with logging and parse results/validation results
        
        private const string IssueNumberSpecialChar = "#";
        private const string DurationSpecialChar = "*";
        private const string DescriptionSpecialChar = "$";
        private const string LabelSpecialChar = "@";
        private const string SpaceCharacter = " ";
        private const int DefaultIssueNumber = 0;

        // TODO, refactor
        private readonly InputPartSelector _issueNumberSelector = new InputPartSelector(IssueNumberSpecialChar, SpaceCharacter + IssueNumberSpecialChar + DurationSpecialChar + DescriptionSpecialChar + LabelSpecialChar);
        private readonly InputPartSelector _durationSelector = new InputPartSelector(DurationSpecialChar, SpaceCharacter + IssueNumberSpecialChar + DurationSpecialChar + DescriptionSpecialChar + LabelSpecialChar);
        private readonly InputPartSelector _descriptionSelector = new InputPartSelector(DescriptionSpecialChar, IssueNumberSpecialChar + DurationSpecialChar + DescriptionSpecialChar + LabelSpecialChar);
        private readonly InputPartSelector _labelSelector = new InputPartSelector(LabelSpecialChar, IssueNumberSpecialChar + DurationSpecialChar + DescriptionSpecialChar + LabelSpecialChar);

        public bool TryParse(string input, out LogEntry logEntry)
        {
            var parseResult = IsInputValid(input);

            if (!TryParseIssueNumberFromSection(input, out var issueNumber))
            {
                parseResult = false;
            }
            if (!TryParseDurationFromSection(input, out var duration))
            {
                parseResult = false;
            }
            if (!TryParseDescriptionFromSection(input, out var description))
            {
                parseResult = false;
            }
            if (!TryParseLabelFromSection(input, out var label))
            {
                parseResult = false;
            }

            try
            {
                logEntry = parseResult 
                    ? new LogEntry(issueNumber ?? DefaultIssueNumber, duration, description, label, DateTimeOffset.Now)
                    : null;
                return parseResult;
            }
            catch (Exception)
            {
                logEntry = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to get label from input string.
        /// Label is optional.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        private bool TryParseLabelFromSection(string input, out string label)
        {
            label = _labelSelector.Get(input).InputPart;

            return true;
        }


        /// <summary>
        /// Tries to get description from input string.
        /// Description is required.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private bool TryParseDescriptionFromSection(string input, out string description)
        {
            description = _descriptionSelector.Get(input).InputPart;

            if (string.IsNullOrEmpty(description))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to get duration from input string.
        /// duration is required.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        private bool TryParseDurationFromSection(string input, out float duration)
        {
            var decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var durationString = _durationSelector.Get(input).InputPart;
            return float.TryParse(durationString?.Replace(",", decimalSeparator).Replace(".", decimalSeparator), out duration);
        }

        /// <summary>
        /// Tries to get issue number from input string.
        /// Issue number is not required.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="issueNumber"></param>
        /// <returns></returns>
        private bool TryParseIssueNumberFromSection(string input, out int? issueNumber)
        {
            var issueNumberString = _issueNumberSelector.Get(input).InputPart;

            if (string.IsNullOrEmpty(issueNumberString))
            {
                issueNumber = null;
                return true;
            }

            var parseResult = int.TryParse(issueNumberString, out var issueNumberParseResult);
            issueNumber = issueNumberParseResult;
            return parseResult;
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

            // Are some required input parts missing?
            if (countDurationSpecialChars != 1 || countDescriptionSpecialChars != 1)
            {
                return false;
            }

            return true;
        }
    }
}