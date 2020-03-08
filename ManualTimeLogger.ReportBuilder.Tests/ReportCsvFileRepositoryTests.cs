using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace ManualTimeLogger.ReportBuilder.Tests
{
    [TestFixture]
    public class ReportCsvFileRepositoryTests
    {
        private readonly string _testFileLocation = Environment.CurrentDirectory;
        private string _testFileName = "name_report_testfile.csv";
        private ReportCsvFileRepository _repository;
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
            _repository = new ReportCsvFileRepository(_testFileLocation, _testFileName);
            _repository.CreateHeader(new []{"a header"});
        }

        [Test]
        public void given_a_repository_when_alive_then_the_file_with_one_header_line_is_created()
        {
            Assert.True(File.Exists(_absoluteFilePath), "File exists");

            var linesInFile = File.ReadAllLines(_absoluteFilePath);
            Assert.AreEqual(1, linesInFile.Length, "Nr of lines in file");
        }

        [Test]
        public void given_a_report_entry_when_saved_then_it_is_persisted_in_the_file()
        {
            var reportEntry = new ReportEntry("TestEngineer", "Description", new DateTime(2020, 3, 1), 3, new Dictionary<DateTime, float>());
            _repository.SaveReportEntry(reportEntry);

            var linesInFile = File.ReadAllLines(_absoluteFilePath);
            Assert.AreEqual(2, linesInFile.Length, "Nr of lines in file");
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
