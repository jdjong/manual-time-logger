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
        private const int DefaultIssueNumber = 0;

        public bool TryParse(string input, out LogEntry logEntry)
        {
            var parseResult = IsInputSectionsLayoutValid(input);

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

            try
            {
                logEntry = parseResult 
                    ? new LogEntry(issueNumber ?? DefaultIssueNumber, duration, description, DateTimeOffset.Now)
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
        /// Tries to get description from input string.
        /// Description is required.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private bool TryParseDescriptionFromSection(string input, out string description)
        {
            description = GetDescriptionInputSection(input)?.Replace(DescriptionSpecialChar, "");

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
            return float.TryParse(GetDurationInputSection(input)?.Replace(DurationSpecialChar, "").Replace(",", decimalSeparator).Replace(".", decimalSeparator), out duration);
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
            var issueNumberInputSection = GetIssueNumberInputSection(input);

            if (string.IsNullOrEmpty(issueNumberInputSection))
            {
                issueNumber = null;
                return true;
            }

            var parseResult = int.TryParse(issueNumberInputSection.Replace(IssueNumberSpecialChar, ""), out var issueNumberParseResult);
            issueNumber = issueNumberParseResult;
            return parseResult;
        }

        /// <summary>
        /// Get description input section by removing the
        /// duration and issue entry sections and space
        /// trimming the ends of this string:
        ///
        /// input:                          #12345 *1.3 $a description you know
        /// remove duration section:        #12345  $a description you know
        /// remove issue number section:      $a description you know
        /// trim spaces:                    $a description you know
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GetDescriptionInputSection(string input)
        {
            var descriptionInputSection = input;
            var issueNumberInputSection = GetIssueNumberInputSection(input);
            var durationInputSection = GetDurationInputSection(input);

            if (!string.IsNullOrEmpty(issueNumberInputSection))
            {
                descriptionInputSection = descriptionInputSection.Replace(issueNumberInputSection, "");
            }

            if (!string.IsNullOrEmpty(durationInputSection))
            {
                descriptionInputSection = descriptionInputSection.Replace(durationInputSection, "");
            }

            return descriptionInputSection.Trim(' ');
        }

        /// <summary>
        /// Get duration input section by getting the substring
        /// of the entire input from the appropriate start symbol
        /// and taking the first part of the substring split by
        /// spaces:
        ///
        /// input:     #12345 *1.3 $a description you know
        /// substring:        *1.3 $a description you know
        /// split:            *1.3
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GetDurationInputSection(string input)
        {
            var startIndex = input.IndexOf(DurationSpecialChar, StringComparison.Ordinal);
            return startIndex > -1
                ? input.Substring(startIndex).Split(' ')[0]
                : null;
        }

        /// <summary>
        /// Get issue number entry input section by getting the substring
        /// of the entire input from the appropriate start symbol
        /// and taking the first part of the substring split by
        /// spaces:
        ///
        /// input:     #12345 *1.3 $a description you know
        /// substring: #12345 *1.3 $a description you know
        /// split:     #12345
        /// </summary>
        private string GetIssueNumberInputSection(string input)
        {
            var startIndex = input.IndexOf(IssueNumberSpecialChar, StringComparison.Ordinal);
            return startIndex > -1
                ? input.Substring(startIndex).Split(' ')[0]
                : null;
        }

        /// <summary>
        /// Checks if the required sections are provided
        /// and sections occur once at maximum.
        /// </summary>
        /// <param name="input"></param>
        private bool IsInputSectionsLayoutValid(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var countDescriptionStartSymbols = input.Count(character => character.ToString() == DescriptionSpecialChar);
            var countDurationStartSymbols = input.Count(character => character.ToString() == DurationSpecialChar);
            var countIssueNumberStartSymbols = input.Count(character => character.ToString() == IssueNumberSpecialChar);
            if (countDescriptionStartSymbols > 1 ||
                countDurationStartSymbols > 1 ||
                countIssueNumberStartSymbols > 1)
            {
                return false;
            }

            if (countDurationStartSymbols != 1 || countDescriptionStartSymbols != 1)
            {
                return false;
            }

            return true;
        }
    }
}