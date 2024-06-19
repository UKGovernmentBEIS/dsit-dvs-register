using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.Data.Entities
{
    public class CertificateInformation :BaseEntity
    {
        public CertificateInformation() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Provider")]
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }
        public string ServiceName { get; set; }
        public ICollection<CertificateInfoRoleMapping> CertificateInfoRoleMappings { get; set; }
        public ICollection<CertificateInfoIdentityProfileMapping> CertificateInfoIdentityProfileMappings { get; set; }
        public bool HasSupplementarySchemes { get; set; }
        public ICollection<CertificateInfoSupSchemeMapping>? CertificateInfoSupSchemeMappings { get; set; }

        public string FileName { get; set; }
        public string FileLink { get; set; }
        public DateTime ConformityIssueDate { get; set; }
        public DateTime ConformityExpiryDate { get; set; }
        public CertificateInfoStatusEnum CertificateInfoStatus { get;set; }
        public string? SubmittedCAB { get; set; }
    }
}
