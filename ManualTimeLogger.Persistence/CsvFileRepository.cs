using System;
using System.IO;
using ManualTimeLogger.Domain;

namespace ManualTimeLogger.Persistence
{
    public class CsvFileRepository : IRepository
    {
        private const string FileBasePath = @"C:/temp/timelogs/";
        private string FullFilePath => FileBasePath + $"timelog_{DateTimeOffset.Now:yyyyMM}.csv";

        public CsvFileRepository()
        {
            if (!Directory.Exists(FileBasePath))
            {
                Directory.CreateDirectory(FileBasePath);
            }

            if (!File.Exists(FullFilePath))
            {
                File.AppendAllLines(
                    FullFilePath, 
                    new[] {$"\"Issue\";\"Duration (hours)\";\"Description\";\"Label\";"});
            }
        }

        public void SaveLogEntry(LogEntry logEntry)
        {
            File.AppendAllLines(
                FullFilePath, 
                new[] {$"\"{logEntry.IssueNumber}\";\"{logEntry.Duration}\";\"{logEntry.Description}\";\"{logEntry.Label}\";\"{logEntry.CreateDate.ToString("yyyyMMdd")}\""});
        }
    }
}
