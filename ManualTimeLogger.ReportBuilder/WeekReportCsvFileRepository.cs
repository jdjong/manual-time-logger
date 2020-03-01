using System;
using System.IO;
using System.Linq;

namespace ManualTimeLogger.ReportBuilder
{
    public class WeekReportCsvFileRepository
    {
        // TODO, unit test
        
        private const char CsvSeparator = ';';
        private readonly string _basePath;
        private readonly string _fileName;
        private readonly DateTime _dateOfMondayOfRequestedWeek;
        private string FullFilePath => Path.Combine(_basePath, _fileName);

        public WeekReportCsvFileRepository(string basePath, string fileName, DateTime dateOfMondayOfRequestedWeek)
        {
            if (string.IsNullOrEmpty(basePath)) throw new ArgumentNullException(basePath);
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(fileName);

            _basePath = basePath;
            _fileName = fileName;

            if (dateOfMondayOfRequestedWeek.DayOfWeek != DayOfWeek.Monday)
            {
                throw new ArgumentException("Date needs to be a monday", nameof(dateOfMondayOfRequestedWeek));
            }

            _dateOfMondayOfRequestedWeek = dateOfMondayOfRequestedWeek;

            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }

            if (!File.Exists(FullFilePath))
            {
                CreateFileWithHeaderLine();
            }
            else
            {
                File.Delete(FullFilePath);
                CreateFileWithHeaderLine();
            }
        }

        private void CreateFileWithHeaderLine()
        {
            File.AppendAllLines(
                FullFilePath,
                new[] {$"\"Wie\"{CsvSeparator}\"Omschrijving\"{CsvSeparator}{string.Join(CsvSeparator.ToString(), Enumerable.Range(0, 7).Select(nr => $"\"{_dateOfMondayOfRequestedWeek.AddDays(nr):yyyyMMdd}\""))}"});
        }

        public void SaveReportEntry(WeekReportEntry reportEntry)
        {
            File.AppendAllLines(
                FullFilePath, 
                new[] { $"\"{reportEntry.Engineer}\"{CsvSeparator}\"{reportEntry.Description}\"{CsvSeparator}{string.Join(CsvSeparator.ToString(), reportEntry.NrOfHoursPerWeekDay.Select(x => $"\"{x.Value}\""))}" });
        }
    }
}
