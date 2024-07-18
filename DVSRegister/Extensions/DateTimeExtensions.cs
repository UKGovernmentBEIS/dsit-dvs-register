
using DVSRegister.CommonUtility;
using Microsoft.AspNetCore.Html;


namespace DVSRegister.Extensions
{
    public static class DateTimeExtensions
    {
        public static HtmlString FormatDateTime(DateTime? dateTime, string format )
        {          
            string date = Helper.GetLocalDateTime(dateTime, format);           
            return new HtmlString($"{date}") ;
        }
    }
}
