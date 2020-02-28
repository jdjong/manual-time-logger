using System;
using System.IO;
using System.Linq;

namespace ManualTimeLogger.ReportBuilder
{
    public class ReportWeekCsvFileRepository
    {
        // TODO, unit test
        
        private const char CsvSeparator = ';';
        private readonly string _basePath;
        private readonly string _fileName;
        private readonly DateTime _dateOfMondayForRequestedWeek;
        private string FullFilePath => Path.Combine(_basePath, _fileName);

        public ReportWeekCsvFileRepository(string basePath, string fileName, DateTime aDateForTheRequestedWeek)
        {
            if (string.IsNullOrEmpty(basePath)) throw new ArgumentNullException(basePath);
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(fileName);

            _basePath = basePath;
            _fileName = fileName;

            _dateOfMondayForRequestedWeek = GetMondayDateForRequestedWeek(aDateForTheRequestedWeek);

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

        // TODO, remove duplication in report week entry class
        private DateTime GetMondayDateForRequestedWeek(DateTime aDateForTheRequestedWeek)
        {
            var dateIterator = aDateForTheRequestedWeek;
            while (dateIterator.DayOfWeek != DayOfWeek.Monday)
            {
                dateIterator = dateIterator.AddDays(-1);
            }

            return dateIterator;
        }

        private void CreateFileWithHeaderLine()
        {
            File.AppendAllLines(
                FullFilePath,
                new[] {$"\"Wie\"{CsvSeparator}\"Omschrijving\"{CsvSeparator}{string.Join(CsvSeparator.ToString(), Enumerable.Range(0, 7).Select(nr => $"\"{_dateOfMondayForRequestedWeek.AddDays(nr):yyyyMMdd}\""))}"});
        }

        public void SaveReportEntry(ReportWeekEntry reportEntry)
        {
            File.AppendAllLines(
                FullFilePath, 
                new[] { $"\"{reportEntry.Engineer}\"{CsvSeparator}\"{reportEntry.Description}\"{CsvSeparator}{string.Join(CsvSeparator.ToString(), reportEntry.NrOfHoursPerWeekDay.Select(x => $"\"{x.Value}\""))}" });
        }
    }
}
