using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models.CAB.Service;
using DVSRegister.Models.CabTrustFramework;
using DVSRegister.Validations;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class ServiceSummaryViewModel :ServiceSummaryBaseViewModel
    {
        public int ProviderProfileId { get; set; }     

        [Required(ErrorMessage = "Enter the service name")]
        [MaximumLength(160, ErrorMessage = "The service name must be less than 161 characters")]
        [AcceptedCharacters(@"^[A-Za-z0-9 &@#().:_'-]+$", ErrorMessage = "The service name must contain only letters, numbers and accepted characters")]
        public string? ServiceName { get; set; }

        [Required(ErrorMessage = "Enter the service website address")]
        [RegularExpression(@"^(https?:\/\/)?(www\.)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(\/[^\s]*)?$", ErrorMessage = "Enter a valid website address")]
        public string? ServiceURL { get; set; }

        [Required(ErrorMessage = "Enter the company address")]
        [AcceptedCharacters(@"^[A-Za-zÀ-ž0-9 &@£$€¥(){}!«»""'''?""/*=#%+.,:;\r\n\\/-]+$", ErrorMessage = "The company address must contain only letters, numbers and accepted characters")]
        public string? CompanyAddress { get; set; }
        public RoleViewModel? RoleViewModel { get; set; }

        [Required(ErrorMessage = "Select ‘Yes’ if the service is certified against GPG44")]
        public bool? HasGPG44 { get; set; }
    

        [Required(ErrorMessage = "Select ‘Yes’ if the service is certified against GPG45")]
        public bool? HasGPG45 { get; set; }
        public QualityLevelViewModel? QualityLevelViewModel { get; set; }
        public IdentityProfileViewModel? IdentityProfileViewModel { get; set; }

        [Required(ErrorMessage = "Select ‘Yes’ if the service is certified against a supplementary scheme")]
        public bool? HasSupplementarySchemes { get; set; }
        public SupplementarySchemeViewModel? SupplementarySchemeViewModel { get; set; }
        public string? FileName { get; set; }
        public string? FileLink { get; set; }
        public decimal? FileSizeInKb { get; set; }
        public DateTime? ConformityIssueDate { get; set; }
        public DateTime? ConformityExpiryDate { get; set; }      
        public int CabUserId { get; set; }
        public int CabId { get; set; }   
        public int ServiceId { get; set; }
        public int ServiceKey { get; set; }      
        public DateTime? CreatedDate { get; set; }
        public TFVersionViewModel? TFVersionViewModel { get; set; }

        public int? SelectedUnderPinningServiceId { get; set; }
        public int? SelectedManualUnderPinningServiceId { get; set; }
        public bool? IsManualServiceLinkedToMultipleServices { get; set; }

        [RequiredEnumValue(ErrorMessage = "Select the service type")]
        public ServiceTypeEnum? ServiceType { get; set; }

        [Required(ErrorMessage = "Select the registration status")]
        public bool? IsUnderpinningServicePublished { get; set; }
        public List<SchemeQualityLevelMappingViewModel>? SchemeQualityLevelMapping { get; set; }
        public List<SchemeIdentityProfileMappingViewModel>? SchemeIdentityProfileMapping { get; set; }

        [Required(ErrorMessage = "Enter the service name")]
        [MaximumLength(160, ErrorMessage = "The service name must be less than 161 characters")]
        [AcceptedCharacters(@"^[A-Za-z0-9 &@#().:_'-]+$", ErrorMessage = "The service name must contain only letters, numbers and accepted characters")]
        public string? UnderPinningServiceName { get; set; }

        [Required(ErrorMessage = "Enter the digital identity and attribute provider's registered name")]
        [MaximumLength(160, ErrorMessage = "The company's registered name must be less than 161 characters")]
        [AcceptedCharacters(@"^[A-Za-zÀ-ž &@£$€¥(){}\[\]!«»""'''?""/*=#%+0-9.,:;\\/-]+$", ErrorMessage = "The company's registered name must contain only letters, numbers and accepted characters")]
        public string? UnderPinningProviderName { get; set; }
        public SelectCabViewModel? SelectCabViewModel { get; set; }
        public DateTime? UnderPinningServiceExpiryDate { get; set; }
        public bool? IsTFVersionChanged { get; set; }
        public bool? IsReupload { get; set; }

       
    }
}
