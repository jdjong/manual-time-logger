using NUnit.Framework;

namespace ManualTimeLogger.Domain.Tests
{
    [TestFixture]
    public class LogEntryInputParserTests
    {
        private LogEntryInputParser _logEntryInputParser;

        [SetUp]
        public void Setup()
        {
            _logEntryInputParser = new LogEntryInputParser();
        }

        [Test]
        [TestCase("*4.5 $some text and more #1234 *2")]
        [TestCase("*4.5 $some text and more #1234 #2")]
        [TestCase("*4.5 $some text and more #1234 $2")]
        [TestCase("$some text and more #1234")]
        [TestCase("*4.5 #1234")]
        [TestCase("")]
        public void wrong_input_section_layouts(string input)
        {
            Assert.IsFalse(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("*4.5 $some text and more #1234")]
        [TestCase("$some text and more *8")]
        [TestCase("*5 #1234 $some text and more")]
        public void correct_input_section_layouts(string input)
        {
            Assert.IsTrue(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("#-1234 *4.5 $some text and more")]
        [TestCase("#123.4 *4.5 $some text and more")]
        [TestCase("#text *4.5 $some text and more")]
        [TestCase("# *4.5 $some text and more")]
        public void wrong_issue_number_sections(string input)
        {
            Assert.IsFalse(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("#1234 *4.5 $some text and more", 1234)]
        [TestCase("#0 *4.5 $some text and more", 0)]
        public void correct_issue_number_sections(string input, int expectedResult)
        {
            Assert.IsTrue(_logEntryInputParser.TryParse(input, out var logEntry));
            Assert.AreEqual(expectedResult, logEntry.IssueNumber);
        }

        [Test]
        [TestCase("#1234 *4.5. $some text and more")]
        [TestCase("#1234 *-4.5 $some text and more")]
        [TestCase("#1234 *text $some text and more")]
        [TestCase("#1234 *0 $some text and more")]
        [TestCase("#1234 * $some text and more")]
        public void wrong_duration_sections(string input)
        {
            Assert.IsFalse(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("#1234 *4.5 $some text and more", 4.5f)]
        [TestCase("#1234 *4,5 $some text and more", 4.5f)]
        [TestCase("#1234 *4 $some text and more", 4)]
        [TestCase("#1234 *0.1 $some text and more", 0.1f)]
        public void correct_duration_sections(string input, float expectedResult)
        {
            Assert.IsTrue(_logEntryInputParser.TryParse(input, out var logEntry));
            Assert.AreEqual(expectedResult, logEntry.Duration);
        }

        [Test]
        [TestCase("#1234 *4.5 ")]
        [TestCase("#1234 *4.5 $")]
        [TestCase("#1234 *4.5 $$")]
        public void wrong_description_sections(string input)
        {
            Assert.IsFalse(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("#1234 *4.5 $some text 4 and more 3.5", "some text 4 and more 3.5")]
        [TestCase("#1234 $some text 4 and more 3.5 *4.5 more text", "some text 4 and more 3.5  more text")]
        [TestCase("$some text 4 and more 3.5 *4.5 more text", "some text 4 and more 3.5  more text")]
        public void correct_description_sections(string input, string expectedResult)
        {
            Assert.IsTrue(_logEntryInputParser.TryParse(input, out var logEntry));
            Assert.AreEqual(expectedResult, logEntry.Description);
        }
    }
}