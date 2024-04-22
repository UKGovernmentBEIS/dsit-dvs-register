namespace DVSRegister.CommonUtility.Models
{
    public class GovUkNotifyConfiguration
    {
        public const string ConfigSection = "GovUkNotify";
        public string ApiKey { get; set; }

        public EmailConfirmationTemplate EmailConfirmationTemplate { get; set; }
    }
}
