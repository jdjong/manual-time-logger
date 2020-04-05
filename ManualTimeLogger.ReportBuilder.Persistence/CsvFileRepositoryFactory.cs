using System;

namespace ManualTimeLogger.ReportBuilder.Persistence
{
    public class CsvFileRepositoryFactory : IRepositoryFactory
    {
        private readonly string _reportsBasePath;
        private readonly string _fileNamePrefix;

        public CsvFileRepositoryFactory(string reportsBasePath, string fileNamePrefix)
        {
            if (string.IsNullOrEmpty(reportsBasePath)) throw new ArgumentException("You should provide a report base path", reportsBasePath);
            if (string.IsNullOrEmpty(fileNamePrefix)) throw new ArgumentException("You should provide a file name prefix", fileNamePrefix);
            _reportsBasePath = reportsBasePath;
            _fileNamePrefix = fileNamePrefix;
        }

        public IRepository Create(string fileName, DateTime reportStartDate, int reportNrOfDays)
        {
            return new CsvFileRepository(_reportsBasePath, $"{_fileNamePrefix}_{fileName}", reportStartDate, reportNrOfDays);
        }
    }
}
