using System;

namespace ManualTimeLogger.Persistence
{
    public class CsvFileRepositoryFactory : IRepositoryFactory
    {
        private readonly string _timeLogBasePath;

        public CsvFileRepositoryFactory(string timeLogBasePath)
        {
            if (string.IsNullOrEmpty(timeLogBasePath)) throw new ArgumentException("You should provide a report base path", timeLogBasePath);
            _timeLogBasePath = timeLogBasePath;
        }

        public IRepository Create(string fileName)
        {
            return new CsvFileRepository(_timeLogBasePath, fileName);
        }

        public string GetFilesBasePath()
        {
            return _timeLogBasePath;
        }
    }
}
