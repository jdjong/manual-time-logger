using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ManualTimeLogger.ReportBuilder
{
    public class ReportCsvFileRepository : IReportCsvFileRepository
    {
        public char CsvSeparator => ';';
        private readonly string _basePath;
        private readonly string _fileName;
        private string FullFilePath => Path.Combine(_basePath, _fileName);

        public ReportCsvFileRepository(string basePath, string fileName)
        {
            if (string.IsNullOrEmpty(basePath)) throw new ArgumentNullException(basePath);
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(fileName);

            _basePath = basePath;
            _fileName = fileName;

            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }

            if (File.Exists(FullFilePath))
            {
                File.Delete(FullFilePath);
            }
        }

        // TODO, it is strange that the calling party should know and create the header with number of columns matching the number of columns created in the save report entry method.
        public void CreateHeader(string[] header)
        {
            File.AppendAllLines(
                FullFilePath,
                header);
        }

        public void SaveReportEntry(ReportEntry reportEntry)
        {
            File.AppendAllLines(
                FullFilePath,
                new[]
                {
                    $"\"{reportEntry.Engineer}\"{CsvSeparator}\"{reportEntry.Description}\"{CsvSeparator}\"{reportEntry.NrOfHoursPerDay.Sum(x => x.Value)}\"{CsvSeparator}{string.Join(CsvSeparator.ToString(), reportEntry.NrOfHoursPerDay.Select(hoursForDay => $"\"{GetHoursForDayString(hoursForDay)}\""))}"
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
