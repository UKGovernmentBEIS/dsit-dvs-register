using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.BusinessLogic.Models.CAB
{
    public class CertificateInfoDto
    {       
        public int Id { get; set; }

        public int ProviderId { get; set; }
        public ProviderDto Provider { get; set; }
        public string ServiceName { get; set; }
        public ICollection<CertificateInfoRoleMappingDto> CertificateInfoRoleMappings { get; set; }
        public ICollection<CertificateInfoIdentityProfileMappingDto> CertificateInfoIdentityProfileMappings { get; set; }
        public bool HasSupplementarySchemes { get; set; }
        public ICollection<CertificateInfoSupSchemeMappingDto>? CertificateInfoSupSchemeMappings { get; set; }

        public string FileName { get; set; }
        public string FileLink { get; set; }
        public DateTime ConformityIssueDate { get; set; }
        public DateTime ConformityExpiryDate { get; set; }
        public CertificateInfoStatusEnum CertificateInfoStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? SubmittedCAB { get; set; }
        public List<RoleDto> Roles { get; set; }
        public List<IdentityProfileDto> IdentityProfiles { get; set; }
        public List<SupplementarySchemeDto>? SupplementarySchemes { get; set; }
    }
}
