using System.Text.RegularExpressions;

namespace DVSRegister.UnitTests.DVSRegister
{
    public class CabServiceControllerTests
    {
        [Theory]
        [InlineData("ValidString123", true)]
        [InlineData("Another Valid String", true)]
        [InlineData("Name & Address", true)]
        [InlineData("User@Name#123", true)]
        [InlineData("Test-String: Example", true)]
        [InlineData("Special_Characters: ()", true)]
        [InlineData("John Doe's Profile", true)]
        [InlineData("Example_123: Test", true)]
        [InlineData("Company Name @2023", true)]
        [InlineData("Valid-Input_Example", true)]
        [InlineData("Invalid String!", false)]
        [InlineData("Invalid#String$", false)]
        [InlineData("Another Invalid String*", false)]
        [InlineData("Invalid String?", false)]
        [InlineData("Invalid String;", false)]
        [InlineData("Invalid String: <script>", false)]
        [InlineData("Invalid String%", false)]
        [InlineData("Invalid String@#^", false)]
        [InlineData("123 Invalid String!", false)]

        public void ServiceNameAcceptedCharacters_Test(string input, bool expectedIsValid)
        {
            string pattern = @"^[A-Za-z0-9 &@#().:_'-]+$";
            var isValid = Regex.IsMatch(input, pattern);
            Assert.Equal(isValid, expectedIsValid);

        }
    }
}
