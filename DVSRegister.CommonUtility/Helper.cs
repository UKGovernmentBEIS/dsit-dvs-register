namespace DVSRegister.CommonUtility
{
    public static class Helper
    {
        public static string GetLocalDateTime(DateTime? dateTime, string format)
        {
            DateTime dateTimeValue = Convert.ToDateTime(dateTime);
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local; // Get local time zone
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(dateTimeValue, localTimeZone); // Convert to local time
            string time = localTime.ToString(format);
            return time;
        }

    }
}
