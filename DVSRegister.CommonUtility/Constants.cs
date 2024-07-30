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
        public const string CabRegistrationErrorPath = "/cab-service/service-error";
        public const string CommonErrorPath = "/service-error";

        public const int URNExpiryDays = 60;
        public const string URNRequiredError = "Enter a unique reference number";
        public const string URNErrorMessage = "Invalid unique reference number. Please try again. If you are still \r\nexperiencing issues, please reach out to the digital identity and \r\nattribute service provider to confirm the correct data.";      
        public const int DaysLeftToComplete = 21;
        public const string SupplementarySchemeErrorMessage = "Select if the digital identity and attribute service provider is certified against any supplementary schemes on their certificate";
        public const string ConformityIssueDayError = "The certificate of confirmity issue date must include a day";
        public const string ConformityIssueMonthError = "The certificate of confirmity issue date must include a month";
        public const string ConformityIssueYearError = "The certificate of confirmity issue date must include a year";
        public const string ConformityIssuePastDateError = "The certificate of confirmity issue date must be today or in past";
        public const string ConformityIssueDateInvalidError = "The certificate of confirmity issue date must be a real date";

        public const string ConformityExpiryDayError = "The certificate of confirmity expiry date must include a day";
        public const string ConformityExpiryMonthError = "The certificate of confirmity expiry date must include a month";
        public const string ConformityExpiryYearError = "The certificate of confirmity expiry date must include a year";
        public const string ConformityExpiryDateError = "The certificate of confirmity expiry date must be today or in past";
        public const string ConformityMaxExpiryDateError = "The certificate of confirmity expiry date must not be more than 2 years after the date of issue";
        public const string ConformityExpiryDateInvalidError = "The certificate of confirmity expiry date must be a real date";
        public const string ConformityIssueDateExpiryDateError = "The certificate of confirmity expiry date cannot be before issue date";
        public const string ConformityExpiryPastDateError = "The certificate of confirmity expiry date must be in future";
        public const string IncorrectPassword = "Incorrect Password";
        public const string IncorrectLoginDetails = "Enter a valid email address and password. After five incorrect attempts, your account will be temporarily locked";


    }
}
