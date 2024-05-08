namespace DVSRegister.Models.CAB
{
    public class CertificateInfoSummaryViewModel
    {
        public int? PreRegistrationId { get; set; }       
        public string? RegisteredName { get; set; }
        public string? TradingName { get; set; }
        public string? PublicContactEmail { get; set; }
        public string? TelephoneNumber { get; set; }
        public string? WebsiteAddress { get; set; }
        public string? Address { get; set; }
        public string? ServiceName { get; set; }
        public bool? HasSupplementarySchemes { get; set; }
        public SupplementarySchemeViewModel? SupplementarySchemeViewModel { get; set; }
        public IdentityProfileViewModel? IdentityProfileViewModel { get; set; }
        public RoleViewModel? RoleViewModel { get; set; }
        public string? FileName { get; set; }
        public string? FileLink { get; set; }
        public DateTime? ConformityIssueDate { get; set; }
        public DateTime? ConformityExpiryDate { get; set; }
    }
}
