

using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.DSITEdit2i;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Models
{
    public class ServiceDraftDto
    {
        public int Id { get; set; }
        public string? ServiceName { get; set; }
        public string? WebSiteAddress { get; set; }
        public string? CompanyAddress { get; set; }
        public bool? HasGPG44 { get; set; }
        public bool? HasGPG45 { get; set; }
        public bool? HasSupplementarySchemes { get; set; }
        public DateTime? ConformityIssueDate { get; set; }
        public DateTime? ConformityExpiryDate { get; set; }
        public ServiceStatusEnum PreviousServiceStatus { get; set; }
        public DateTime ModifiedTime { get; set; }

        public int ServiceId { get; set; }
        public ServiceDto Service { get; set; }
        public int ProviderProfileId { get; set; }
        public ProviderProfileDto Provider { get; set; }

        public int RequestedUserId { get; set; }
        public UserDto User { get; set; }

        public ICollection<ServiceRoleMappingDraftDto> ServiceRoleMappingDraft { get; set; } = new List<ServiceRoleMappingDraftDto>();
        public ICollection<ServiceQualityLevelMappingDraftDto> ServiceQualityLevelMappingDraft { get; set; } = new List<ServiceQualityLevelMappingDraftDto>();
        public ICollection<ServiceIdentityProfileMappingDraftDto> ServiceIdentityProfileMappingDraft { get; set; } = new List<ServiceIdentityProfileMappingDraftDto>();
        public ICollection<ServiceSupSchemeMappingDraftDto> ServiceSupSchemeMappingDraft { get; set; } = new List<ServiceSupSchemeMappingDraftDto>();
        public bool? IsUnderpinningServicePublished { get; set; }
        public int? UnderPinningServiceId { get; set; }
        public ServiceDto UnderPinningService { get; set; }
        public int? ManualUnderPinningServiceId { get; set; }
        public ManualUnderPinningServiceDto ManualUnderPinningService { get; set; }
        public ManualUnderPinningServiceDraftDto ManualUnderPinningServiceDraft { get; set; } // for newly entered service
    }
}