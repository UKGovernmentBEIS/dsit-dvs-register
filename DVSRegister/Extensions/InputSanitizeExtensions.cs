namespace DVSRegister.Extensions
{
    public static class InputSanitizeExtensions
    {
        public static  string CleanseInput(string input)
        {
            // Example: Remove any HTML tags and encode special characters
            string cleansedInput = System.Web.HttpUtility.HtmlEncode(System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", ""));
            return cleansedInput;
        }
    }
}
