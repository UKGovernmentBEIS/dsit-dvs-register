
using DVSRegister.CommonUtility;
using Microsoft.AspNetCore.Html;


namespace DVSRegister.Extensions
{
    public static class DateTimeExtensions
    {
        public static HtmlString FormatDateTime(DateTime? dateTime)
        {
            DateTime dateTimeValue = Convert.ToDateTime(dateTime);
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local; // Get local time zone
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(dateTimeValue, localTimeZone); // Convert to local time
            string time = (localTime.Hour, localTime.Minute) switch
            {
                (12, 0) => "Midday",
                (0, 0) => "Midnight",
                _ => localTime.ToString("h:mmtt").ToLower()
            };

            string date = localTime.ToString("d MMM yyyy");
            return new HtmlString($"{date}; {time}");
        }
        public static HtmlString FormatDateTime(DateTime? dateTime, string format )
        {          
            string date = Helper.GetLocalDateTime(dateTime, format);           
            return new HtmlString($"{date}") ;
        }
    }
}
