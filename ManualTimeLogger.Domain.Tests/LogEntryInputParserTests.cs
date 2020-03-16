using System.Collections.Generic;
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
            _logEntryInputParser = new LogEntryInputParser(new List<string>{"roi", "nb", "nwb", "norma", "sogyo", "liniebreed"});
        }

        [Test]
        [TestCase("nb !anders *4.5 $some text and more #1234 *2")]
        [TestCase("nb !anders *4.5 $some text and more #1234 #2")]
        [TestCase("nb *4.5 $some text and more #1234 $2")]
        [TestCase("nb $some text and more #1234")]
        [TestCase("nb *4.5 #1234")]
        [TestCase("")]
        public void wrong_input_input_layouts(string input)
        {
            Assert.IsFalse(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("nb *4.5 $some text and more #1234!anders ")]
        [TestCase("nb $some text and more *8!anders ")]
        [TestCase("nb *5 #1234 $some text and more!anders ")]
        [TestCase("nb *4.5$some text and more#1234!anders ")]
        [TestCase("nb $some text and more*8!anders ")]
        [TestCase("nb *5#1234$some text and more!anders ")]
        public void correct_input_input_layouts(string input)
        {
            Assert.IsTrue(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("nb #-1234 *4.5 $some text and more!anders ")]
        [TestCase("nb #123.4 *4.5 $some text and more!anders ")]
        [TestCase("nb #text *4.5 $some text and more!anders ")]
        public void wrong_issue_number_inputs(string input)
        {
            Assert.IsFalse(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("nb #1234 *4.5 $some text and more!anders ", 1234)]
        [TestCase("nb #0 *4.5 $some text and more!anders ", 0)]
        [TestCase("nb # *4.5 $some text and more!anders ", 0)]
        [TestCase("nb #1234*4.5$some text and more!anders ", 1234)]
        [TestCase("nb #0*4.5$some text and more!anders ", 0)]
        [TestCase("nb #*4.5$some text and more!anders ", 0)]
        public void correct_issue_number_inputs(string input, int expectedResult)
        {
            Assert.IsTrue(_logEntryInputParser.TryParse(input, out var logEntry));
            Assert.AreEqual(expectedResult, logEntry.IssueNumber);
        }

        [Test]
        [TestCase("nb !anders #1234 *4.5. $some text and more")]
        [TestCase("nb !anders #1234 *-4.5 $some text and more")]
        [TestCase("nb !anders #1234 *text $some text and more")]
        [TestCase("nb !anders #1234 *0 $some text and more")]
        [TestCase("nb !anders #1234 * $some text and more")]
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
        [TestCase("nb !Mailen #1234 *4.5 $some text and more", 4.5f)]
        [TestCase("nb !anders#1234 *4,5 $some text and more", 4.5f)]
        [TestCase("nb !Anders#1234 *4 $some text and more", 4)]
        [TestCase("nb #1234 *0.1 $some text and more !Anders", 0.1f)]
        [TestCase("nb #1234*4.5!Anders$some text and more", 4.5f)]
        [TestCase("nb !Anders#1234*4,5$some text and more", 4.5f)]
        [TestCase("nb #1234!Anders*4 $some text and more", 4)]
        [TestCase("nb #1234!Anders*0.1$some text and more", 0.1f)]
        [TestCase("nb #1234!Anders*0:30$some text and more", 0.5f)]
        [TestCase("nb #1234!Anders*0:03$some text and more", 0.05f)]
        [TestCase("nb #1234!Anders*0:3$some text and more", 0.05f)]
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
        [TestCase("nb #1234 *4.5 ")]
        [TestCase("nb !anders #1234 *4.5 $")]
        [TestCase("nb !anders #1234 *4.5 $$")]
        public void wrong_description_inputs(string input)
        {
            Assert.IsFalse(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("nb #1234 *4.5 $some text 4 and more 3.5", "some text 4 and more 3.5")]
        [TestCase("nb #1234 $some text 4 and more 3.5 *4.5 more text", "some text 4 and more 3.5")]
        [TestCase("nb !Telefoneren $some text 4 and more 3.5 *4.5 more text", "some text 4 and more 3.5")]
        [TestCase("nb !Meeting #1234*4.5$some text 4 and more 3.5", "some text 4 and more 3.5")]
        [TestCase("nb !Meeting #1234$some text 4 and more 3.5*4.5 more text", "some text 4 and more 3.5")]
        [TestCase("nb !Meeting $some text 4 and more 3.5*4.5 more text", "some text 4 and more 3.5")]
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
        [TestCase("nb #1234 $required description *4.5 @@")]
        public void wrong_label_inputs(string input)
        {
            Assert.IsFalse(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("nb !anders #1234 *4.5 $required description @some text 4 and more 3.5", "some text 4 and more 3.5")]
        [TestCase("nb #1234 @some text 4 and more 3.5 *4.5 $required description more text", "some text 4 and more 3.5")]
        [TestCase("nb !anders @some text 4 and more 3.5 *4.5 $required description more text", "some text 4 and more 3.5")]
        public void correct_label_inputs(string input, string expectedResult)
        {
            Assert.IsTrue(_logEntryInputParser.TryParse(input, out var logEntry));
            Assert.AreEqual(expectedResult, logEntry.Label);
        }

        /// <summary>
        /// Activity (!) is optional, so cannot do much wrong
        /// </summary>
        /// <param name="input"></param>
        [Test]
        [TestCase("nb #1234 $required description *4.5 !!")]
        public void wrong_activity_inputs(string input)
        {
            Assert.IsFalse(_logEntryInputParser.TryParse(input, out _));
        }

        [Test]
        [TestCase("nb !mailen #1234 *4.5 $required description @some text 4 and more 3.5", "some text 4 and more 3.5")]
        [TestCase("nb #1234 @some text 4 and more 3.5 *4.5 $required description more text", "some text 4 and more 3.5")]
        [TestCase("nb @some text 4 and more 3.5 !testen *4.5 $required description more text", "some text 4 and more 3.5")]
        public void correct_activity_inputs(string input, string expectedResult)
        {
            Assert.IsTrue(_logEntryInputParser.TryParse(input, out var logEntry));
            Assert.AreEqual(expectedResult, logEntry.Label);
        }
    }
}