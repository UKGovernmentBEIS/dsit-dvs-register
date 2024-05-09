namespace DVSRegister.CommonUtility
{
    public static class Constants
    {       
        public const string DbContextNull = "Db context is null";
        public const string DbConnectionFailed = "DB connection failed:";
        public const string DbConnectionSuccess = "DB connection success:";
        public const string ApplicationSubmittedMessage = "Success";
        public const string ConfirmationMessage = "Confirm if the information you've provided is correct";
        public const string FailedMessage = "Failed";
        public const string PreRegistrationErrorPath = "/pre-registration/service-error";
        public const int URNExpiryDays = 60;
        public const string URNErrorMessage = "Invalid unique reference number. Please try again. If you are still \r\nexperiencing issues, please reach out to the digital identity and \r\nattribute service provider to confirm the correct data.";
    }
}
