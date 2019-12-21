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
        public void wrong_input_input_layouts(string input)
        {
            Assert.IsFalse(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("*4.5 $some text and more #1234")]
        [TestCase("$some text and more *8")]
        [TestCase("*5 #1234 $some text and more")]
        [TestCase("*4.5$some text and more#1234")]
        [TestCase("$some text and more*8")]
        [TestCase("*5#1234$some text and more")]
        public void correct_input_input_layouts(string input)
        {
            Assert.IsTrue(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("#-1234 *4.5 $some text and more")]
        [TestCase("#123.4 *4.5 $some text and more")]
        [TestCase("#text *4.5 $some text and more")]
        public void wrong_issue_number_inputs(string input)
        {
            Assert.IsFalse(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("#1234 *4.5 $some text and more", 1234)]
        [TestCase("#0 *4.5 $some text and more", 0)]
        [TestCase("# *4.5 $some text and more", 0)]
        [TestCase("#1234*4.5$some text and more", 1234)]
        [TestCase("#0*4.5$some text and more", 0)]
        [TestCase("#*4.5$some text and more", 0)]
        public void correct_issue_number_inputs(string input, int expectedResult)
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
        public void wrong_duration_inputs(string input)
        {
            Assert.IsFalse(_logEntryInputParser.TryParse(input, out _));
        }

        /// <summary>
        /// Decimal separator can be a . or ,; it does not matter
        /// </summary>
        /// <param name="input"></param>
        /// <param name="expectedResult"></param>
        [Test]
        [TestCase("#1234 *4.5 $some text and more", 4.5f)]
        [TestCase("#1234 *4,5 $some text and more", 4.5f)]
        [TestCase("#1234 *4 $some text and more", 4)]
        [TestCase("#1234 *0.1 $some text and more", 0.1f)]
        [TestCase("#1234*4.5$some text and more", 4.5f)]
        [TestCase("#1234*4,5$some text and more", 4.5f)]
        [TestCase("#1234*4 $some text and more", 4)]
        [TestCase("#1234*0.1$some text and more", 0.1f)]
        public void correct_duration_inputs(string input, float expectedResult)
        {
            Assert.IsTrue(_logEntryInputParser.TryParse(input, out var logEntry));
            Assert.AreEqual(expectedResult, logEntry.Duration);
        }

        /// <summary>
        /// Description ($) is required and should have a value
        /// </summary>
        /// <param name="input"></param>
        [Test]
        [TestCase("#1234 *4.5 ")]
        [TestCase("#1234 *4.5 $")]
        [TestCase("#1234 *4.5 $$")]
        public void wrong_description_inputs(string input)
        {
            Assert.IsFalse(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("#1234 *4.5 $some text 4 and more 3.5", "some text 4 and more 3.5")]
        [TestCase("#1234 $some text 4 and more 3.5 *4.5 more text", "some text 4 and more 3.5")]
        [TestCase("$some text 4 and more 3.5 *4.5 more text", "some text 4 and more 3.5")]
        [TestCase("#1234*4.5$some text 4 and more 3.5", "some text 4 and more 3.5")]
        [TestCase("#1234$some text 4 and more 3.5*4.5 more text", "some text 4 and more 3.5")]
        [TestCase("$some text 4 and more 3.5*4.5 more text", "some text 4 and more 3.5")]
        public void correct_description_inputs(string input, string expectedResult)
        {
            Assert.IsTrue(_logEntryInputParser.TryParse(input, out var logEntry));
            Assert.AreEqual(expectedResult, logEntry.Description);
        }

        /// <summary>
        /// Label (@) is optional, so cannot do much wrong
        /// </summary>
        /// <param name="input"></param>
        [Test]
        [TestCase("#1234 $required description *4.5 @@")]
        public void wrong_label_inputs(string input)
        {
            Assert.IsFalse(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("#1234 *4.5 $required description @some text 4 and more 3.5", "some text 4 and more 3.5")]
        [TestCase("#1234 @some text 4 and more 3.5 *4.5 $required description more text", "some text 4 and more 3.5")]
        [TestCase("@some text 4 and more 3.5 *4.5 $required description more text", "some text 4 and more 3.5")]
        public void correct_label_inputs(string input, string expectedResult)
        {
            Assert.IsTrue(_logEntryInputParser.TryParse(input, out var logEntry));
            Assert.AreEqual(expectedResult, logEntry.Label);
        }
    }
}