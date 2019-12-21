using NUnit.Framework;

namespace ManualTimeLogger.Domain.Tests
{
    [TestFixture]
    public class InputPartSelectorTests
    {
        private InputPartSelector _selector;

        [SetUp]
        public void SetUp()
        {
            _selector = new InputPartSelector("#", "$@#*");
        }

        [Test]
        [TestCase("$test test #123 asd #123 asd")]
        [TestCase("$test test @123")]
        public void FailedResults(string input)
        {
            var selectorResult = _selector.Get(input);
            Assert.IsFalse(selectorResult.IsSuccess);
            Assert.IsNull(selectorResult.InputPart);
            Assert.IsNotEmpty(selectorResult.FailedMessage);
        }

        [Test]
        [TestCase("$test test #", "")]
        [TestCase("$test test #123 asd @qwe", "123 asd")]
        [TestCase("#123 asd $test test @qwe", "123 asd")]
        [TestCase("@qwe $test test #123 asd", "123 asd")]
        [TestCase("$test test#123 asd@qwe", "123 asd")]
        [TestCase("#123 asd$test test@qwe", "123 asd")]
        [TestCase("@qwe$test test#123 asd", "123 asd")]
        public void SuccessResults(string input, string expectedInputPart)
        {
            var selectorResult = _selector.Get(input);
            Assert.IsTrue(selectorResult.IsSuccess);
            Assert.AreEqual(selectorResult.InputPart, expectedInputPart);
            Assert.IsNull(selectorResult.FailedMessage);
        }

    }
}