using System;
using System.Globalization;
using ManualTimeLogger.Domain;
using NUnit.Framework;

namespace ManualTimeLogger.Persistence.Tests
{
    [TestFixture]
    public class CsvFileLogEntryTests
    {
        [Test]
        public void given_a_log_entry_csv_line_when_creating_the_csv_file_log_entry_then_the_line_string_and_domain_object_are_the_same()
        {
            var decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var csvLine = $"\"12345\";\"1{decimalSeparator}25\";\"TestDescription\";\"TestLabel\";\"anders\";\"20200201\"";
            var csvFileLogEntry = new CsvFileLogEntry(csvLine, ';');
            var csvFileLogEntry2 = new CsvFileLogEntry(csvFileLogEntry.AsDomainObject, ';');

            Assert.AreEqual(csvFileLogEntry.AsDomainObject, csvFileLogEntry2.AsDomainObject, "LogEntry comparison");
            Assert.AreEqual(csvFileLogEntry.AsCsvLine, csvFileLogEntry2.AsCsvLine, "LogEntryAsString comparison");
        }

        [Test]
        public void given_a_log_entry_object_when_creating_the_csv_file_log_entry_then_the_line_string_and_domain_object_are_the_same()
        {
            var logEntry = new LogEntry(12345, 1.25f, "TestDescription", "TestLabel", "mailen", new DateTime(2020,2,1));
            
            var csvFileLogEntry = new CsvFileLogEntry(logEntry, ';');
            var csvFileLogEntry2 = new CsvFileLogEntry(csvFileLogEntry.AsCsvLine, ';');

            Assert.AreEqual(csvFileLogEntry.AsDomainObject, csvFileLogEntry2.AsDomainObject, "LogEntry comparison");
            Assert.AreEqual(csvFileLogEntry.AsCsvLine, csvFileLogEntry2.AsCsvLine, "LogEntryAsString comparison");
        }

        // TODO, add tests for locality difference for decimal separator. For the case when it is or is not your locality setting. Should work for both.
    }
}
