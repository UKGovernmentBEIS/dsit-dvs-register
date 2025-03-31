﻿namespace DVSRegister.CommonUtility
{
    public static class Constants
    {       
        public const string DbContextNull = "Db context is null";
        public const string DbConnectionFailed = "DB connection failed:";
        public const string DbConnectionSuccess = "DB connection success:";
        public const string ApplicationSubmittedMessage = "Success";
        public const string ConfirmationMessage = "Confirm if the information you've provided is correct";  
        public const string CabRegistrationErrorPath = "/cab-service/service-error";
        public const string CommonErrorPath = "/service-error";
         
        public const int DaysLeftToComplete = 21;        

        public const string ConformityIssueDayError = "The certificate of conformity issue date must include a day";
        public const string ConformityIssueMonthError = "The certificate of conformity issue date must include a month";
        public const string ConformityIssueYearError = "The certificate of conformity issue date must include a year";
        public const string ConformityIssuePastDateError = "The certificate of conformity issue date must be today or in the past";
        public const string ConformityIssueDateInvalidError = "The certificate of conformity issue date must be a real date";

        public const string ConformityExpiryDayError = "The certificate of conformity expiry date must include a day";
        public const string ConformityExpiryMonthError = "The certificate of conformity expiry date must include a month";
        public const string ConformityExpiryYearError = "The certificate of conformity expiry date must include a year";
        public const string ConformityExpiryDateError = "The certificate of conformity expiry date must be today or in the past";
        public const string ConformityMaxExpiryDateError = "The certificate of conformity expiry date must not be more than 2 years 60 days after the date of issue";
        public const string ConformityExpiryDateInvalidError = "The certificate of conformity expiry date must be a real date";
        public const string ConformityIssueDateExpiryDateError = "The certificate of conformity expiry date cannot be before issue date";
        public const string ConformityExpiryPastDateError = "The certificate of conformity expiry date must be in the future";
        public const string IncorrectPassword = "Incorrect Password";
        public const string IncorrectLoginDetails = "Enter a valid email address and password. After five incorrect attempts, your account will be temporarily locked";
        public const string NullFieldsDisplay = "No data";

        public const string RegisteredNameExistsError = "The registered name you have entered already exists";


    }
}
