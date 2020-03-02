using System;
using System.IO;
using System.Linq;

namespace ManualTimeLogger.ReportBuilder
{
    public class ReportCsvFileRepository
    {
        // TODO, unit test
        
        private const char CsvSeparator = ';';
        private readonly string _basePath;
        private readonly string _fileName;
        private readonly DateTime _firstDateOfReportPeriod;
        private readonly int _periodNrOfDays;
        private string FullFilePath => Path.Combine(_basePath, _fileName);

        public ReportCsvFileRepository(string basePath, string fileName, DateTime firstFirstDateOfReportPeriod, int periodNrOfDays)
        {
            if (string.IsNullOrEmpty(basePath)) throw new ArgumentNullException(basePath);
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(fileName);

            _basePath = basePath;
            _fileName = fileName;

            _firstDateOfReportPeriod = firstFirstDateOfReportPeriod;
            _periodNrOfDays = periodNrOfDays;

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
                new[] {$"\"Wie\"{CsvSeparator}\"Omschrijving\"{CsvSeparator}{string.Join(CsvSeparator.ToString(), Enumerable.Range(0, _periodNrOfDays).Select(nr => $"\"{_firstDateOfReportPeriod.AddDays(nr):yyyyMMdd}\""))}"});
        }

        public void SaveReportEntry(ReportEntry reportEntry)
        {
            if (_periodNrOfDays != reportEntry.PeriodNrOfDays)
            {
                throw new Exception($"report entry spans {reportEntry.PeriodNrOfDays} nr. of days while this repository is for {_periodNrOfDays} nr. of days");
            }
            
            File.AppendAllLines(
                FullFilePath, 
                new[] { $"\"{reportEntry.Engineer}\"{CsvSeparator}\"{reportEntry.Description}\"{CsvSeparator}{string.Join(CsvSeparator.ToString(), reportEntry.NrOfHoursPerWeekDay.Select(x => $"\"{x.Value}\""))}" });
        }
    }
}
