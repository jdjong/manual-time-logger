using NUnit.Framework;

namespace ManualTimeLogger.Domain.Tests
{
    [TestFixture]
    public class InputPartWithoutSpacesSelectorTests
    {
        private InputPartWithoutSpacesSelector _selector;

        [SetUp]
        public void SetUp()
        {
            _selector = new InputPartWithoutSpacesSelector("#");
        }

        [Test]
        [TestCase("$test test #123 #123")]
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
        [TestCase("$test test #123 @qwe", "123")]
        [TestCase("#123 $test test @qwe", "123")]
        [TestCase("@qwe $test test #123", "123")]
        public void SuccessResults(string input, string expectedInputPart)
        {
            var selectorResult = _selector.Get(input);
            Assert.IsTrue(selectorResult.IsSuccess);
            Assert.AreEqual(selectorResult.InputPart, expectedInputPart);
            Assert.IsNull(selectorResult.FailedMessage);
        }

    }
}
