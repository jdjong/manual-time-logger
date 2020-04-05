using ManualTimeLogger.ReportBuilder.Commands;
using ManualTimeLogger.ReportBuilder.Persistence;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace ManualTimeLogger.ReportBuilder.IntegrationTests
{
    [TestFixture]
    public class FromTimeLogToReportsTests
    {
        private ManualTimeLogger.Persistence.IRepositoryFactory _timeLoggerRepositoryFactory;
        private string _expectedReportFilesLocation;

        [OneTimeSetUp]
        public void SetTestTimeLogLocation()
        {
            var inputTestFilesLocation = Path.Combine(TestContext.CurrentContext.TestDirectory, "InputTimeLogs");
            _expectedReportFilesLocation = Path.Combine(TestContext.CurrentContext.TestDirectory, "ExpectedReports");
            _timeLoggerRepositoryFactory = new ManualTimeLogger.Persistence.CsvFileRepositoryFactory(inputTestFilesLocation);
        }

        [Test]
        public void TestTimeLoggerRepositoryFactory()
        {
            var repo1 = _timeLoggerRepositoryFactory.Create("engineer1_timelog_202003.csv");
            var allLogEntries1 = repo1.GetAllLogEntries();
            Assert.IsNotEmpty(allLogEntries1, "engineer1_timelog_202003.csv is read properly");

            var repo2 = _timeLoggerRepositoryFactory.Create("engineer2iscopyof1_timelog_202003.csv");
            var allLogEntries2 = repo2.GetAllLogEntries();
            Assert.IsNotEmpty(allLogEntries2, "engineer2iscopyof1_timelog_202003.csv is read properly");
        }

        [Test]
        public void TestReadingExpectedReports()
        {
            var expectedReportFileNames = Directory.EnumerateFiles(_expectedReportFilesLocation);

            foreach (var fileName in expectedReportFileNames)
            {
                var fileLines = File.ReadAllLines(Path.Combine(_expectedReportFilesLocation,fileName)).ToList();
                Assert.IsNotEmpty(fileLines, $"Reading file {fileName}");
            }
        }

        // TODO, check if test is complete and clarify what is happening/tested
        // TODO, add account specific tests
        // TODO, check the expected files with the input files manually, because I only performed a steekproef

        [Test]
        public void given_two_time_logs_and_no_account_filter_when_building_month_reports_then_the_expected_month_reports_are_build()
        {
            var firstDayOfPeriod = new DateTime(2020, 3, 1);
            var reportRepositoryFactory = new InMemoryCsvRepositoryFactory("all");

            var command = new BuildMonthReportsCommand(firstDayOfPeriod);
            var commandHandler = new CommandHandler(_timeLoggerRepositoryFactory, reportRepositoryFactory);

            commandHandler.Handle(command);

            var expectedReportFullFilePaths = Directory.EnumerateFiles(_expectedReportFilesLocation).ToList();
            var repositoryFileNames = reportRepositoryFactory.Repositories
                .Select(x => x.FileName)
                .ToList();

            foreach (var expectedReportFullFilePath in expectedReportFullFilePaths)
            {
                var expectedFileName = Path.GetFileName(expectedReportFullFilePath);
                // ReSharper disable once PossibleNullReferenceException
                if (!expectedFileName.StartsWith("all")) // no account filter, thus test only files starting with all
                {
                    continue;
                }

                Assert.IsTrue(repositoryFileNames.Any(fileName => fileName == expectedFileName));
                
                var expectedFileLines = File.ReadAllLines(Path.Combine(_expectedReportFilesLocation, expectedFileName)).ToList();
                var actualFileLines = reportRepositoryFactory.Repositories.Single(x => x.FileName == expectedFileName).SavedReportEntries;

                Assert.True(expectedFileLines.Count > 0, "Count of expected lines");
                Assert.True(actualFileLines.Count > 0, "Count of actual lines");
                Assert.True(expectedFileLines.SequenceEqual(actualFileLines), $"Compare expected with actual for {expectedFileName}");
            }
        }
    }
}
