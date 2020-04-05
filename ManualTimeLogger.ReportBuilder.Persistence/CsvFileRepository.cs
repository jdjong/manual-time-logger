using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ManualTimeLogger.ReportBuilder.Persistence
{
    public class CsvFileRepository : IRepository
    {
        public char CsvSeparator => ';';
        private readonly string _basePath;
        private readonly string _fileName;
        private readonly DateTime _firstDayOfReport;
        private readonly int _periodNrOfDays;
        private int _reportColumnCount;
        private string FullFilePath => Path.Combine(_basePath, _fileName);

        public CsvFileRepository(string basePath, string fileName, DateTime firstDayOfReport, int periodNrOfDays)
        {
            if (string.IsNullOrEmpty(basePath)) throw new ArgumentNullException(basePath);
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(fileName);

            _basePath = basePath;
            _fileName = fileName;
            _firstDayOfReport = firstDayOfReport;
            _periodNrOfDays = periodNrOfDays;

            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }

            if (File.Exists(FullFilePath))
            {
                File.Delete(FullFilePath);
            }

            CreateHeader();
        }

        private void CreateHeader()
        {
            var headerString = $"\"GroupedBy\"{CsvSeparator}\"ThenGroupedBy\"{CsvSeparator}\"Totaal\"{CsvSeparator}{string.Join(CsvSeparator.ToString(), Enumerable.Range(0, _periodNrOfDays).Select(nr => $"\"{_firstDayOfReport.AddDays(nr):yyyyMMdd}\""))}";
            _reportColumnCount = headerString.Split(CsvSeparator).Length;

            File.AppendAllLines(
                FullFilePath,
                new []
                {
                    headerString
                });
        }

        public void SaveReportEntry(ReportEntry reportEntry)
        {
            var reportEntryString = $"\"{reportEntry.GroupedBy}\"{CsvSeparator}\"{reportEntry.ThenGroupedBy}\"{CsvSeparator}\"{Enumerable.Sum<KeyValuePair<DateTime, float>>(reportEntry.NrOfHoursPerDay, x => x.Value)}\"{CsvSeparator}{string.Join(CsvSeparator.ToString(), Enumerable.Select<KeyValuePair<DateTime, float>, string>(reportEntry.NrOfHoursPerDay, hoursForDay => $"\"{GetHoursForDayString(hoursForDay)}\""))}";
            var reportEntryColumnCount = reportEntryString.Split(CsvSeparator).Length;

            if (reportEntryColumnCount != _reportColumnCount)
            {
                throw new ArgumentException($"Report entry column count {reportEntryColumnCount} does not match intended report column count {_reportColumnCount}", nameof(reportEntry));
            }

            File.AppendAllLines(
                FullFilePath,
                new[]
                {
                    reportEntryString
                });
        }

        private string GetHoursForDayString(KeyValuePair<DateTime, float> hoursForDay)
        {
            return hoursForDay.Value < 0.01f
                ? string.Empty
                : Math.Round(hoursForDay.Value, 2, MidpointRounding.AwayFromZero).ToString("0.00");
        }
    }
}
