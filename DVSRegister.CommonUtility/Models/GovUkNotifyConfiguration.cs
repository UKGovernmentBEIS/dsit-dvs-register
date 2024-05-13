namespace DVSRegister.CommonUtility.Models
{
    public class GovUkNotifyConfiguration
    {
        public const string ConfigSection = "GovUkNotify";
        public string ApiKey { get; set; }
        public string OfDiaEmailId { get; set; }
        public string LoginLink { get; set; }

        public EmailConfirmationTemplate EmailConfirmationTemplate { get; set; }
        public ApplicationReceivedTemplate ApplicationReceivedTemplate { get; set; }
    }
}
