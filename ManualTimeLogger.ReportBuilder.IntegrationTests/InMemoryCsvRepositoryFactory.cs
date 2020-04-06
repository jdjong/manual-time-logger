using System;
using System.Collections.Generic;
using ManualTimeLogger.ReportBuilder.Persistence;

namespace ManualTimeLogger.ReportBuilder.IntegrationTests
{
    public class InMemoryCsvRepositoryFactory : IRepositoryFactory
    {
        private readonly string _fileNamePrefix;

        // TODO, interface requires method with file name. Hmmm, not properly interfaced. Rename interface to file repository
        public List<InMemoryCsvRepository> Repositories { get; }

        public InMemoryCsvRepositoryFactory(string fileNamePrefix)
        {
            _fileNamePrefix = fileNamePrefix;
            Repositories = new List<InMemoryCsvRepository>();
        }

        public IRepository Create(string fileName, DateTime reportStartDate, int reportNrOfDays)
        {
            var inMemoryCsvRepository = new InMemoryCsvRepository($"{_fileNamePrefix}_{fileName}", reportStartDate, reportNrOfDays);
            Repositories.Add(inMemoryCsvRepository);
            return inMemoryCsvRepository;
        }
    }
}
