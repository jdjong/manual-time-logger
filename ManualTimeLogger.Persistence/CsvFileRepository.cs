using System;
using System.IO;
using ManualTimeLogger.Domain;

namespace ManualTimeLogger.Persistence
{
    public class CsvFileRepository : IRepository
    {
        private const string FileBasePath = @"C:/temp/timelogs/";

        public void SaveLogEntry(LogEntry logEntry)
        {
            File.AppendAllText(
                $"C:/temp/timelogs/timelog_{DateTimeOffset.Now:yyyyMM}.csv", 
                $"{logEntry.IssueNumber};{logEntry.Duration};{logEntry.Description};{logEntry.Label};");
        }
    }
}
