using System;
using System.IO;
using System.Linq;
using ManualTimeLogger.Domain;
using NUnit.Framework;

namespace ManualTimeLogger.Persistence.Tests
{
    [TestFixture]
    public class CsvFileRepositoryTests
    {
        private readonly string _testFileLocation = Environment.CurrentDirectory;
        private string _testFileName = "name_timelog_testfile.csv";
        private CsvFileRepository _repository;
        private string _absoluteFilePath;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _absoluteFilePath = Path.Combine(_testFileLocation, _testFileName);
            RemoveTestFileIfExists();
        }

        [SetUp]
        public void SetUp()
        {
            _repository = new CsvFileRepository(_testFileLocation, _testFileName);
        }

        [Test]
        public void given_a_repository_when_alive_then_the_file_with_one_header_line_is_created()
        {
            Assert.True(File.Exists(_absoluteFilePath), "File exists");

            var linesInFile = File.ReadAllLines(_absoluteFilePath);
            Assert.AreEqual(1, linesInFile.Length, "Nr of lines in file");

            var engineerName = _repository.GetEngineerName();
            Assert.AreEqual(_testFileName.Split('_')[0], engineerName, "Engineer name");
        }

        [Test]
        public void given_a_log_entry_when_saved_then_it_is_persisted_in_the_file()
        {
            var logEntry = new LogEntry(12345, 1.25f, "description", "label", "activity", "nb", new DateTime(2020,3,1));
            _repository.SaveLogEntry(logEntry);

            var linesInFile = File.ReadAllLines(_absoluteFilePath);
            Assert.AreEqual(2, linesInFile.Length, "Nr of lines in file");

            var allLogEntries = _repository.GetAllLogEntries();
            Assert.AreEqual(1, allLogEntries.Count(), "Nr of log entries in file");

            Assert.AreEqual(logEntry, allLogEntries.FirstOrDefault(), "Saved log entry equals retrieved log entry");
        }

        [Test]
        public void given_a_few_saved_log_entries_for_the_same_day_then_the_total_number_of_hours_logged_for_that_day_equals_the_sum_of_the_duration_of_the_entries()
        {
            var logDate = new DateTime(2020, 3, 1);
            var logEntry1 = new LogEntry(12345, 1.25f, "description", "label", "activity", "nb", logDate);
            var logEntry2 = new LogEntry(12345, 2.25f, "description", "label", "activity", "nb", logDate);
            var logEntry3 = new LogEntry(12345, 1f, "description", "label", "activity", "nb", logDate);

            _repository.SaveLogEntry(logEntry1);
            _repository.SaveLogEntry(logEntry2);
            _repository.SaveLogEntry(logEntry3);

            Assert.AreEqual(logEntry1.Duration + logEntry2.Duration + logEntry3.Duration, _repository.GetTotalLoggedHoursForDate(logDate));
        }

        [TearDown]
        public void RemoveTestFile()
        {
            RemoveTestFileIfExists();

        }

        private void RemoveTestFileIfExists()
        {
            if (File.Exists(_absoluteFilePath))
            {
                File.Delete(_absoluteFilePath);
            }
        }
    }
}
