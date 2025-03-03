using DVSRegister.CommonUtility.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;
using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.Data.Entities
{
    public class Service
    {
        public Service() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("ProviderProfile")]
        public int ProviderProfileId { get; set; }
        public ProviderProfile Provider { get; set; }
        public string? ServiceName { get; set; }
        public int ServiceKey { get; set; }
        public int ServiceVersion { get; set; } = 1;
        public bool IsCurrent { get; set; } = true;
        public string? WebSiteAddress { get; set; }
        public string? CompanyAddress { get; set; }
        public ICollection<ServiceRoleMapping>? ServiceRoleMapping { get; set; }
        public bool? HasGPG44 { get; set; }       
        public ICollection<ServiceQualityLevelMapping>? ServiceQualityLevelMapping { get; set; }
        public bool? HasGPG45 { get; set; }
        public ICollection<ServiceIdentityProfileMapping>? ServiceIdentityProfileMapping { get; set; }
        public bool? HasSupplementarySchemes { get; set; }
        public ICollection<ServiceSupSchemeMapping>? ServiceSupSchemeMapping { get; set; }

        public string? FileName { get; set; }
        public string? FileLink { get; set; }

        [Column(TypeName = "decimal(10, 1)")]
        public decimal? FileSizeInKb { get; set; }
        public DateTime? ConformityIssueDate { get; set; }
        public DateTime? ConformityExpiryDate { get; set; }
        
        [ForeignKey("CabUser")]
        public int CabUserId { get; set; }
        public CabUser CabUser { get; set; }
        public ServiceStatusEnum ServiceStatus { get; set; }      
        public DateTime? CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public DateTime? PublishedTime { get; set; }
        public CertificateReview CertificateReview { get; set; }
        public ProceedApplicationConsentToken ProceedApplicationConsentToken { get; set; }
        public ServiceRemovalReasonEnum? ServiceRemovalReason { get; set; }
        public string? RemovalReasonByCab { get; set; }
        public DateTime? RemovalRequestTime { get; set; }
        public DateTime? RemovedTime { get; set; }
    }
}
