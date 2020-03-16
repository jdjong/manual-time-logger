using System;
using System.Globalization;
using ManualTimeLogger.Domain;
using NLog;

namespace ManualTimeLogger.Persistence
{
    public class CsvFileLogEntry
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly char _separator;
        public LogEntry AsDomainObject { get; }
        public string AsCsvLine { get; }

        public static string GetHeader(char separator)
        {
            return $"\"Account\"{separator}\"Issue\"{separator}\"Duration (hours)\"{separator}\"Description\"{separator}\"Label\"{separator}\"Activity\"{separator}\"Create date\"";
        }

        public CsvFileLogEntry(LogEntry domainObject, char separator)
        {
            _separator = separator;
            AsDomainObject = domainObject;
            AsCsvLine = ToCsvLine(domainObject, separator);
        }

        public CsvFileLogEntry(string csvLine, char separator)
        {
            _separator = separator;

            try
            {
                var logEntryPropertyStrings = csvLine.Split(separator);
                var accountString = logEntryPropertyStrings[0].Trim('"');
                var issueNumberString = logEntryPropertyStrings[1].Trim('"');
                var durationString = logEntryPropertyStrings[2].Trim('"');
                var descriptionString = logEntryPropertyStrings[3].Trim('"');
                var labelString = logEntryPropertyStrings[4].Trim('"');
                var activityString = logEntryPropertyStrings[5].Trim('"');
                var createDateString = logEntryPropertyStrings[6].Trim('"');

                var issueNumber = string.IsNullOrEmpty(issueNumberString) ? 0 : int.Parse(issueNumberString);
                var duration = float.Parse(GetCultureInvariantDurationString(durationString), CultureInfo.InvariantCulture.NumberFormat);
                var description = descriptionString;
                var label = labelString;
                var activity = activityString;
                var account = accountString;
                var createDate = new DateTime(int.Parse(createDateString.Substring(0,4)), int.Parse(createDateString.Substring(4,2)), int.Parse(createDateString.Substring(6,2)));

                AsDomainObject = new LogEntry(issueNumber, duration, description, label, activity, account, createDate);
                AsCsvLine = ToCsvLine(AsDomainObject, separator);
            }
            catch (Exception e)
            {
                Log.Info($"Input csv line is {csvLine} and separator is {separator}");
                throw;
            }
        }

        private static string GetCultureInvariantDurationString(string durationString)
        {
            return durationString.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator).Replace(".", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
        }

        private string ToCsvLine(LogEntry domainObject, char separator)
        {
            return $"\"{domainObject.Account}\"{separator}\"{domainObject.IssueNumber}\"{separator}\"{domainObject.Duration.ToString(CultureInfo.InvariantCulture.NumberFormat)}\"{separator}\"{domainObject.Description}\"{separator}\"{domainObject.Label}\"{separator}\"{domainObject.Activity}\"{separator}\"{domainObject.CreateDate:yyyyMMdd}\"";
        }
    }
}
