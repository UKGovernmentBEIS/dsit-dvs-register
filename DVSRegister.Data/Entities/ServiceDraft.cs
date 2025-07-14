using DVSRegister.CommonUtility.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class ServiceDraft
    {
        public ServiceDraft() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? ServiceName { get; set; }
        public string? WebSiteAddress { get; set; }
        public string? CompanyAddress { get; set; }
        public ICollection<ServiceRoleMappingDraft>? ServiceRoleMappingDraft { get; set; }
        public bool? HasGPG44 { get; set; }       
        public ICollection<ServiceQualityLevelMappingDraft>? ServiceQualityLevelMappingDraft { get; set; }
        public bool? HasGPG45 { get; set; }
        public ICollection<ServiceIdentityProfileMappingDraft>? ServiceIdentityProfileMappingDraft { get; set; }
        public bool? HasSupplementarySchemes { get; set; }
        public ICollection<ServiceSupSchemeMappingDraft>? ServiceSupSchemeMappingDraft { get; set; }
      
        public DateTime? ConformityIssueDate { get; set; }
        public DateTime? ConformityExpiryDate { get; set; }
        
        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [ForeignKey("ProviderProfile")]
        public int ProviderProfileId { get; set; }
        public ProviderProfile Provider { get; set; }

        [ForeignKey("User")]
        public int RequestedUserId{ get; set; }
        public User User { get; set; } 
        public ServiceStatusEnum PreviousServiceStatus { get; set; }        
        public DateTime ModifiedTime { get; set; }
        public bool? IsUnderpinningServicePublished { get; set; }
     
        [ForeignKey("UnderPinningService")]
        public int? UnderPinningServiceId { get; set; }
        public Service UnderPinningService { get; set; }

        [ForeignKey("ManualUnderPinningServiceDraft")]
        public int? ManualUnderPinningServiceDraftId { get; set; }
        public ManualUnderPinningServiceDraft ManualUnderPinningServiceDraft { get; set; }

        [ForeignKey("ManualUnderPinningService")]
        public int? ManualUnderPinningServiceId { get; set; }
        public ManualUnderPinningService ManualUnderPinningService { get; set; }
    }
}
