﻿using DVSAdmin.CommonUtility.Models.Enums;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using System.Text.Json.Serialization;

namespace DVSRegister.BusinessLogic.Models.CAB
{
    public class ProviderProfileDto
    {
        public int Id { get; set; }
        public string RegisteredName { get; set; }
        public string TradingName { get; set; }
        public bool HasRegistrationNumber { get; set; }
        public string? CompanyRegistrationNumber { get; set; }
        public string? DUNSNumber { get; set; }
        public bool HasParentCompany { get; set; }
        public string? ParentCompanyRegisteredName { get; set; }
        public string? ParentCompanyLocation { get; set; }
        public string PrimaryContactFullName { get; set; }
        public string PrimaryContactJobTitle { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string PrimaryContactTelephoneNumber { get; set; }
        public string SecondaryContactFullName { get; set; }
        public string SecondaryContactJobTitle { get; set; }
        public string SecondaryContactEmail { get; set; }
        public string SecondaryContactTelephoneNumber { get; set; }
        public string? PublicContactEmail { get; set; }
        public string? ProviderTelephoneNumber { get; set; }
        public string ProviderWebsiteAddress { get; set; }
        public int CompanyId { get; set; }
        public int CabUserId { get; set; }
        public CabUser CabUser { get; set; }
        public ProviderStatusEnum ProviderStatus { get; set; }

        [JsonIgnore]
        public ICollection<ServiceDto>? Services { get; set; }  
        public DateTime? CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public DateTime? PublishedTime { get; set; }
        public DateTimeInfoDto LastUpdatedInfo { get; set; }
        public RemovalReasonsEnum? RemovalReason { get; set; }
    }
}
