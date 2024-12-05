using System.Text.RegularExpressions;

namespace Unit_testing
{
    public class Tests
    {
        private string _pattern;

        [SetUp]
        public void Setup()
        {
            _pattern = @"^[A-Za-zÀ-ž&@£$€¥(){}\[\]<>!«»“”'‘’?""/*=#%+0-9.,:;\\/-]+$";
        }

        [Test]
        [TestCase("ValidName123", true)]
        [TestCase("NameWithAccentsÀÁÂ", true)]
        [TestCase("NameWithSymbols&@£$€¥", true)]
        [TestCase("NameWithPunctuation.,:;-", true)]
        [TestCase("NameWithBrackets()", true)]
        [TestCase("NameWithBrackets[]", true)]
        [TestCase("NameWithBrackets{}", true)]
        [TestCase("NameWithBrackets<>", true)]
        [TestCase("NameWithExclamation!", true)]
        [TestCase("NameWithGuillemet«»", true)]
        [TestCase("NameWithInvertedComma“”", true)]
        [TestCase("NameWithApostrophe‘’", true)]
        [TestCase("NameWithQuestionMark?", true)]
        [TestCase("NameWithSolidus\\/\\", true)]
        [TestCase("NameWithSigns*=", true)]
        [TestCase("NameWithSymbols#%+", true)]
        public void TestAcceptedCharacters(string input, bool expectedIsValid)
        {
            var isValid = Regex.IsMatch(input, _pattern);
            Assert.AreEqual(expectedIsValid, isValid);
        }
    }
}




