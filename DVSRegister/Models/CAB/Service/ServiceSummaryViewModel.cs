using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.Models.CAB
{
    public class ServiceSummaryViewModel
    {
        public int ProviderProfileId { get; set; }     
        public string? ServiceName { get; set; }
        public string? WebsiteAddress { get; set; }
        public string? CompanyAddress { get; set; }
        public RoleViewModel? RoleViewModel { get; set; }
        public bool? HasGPG44 { get; set; }
        public QualityLevelViewModel? QualityLevelViewModel { get; set; }
        public IdentityProfileViewModel? IdentityProfileViewModel { get; set; }
        public bool? HasSupplementarySchemes { get; set; }
        public SupplementarySchemeViewModel? SupplementarySchemeViewModel { get; set; }
        public string? FileName { get; set; }
        public string? FileLink { get; set; }
        public decimal? FileSizeInKb { get; set; }
        public DateTime? ConformityIssueDate { get; set; }
        public DateTime? ConformityExpiryDate { get; set; }
        public CertificateInfoStatusEnum CertificateInfoStatus { get; set; }
        public int CabUserId { get; set; }
        public bool FromSummaryPage { get;set; }
    }
}
