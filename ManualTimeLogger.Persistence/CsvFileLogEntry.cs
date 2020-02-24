using System;
using ManualTimeLogger.Domain;

namespace ManualTimeLogger.Persistence
{
    public class CsvFileLogEntry
    {
        private readonly char _separator;
        public LogEntry AsDomainObject { get; }
        public string AsCsvLine { get; }

        public static string GetHeader(char separator)
        {
            return $"\"Issue\"{separator}\"Duration (hours)\"{separator}\"Description\"{separator}\"Label\"{separator}\"Activity\"{separator}\"Create date\"";
        }

        public CsvFileLogEntry(LogEntry domainObject, char separator)
        {
            _separator = separator;
            AsDomainObject = domainObject;
            AsCsvLine = $"\"{domainObject.IssueNumber}\"{separator}\"{domainObject.Duration}\"{separator}\"{domainObject.Description}\"{separator}\"{domainObject.Label}\"{separator}\"{domainObject.Activity.ToString()}\"{separator}\"{domainObject.CreateDate:yyyyMMdd}\"";
        }

        public CsvFileLogEntry(string csvLine, char separator)
        {
            _separator = separator;
            AsCsvLine = csvLine;
            
            var logEntryPropertyStrings = csvLine.Split(separator);
            var issueNumberString = logEntryPropertyStrings[0].Trim('"');
            var durationString = logEntryPropertyStrings[1].Trim('"');
            var descriptionString = logEntryPropertyStrings[2].Trim('"');
            var labelString = logEntryPropertyStrings[3].Trim('"');
            var activityString = logEntryPropertyStrings[4].Trim('"');
            var createDateString = logEntryPropertyStrings[5].Trim('"');

            // TODO, do I need to do some validation since it is intended and possible to change the csv manually? Probably...
            var issueNumber = string.IsNullOrEmpty(issueNumberString) ? 0 : int.Parse(issueNumberString);
            var duration = float.Parse(durationString);
            var description = descriptionString;
            var label = labelString;
            Enum.TryParse<Activity>(activityString?.ToLower(), out var activity);
            var createDate = new DateTime(int.Parse(createDateString.Substring(0,4)), int.Parse(createDateString.Substring(4,2)), int.Parse(createDateString.Substring(6,2)));

            AsDomainObject = new LogEntry(issueNumber, duration, description, label, activity, createDate);
        }
    }
}
