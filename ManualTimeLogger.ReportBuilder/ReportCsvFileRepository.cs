using System;
using System.IO;
using System.Linq;

namespace ManualTimeLogger.ReportBuilder
{
    public class ReportCsvFileRepository
    {
        // TODO, unit test
        
        public const char CsvSeparator = ';';
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
                new[] { $"\"{reportEntry.Engineer}\"{CsvSeparator}\"{reportEntry.Description}\"{CsvSeparator}{string.Join(CsvSeparator.ToString(), reportEntry.NrOfHoursPerDay.Select(x => $"\"{x.Value}\""))}" });
        }
    }
}
