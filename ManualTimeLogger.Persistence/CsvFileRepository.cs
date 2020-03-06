using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManualTimeLogger.Domain;

namespace ManualTimeLogger.Persistence
{
    public class CsvFileRepository : IRepository
    {
        private const char CsvSeparator = ';';
        private readonly string _basePath;
        private readonly string _fileName;
        private string FullFilePath => Path.Combine(_basePath, _fileName);

        public CsvFileRepository(string basePath, string fileName)
        {
            if (string.IsNullOrEmpty(basePath)) throw new ArgumentNullException(basePath);
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(fileName);

            _basePath = basePath;
            _fileName = fileName;

            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
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

        public IEnumerable<LogEntry> GetAllLogEntries()
        {
            var csvLines = File.ReadAllLines(FullFilePath);

            return csvLines
                .Skip(1) // skip header
                .Select(csvLine => new CsvFileLogEntry(csvLine, CsvSeparator).AsDomainObject);
        }

        // TODO, improve code, test code and to interface
        public string GetEngineerName()
        {
            return _fileName.Split('_')[0];
        }
    }
}
