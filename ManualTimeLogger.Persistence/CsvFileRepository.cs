using System;
using System.IO;
using System.Linq;
using ManualTimeLogger.Domain;

namespace ManualTimeLogger.Persistence
{
    public class CsvFileRepository : IRepository
    {
        private const char CsvSeparator = ';';
        private const string FileBasePath = @"C:/temp/timelogs/";
        private string FullFilePath => FileBasePath + $"timelog_{DateTime.Today:yyyyMM}.csv";

        public CsvFileRepository()
        {
            if (!Directory.Exists(FileBasePath))
            {
                Directory.CreateDirectory(FileBasePath);
            }

            if (!File.Exists(FullFilePath))
            {
                CreateFileWithHeaderLine();
            }
        }

        private void CreateFileWithHeaderLine()
        {
            File.AppendAllLines(
                FullFilePath,
                new[] {CsvFileLogEntry.GetHeader(CsvSeparator)});
        }

        public void SaveLogEntry(LogEntry logEntry)
        {
            File.AppendAllLines(
                FullFilePath, 
                new[] {new CsvFileLogEntry(logEntry, CsvSeparator).AsCsvLine });
        }

        public float GetTotalLoggedHoursForDate(DateTime date)
        {
            var csvLines = File.ReadAllLines(FullFilePath);

            return csvLines
                .Skip(1) // skip header
                .Select(csvLine => new CsvFileLogEntry(csvLine, CsvSeparator))
                .Where(csvFileLogEntry => csvFileLogEntry.AsDomainObject.CreateDate == date)
                .Sum(csvFileLogEntry => csvFileLogEntry.AsDomainObject.Duration);
        }
    }
}
