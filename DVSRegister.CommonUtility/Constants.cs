namespace DVSRegister.CommonUtility
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
        public const string ConformityMaxExpiryDateErrorTF0_4 = "The certificate of conformity expiry date must not be more than 3 years 60 days after the date of issue";
        public const string ConformityExpiryDateInvalidError = "The certificate of conformity expiry date must be a real date";
        public const string ConformityIssueDateExpiryDateError = "The certificate of conformity expiry date cannot be before issue date";
        public const string ConformityExpiryPastDateError = "The certificate of conformity expiry date must be in the future";
        public const string IncorrectPassword = "Incorrect Password";
        public const string IncorrectLoginDetails = "Enter a valid email address and password. After five incorrect attempts, your account will be temporarily locked";
        public const string NullFieldsDisplay = "Not applicable";

        public const string RegisteredNameExistsError = "The registered name you have entered already exists";
        public const string NewApplication = "New application";
        public const string ReApplication = "Reapplication";
        public const decimal TFVersion0_4 = 0.4m;
        public const decimal TFVersion0_3 = 0.3m;
        public const string GPG44Authentication = "GPG 44 quality of authentication";
        public const string GPG44Protection = "GPG 44 level of protection";
        public const string GPG45IdentityProfiles = "GPG 45 identity profiles";
        public const string SupplementaryCodes = "Supplementary Codes";
        public const string RegisteredAddress = "Registered address";
        public const string ServiceName = "Service name";
        public const string Roles = "Roles certified against";
        public const string UnderpinningServiceName = "Underpinning service name";
        public const string UnderpinningProviderName = "Underpinning service provider registered name";
        public const string CabOfUnderpinningService = "CAB of underpinning service";
        public const string UnderpiningExpiryDate = "Underpinning service certificate expiry date";
        public const string NotGpg45SubsetError = "The identity profiles you have selected for this service must align to the options you select for any supplementary codes it is also certified against";
        public const string NotGpg44SubsetError = "The quality of authenticator and quality of protection levels you have selected for this service must align to the options you select for any supplementary codes it is certified against";
        public const string ServiceGpg44SelectedNo = "The identity profiles you have selected for this service must align to the options you select for any supplementary codes it is also certified against";

        public const string RegisteredName = "Registered name";
        public const string TradingName = "Trading name";
        public const string CompanyRegistrationNumber = "Companies House or charity registration number";
        public const string DUNSNumber = "D U-N-S number";
        public const string ParentCompanyRegisteredName = "Registered name of parent company";
        public const string ParenyCompanyLocation = "Location of parent company";

        public const string PrimaryContactName = "Primary contact full name";
        public const string PrimaryContactEmail = "Primary contact email";
        public const string PrimaryContactJobTitle = "Primary contact job title";
        public const string PrimaryContactTelephone = "Primary contact telephone number";

        public const string SecondaryContactName = "Secondary contact full name";
        public const string SecondaryContactEmail = "Secondary contact email";
        public const string SecondaryContactJobTitle = "Secondary contact job title";
        public const string SecondaryContactTelephone = "Secondary contact telephone number";

        public const string ProviderWebsiteAddress = "Provider website address";
        public const string LinkToContactPage = "Link to contact page";
        public const string PublicContactEmail = "Public contact email";
        public const string ProviderTelephoneNumber = "Provider telephone number";

        public const string InvitationDeclined = "Invitation declined";

    }
}
