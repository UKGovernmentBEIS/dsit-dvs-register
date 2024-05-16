using Microsoft.AspNetCore.Html;

namespace DVSRegister.Extensions
{
    public static class HtmlExtensions
    {
        public static HtmlString ToStringWithLineBreaks(string input)
        {
           
            string output = input?.Replace("\r", "<br>")??string.Empty;    

            return new HtmlString(output);
        }
    }
}
