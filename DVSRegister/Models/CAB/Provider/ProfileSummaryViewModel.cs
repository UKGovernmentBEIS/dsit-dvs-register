namespace DVSRegister.Models.CAB
{
    public class ProfileSummaryViewModel
    {     
        public string? RegisteredName { get; set; }
        public string? TradingName { get; set; }
        public bool? HasRegistrationNumber { get; set; }
        public string? CompanyRegistrationNumber { get; set; }
        public string? DUNSNumber { get; set; }      
        public PrimaryContactViewModel? PrimaryContact { get; set; }
        public SecondaryContactViewModel? SecondaryContact { get; set; }
        public string? PublicContactEmail { get; set; }
        public string? ProviderTelephoneNumber { get; set; }
        public string? ProviderWebsiteAddress { get; set; }
    }
}
